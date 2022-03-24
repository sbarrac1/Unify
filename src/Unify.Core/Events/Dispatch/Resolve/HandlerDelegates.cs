namespace Unify.Core.Events.Dispatch.Resolve;

/// <summary>
/// Calls one or more event handlers
/// </summary>
internal delegate void EventHandlerCaller(IEvent evt);

/// <summary>
/// Calls a request handler
/// </summary>
internal delegate IEvent RequestHandlerCaller(IRequest request);
