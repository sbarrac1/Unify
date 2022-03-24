using System.Collections.Concurrent;
using Unify.Core.Events;

namespace Unify.Core.Net.IO;

/// <summary>
/// An event stream that uses BlockingCollections instead of
/// serializing to/from a stream
/// </summary>
public sealed class LocalEventStream : IEventStream
{
    private readonly BlockingCollection<EventWrapper> _input;
    private readonly BlockingCollection<EventWrapper> _output;
    private readonly object _sharedLock;
    private readonly CancellationTokenSource _disposeCts;

    public static (IEventStream, IEventStream) CreatePair()
    {
        BlockingCollection<EventWrapper> a = new();
        BlockingCollection<EventWrapper> b = new();
        var lockObject = new object();
        var cts = new CancellationTokenSource();

        return (new LocalEventStream(a, b, lockObject, cts), new LocalEventStream(b, a, lockObject, cts));
    }

    public LocalEventStream(BlockingCollection<EventWrapper> input, 
        BlockingCollection<EventWrapper> output,
        object sharedLock,
        CancellationTokenSource disposeCts)
    {
        _input = input;
        _output = output;
        _sharedLock = sharedLock;
        _disposeCts = disposeCts;
    }
    
    public void WriteEvent(EventWrapper wrapper)
    {
        lock (_sharedLock)
        {
            if (_disposeCts.IsCancellationRequested)
                throw new IOException("Stream closed");
    
            _output.Add(wrapper);
        }
    }

    public EventWrapper ReadEvent()
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

    public Task WriteEventAsync(EventWrapper wrapper, CancellationToken ct = default)
    {
        lock (_sharedLock)
        {
            if (_disposeCts.IsCancellationRequested)
                throw new IOException("Stream closed");
    
            _output.Add(wrapper);
            return Task.CompletedTask;
        }
    }

    public Task<EventWrapper> ReadEventAsync(CancellationToken ct = default)
    {
        lock (_sharedLock)
        {
            if (_disposeCts.IsCancellationRequested)
                throw new IOException("Stream is closed");
        }
        
        try
        {
            return Task.FromResult(_input.Take(_disposeCts.Token));
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

            _input.Dispose();
            _output.Dispose();
        }
    }
}