namespace Unify.Core.Events.Dispatch;
public sealed class NullDispatcher : IDispatcher
{
    public bool DispatchEvent(IEvent @event)
    {
        return true;
    }

    public TReply DispatchRequest<TReply>(IRequest<TReply> request) where TReply : IEvent
    {
        return default!; 
    }

    public IEvent DispatchRequestUnsafe(IRequest request)
    {
        return default!;
    }

    public void Dispose()
    {

    }
}
