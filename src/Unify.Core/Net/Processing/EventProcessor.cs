using System.Collections.Concurrent;
using System.Net.Sockets;
using Unify.Core.Events;
using Unify.Core.Events.Dispatch;
using Unify.Core.Net.IO;

namespace Unify.Core.Net.Processing;

public sealed class EventProcessor : IEventProcessor
{
    //Todo - use a shutdown method instead of doing everything in Dispose();
    //also wait for threads to exit properly

    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public bool Connected { get => !_disposed; }

    private Action<Exception> _onFault;
    private readonly IEventStream _eventStream;
    private readonly IDispatcher _dispatcher;

    private BlockingCollection<IEvent> _processQueue;
    private BlockingCollection<IEvent> _writeQueue;
    private CancellationTokenSource _disposeCts;
    private readonly object _lockObject = new();
    private bool _disposed;

    private readonly Dictionary<Guid, RequestAwaiter> _awaiters = new();

    private bool _initializeCalled;

    /// <summary>
    /// Value to keep track of tasks that have been created by this event processor
    /// </summary>
    private int _workerCount = 0;

    public EventProcessor(IEventStream eventStream, IDispatcher dispatcher)
    {
        _eventStream = eventStream;
        _dispatcher = dispatcher;
    }
    
    public void PostEvent(IEvent @event)
    {
        if (_disposed)
            return;

        try
        {
            if (_writeQueue.Count > 3500)
            {
                //PostEvent should return straight away, so use a background task
                _ = Task.Run(() => InternalOnException(new IOException("Too many queued write events")));
                return;
            }

            _writeQueue.Add(@event);
        }
        catch (Exception)
        {
            //Ignore
        }
    }
    
    public TReply SendRequest<TReply>(IRequest<TReply> request) where TReply : IEvent
    {
        Guid requestId = Guid.NewGuid();
        
        using (var awaiter = new RequestAwaiter())
        {
            lock (_lockObject)
            {
                if (_disposed)
                    throw new IOException("The event target is closed");
                
                _awaiters.Add(requestId, awaiter);

                request.EventId = requestId;
                _writeQueue.Add(request);
            }

            try
            {
                return (TReply) awaiter.AwaitReply(5000);
            }
            finally
            {
                lock (_lockObject)
                {
                    _awaiters.Remove(requestId);
                }
            }
        }
    }

    public void BeginBackgroundWorker(Action<Exception> onFault)
    {
        lock (_lockObject)
        {
            _initializeCalled = true;
            _onFault = onFault;

            _processQueue = new();
            _writeQueue = new();
            _disposeCts = new();
            
            Interlocked.Increment(ref _workerCount);
            _ = Task.Run(BackgroundReaderLoop);
            Interlocked.Increment(ref _workerCount);
            _ = Task.Run(BackgroundWriterLoop);
            Interlocked.Increment(ref _workerCount);
            _ = Task.Run(BackgroundProcessorLoop);
        }
    }

    private void BackgroundReaderLoop()
    {
        try
        {
            while (!_disposed)
            {
                var next = _eventStream.ReadEvent();

                lock (_lockObject)
                {
                    if (_awaiters.TryGetValue(next.EventId, out var awaiter))
                    {
                        awaiter.SetReply(next);
                        continue;
                    }
                }

                if (_processQueue.Count > 3500)
                    throw new IOException("Too many queued process events");

                _processQueue.Add(next);
            }
        }
        catch (Exception ex)
        {
            _logger.Trace(ex, "Background reader loop threw exception");

            InternalOnException(ex);
        }
        
        Interlocked.Decrement(ref _workerCount);
        
       if (_logger.IsTraceEnabled)
            _logger.Trace("Background reader task stopped");
    }

    private void BackgroundWriterLoop()
    {
        try
        {
            while (!_disposed)
            {
                var next = _writeQueue.Take(_disposeCts.Token);

                _eventStream.WriteEvent(next);
            }
        }
        catch (Exception ex)
        {
            _logger.Trace(ex, "Background writer loop threw exception");

            InternalOnException(ex);
        }

       if (_logger.IsTraceEnabled)
            _logger.Trace("Background writer task stopped");
        
        Interlocked.Decrement(ref _workerCount);
    }

    private void BackgroundProcessorLoop()
    {
        try
        {
            while (!_disposed)
            {
                var next = _processQueue.Take(_disposeCts.Token);

                if (next is IRequest request)
                    InternalProcessRequest(request, next.EventId);
                else
                    InternalProcessEvent(next);
            }
        }
        catch (Exception ex)
        {
            _logger.Trace(ex, "Background processor loop threw exception");

            InternalOnException(ex);
        }
        
        if (_logger.IsTraceEnabled)
            _logger.Trace("Background processor task stopped");
        
        Interlocked.Decrement(ref _workerCount);
    }

    private void InternalProcessEvent(IEvent @event)
    {
        try
        {
            _dispatcher.DispatchEvent(@event);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to process event type {@event}");
        }
    }

    private void InternalProcessRequest(IRequest request, Guid requestId)
    {
        Interlocked.Increment(ref _workerCount);

        _ = Task.Run(() =>
        {
            try
            {
                var reply = _dispatcher.DispatchRequestUnsafe(request);
                reply.EventId = request.EventId;

                _writeQueue.Add(reply);
            }
            catch (Exception ex)
            {
                lock (_lockObject)
                {
                    if (_disposed)
                        return;

                    _writeQueue.Add(new RequestFailedEvent()
                    {
                        Reason = "The server could not handle the request",
                        EventId = requestId
                    });
                }
                
                _logger.Error(ex, $"Failed to process request type {request}");
            }

            Interlocked.Decrement(ref _workerCount);
        });
    }

    private void InternalOnException(Exception ex)
    {
        if (_disposed)
            return;
        
        lock (_lockObject)
        {
            if (_disposed)
                return;

            _logger.Trace(ex, "Event processor stopping due to exception");
            Dispose();
            _onFault(ex);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        lock (_lockObject)
        {
            if (_disposed)
                return;

            if (!_initializeCalled)
                return;

            _disposed = true;
            _disposeCts.Cancel();

            _writeQueue.CompleteAdding();
            _processQueue.CompleteAdding();

            foreach (var awaiter in _awaiters)
            {
                awaiter.Value.SetReply(new RequestFailedEvent()
                {
                    Reason = "Event processor is closing"
                });
            }

            _awaiters.Clear();
            _eventStream.Dispose();
            
            _writeQueue.Dispose();
            _processQueue.Dispose();
        }

        //Attempt to wait for workers to exit, but dont wait forever
        for (int i = 0; i < 20; i++)
        {
            if (_workerCount <= 1)
            {
                _logger.Debug("Processor threads stopped");
                return;
            }

            Thread.Sleep(100);
        }

        _logger.Warn("Event processor threads did not exit after 2000ms");
    }
}