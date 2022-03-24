using Unify.Core.Events;
using Unify.Core.Events.Target.Exceptions;

namespace Unify.Core.Net.Processing;

public sealed class RequestAwaiter : IDisposable
{
    private readonly ManualResetEventSlim _waitEvent = new(false);
    
    private IEvent _reply;
    private bool _disposed;

    public IEvent AwaitReply(int timeoutMs)
    {
        if (!_waitEvent.Wait(timeoutMs))
        { 
            Dispose();
            throw new TimeoutException();
        }
        
        lock (this)
        {
            var reply = _reply;
            
            Dispose();
            
            if (reply is RequestFailedEvent failEvent)
                throw new RequestFailedException($"The request failed: {failEvent.Reason}");

            if (reply is null)
                throw new ApplicationException("_reply was null");
            
            return reply;
        }
    }
    
    public void SetReply(IEvent reply)
    {
        lock (this)
        {
            if (_disposed)
                return;
            
            _reply = reply;
            _waitEvent.Set();
        }
    }
    
    public void Dispose()
    {
        lock (this)
        {
            _disposed = true;
            _reply = null;
            _waitEvent.Dispose();
        }
    }
}