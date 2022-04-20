using System.Collections.Concurrent;
using Unify.Core.Events;

namespace Unify.Core.Net.IO;

/// <summary>
/// An event stream that uses BlockingCollections instead of
/// serializing to/from a stream
/// </summary>
public sealed class LocalEventStream : IEventStream
{
    private readonly BlockingCollection<IEvent> _input;
    private readonly BlockingCollection<IEvent> _output;
    private readonly object _sharedLock;
    private readonly CancellationTokenSource _disposeCts;

    public static (IEventStream, IEventStream) CreatePair()
    {
        BlockingCollection<IEvent> a = new();
        BlockingCollection<IEvent> b = new();
        var lockObject = new object();
        var cts = new CancellationTokenSource();

        return (new LocalEventStream(a, b, lockObject, cts), new LocalEventStream(b, a, lockObject, cts));
    }

    public LocalEventStream(BlockingCollection<IEvent> input, 
        BlockingCollection<IEvent> output,
        object sharedLock,
        CancellationTokenSource disposeCts)
    {
        _input = input;
        _output = output;
        _sharedLock = sharedLock;
        _disposeCts = disposeCts;
    }
    
    public void WriteEvent(IEvent @event)
    {
        lock (_sharedLock)
        {
            if (_disposeCts.IsCancellationRequested)
                throw new IOException("Stream closed");
    
            _output.Add(@event);
        }
    }

    public IEvent ReadEvent()
    {
        lock (_sharedLock)
        {
            if (_disposeCts.IsCancellationRequested)
                throw new IOException("Stream is closed");
        }
        
        try
        {
            return _input.Take(_disposeCts.Token);
        }
        catch (Exception)
        {
            throw new IOException("Stream closed");
        }
    }

    public ValueTask WriteEventAsync(IEvent @event, CancellationToken ct = default)
    {
        lock (_sharedLock)
        {
            if (_disposeCts.IsCancellationRequested)
                throw new IOException("Stream closed");
    
            _output.Add(@event);
            return ValueTask.CompletedTask;
        }
    }

    public ValueTask<IEvent> ReadEventAsync(CancellationToken ct = default)
    {
        lock (_sharedLock)
        {
            if (_disposeCts.IsCancellationRequested)
                throw new IOException("Stream is closed");
        }
        
        try
        {
            return ValueTask.FromResult(_input.Take(_disposeCts.Token));
        }
        catch (Exception)
        {
            throw new IOException("Stream closed");
        }
    }

    public void Dispose()
    {
        lock (_sharedLock)
        {
            _disposeCts.Cancel();

            //Call dispose on any disposable events
            while(_input.TryTake(out var obj))
            {
                if (obj is IDisposable disposable)
                    disposable.Dispose();
            }

            _input.Dispose();
            _output.Dispose();
        }
    }
}