using Unify.Core.Events.Target.Exceptions;

namespace Unify.Core.Events.Target;

/// <summary>
/// Represents a target that can process events and requests
/// </summary>
public interface IEventTarget
{
    bool Connected { get; }

    /// <summary>
    /// Queues an event to be sent to the target. This method will never
    /// throw and always returns immediately.
    /// </summary>
    /// <param name="event"></param>
    void PostEvent(IEvent @event);

    /// <summary>
    /// Sends a request to the target. This method will block until either the
    /// target returns are reply or the request times out.
    /// </summary>
    /// <param name="request">The request to send</param>
    /// <typeparam name="TReply">The expected type of reply</typeparam>
    /// <param name="timeoutMs">The amount of time in milliseconds to wait before throwing a TimeoutException</param>
    /// <returns>The reply returned by the target</returns>
    /// <exception cref="IOException">Thrown if the event target is not available</exception>
    /// <exception cref="RequestFailedException">Thrown if the target failed to process the request</exception>
    /// <exception cref="TimeoutException">Thrown if a reply is not received within <see cref="timeoutMs"/> milliseconds</exception>
    TReply SendRequest<TReply>(IRequest<TReply> request)
        where TReply : IEvent;
}
