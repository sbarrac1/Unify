namespace Unify.Core.Events.Handlers;

/// <summary>
/// Handles a request. Only one request handler can be registered for a given request type
/// </summary>
/// <typeparam name="TRequest">The type of request to handle</typeparam>
/// <typeparam name="TReply">The type of return value</typeparam>
public interface IRequestHandler<in TRequest, out TReply>
    where TRequest : IRequest<TReply>
    where TReply : IEvent
{
    TReply Handle(TRequest request);
}
