using Unify.Core.Events.Dispatch;

namespace Unify.Core.Events.Target;

/// <summary>
/// Mocks an IEventTarget by dispatching the events to a dispatcher
/// </summary>
public sealed class DispatcherEventTarget : IEventTarget
{
    private readonly IDispatcher _dispatcher;
    
    public bool Connected => true;

    public DispatcherEventTarget(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void PostEvent(IEvent @event)
    {
        _dispatcher.DispatchEvent(@event);
    }

    public TReply SendRequest<TReply>(IRequest<TReply> request) where TReply : IEvent
    {
        return _dispatcher.DispatchRequest<TReply>(request);
    }
}
