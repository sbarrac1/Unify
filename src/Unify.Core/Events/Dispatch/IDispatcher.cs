using Unify.Core.Events.Dispatch.Exceptions;

namespace Unify.Core.Events.Dispatch;

/// <summary>
/// Dispatches events and requests to handlers registered within an Ioc context
/// </summary>
public interface IDispatcher : IDisposable
{
    /// <summary>
    /// Dispatches an event to any event handlers registered for the event type
    /// </summary>
    /// <param name="event">The event to dispatch</param>
    /// <exception cref="AggregateException">An exception occured within an event handler</exception>
    /// <returns>True if at least one event handler was called, false if there were no event handlers registered for the
    /// event type</returns>
    bool DispatchEvent(IEvent @event);


    /// <summary>
    /// Dispatches a request to the request handler registered for the given request type
    /// </summary>
    /// <param name="request">The request to dispatch</param>
    /// <typeparam name="TReply">The return type of the request</typeparam>
    /// <returns>The reply returned from the request handler</returns>
    /// <exception cref="NoRequestHandlerException">No request handler was registered for the request type</exception>
    /// <exception cref="AggregateException">An exception occured within the request handler</exception>
    TReply DispatchRequest<TReply>(IRequest<TReply> request)
        where TReply : IEvent;

    /// <summary>
    /// Dispatches a request to the request handler registered for the given type. This is a non-generic
    /// method for dispatching requests without knowing the exact type.
    /// </summary>
    /// <param name="request">The request to dispatch</param>
    /// <returns>The reply returned by the request handler</returns>
    /// <exception cref="NoRequestHandlerException">No request handler was registered for the request type</exception>
    /// <exception cref="AggregateException">An exception occured within the request handler</exception>
    /// <exception cref="InvalidOperationException">An invalid request type was given, EG a non generic request object</exception>
    IEvent DispatchRequestUnsafe(IRequest request);
}