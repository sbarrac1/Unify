namespace Unify.Core.Events.Dispatch.Resolve;

internal interface IEventHandlerResolver
{
    /// <summary>
    /// Attempts to create a delegate that calls any event handlers
    /// that are registered for the given event type
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="caller">If this method returns true, the returned delegate will</param>
    /// <returns>True if at least 1 event handler was found, false if no event handlers were found</returns>
    bool TryResolve(Type eventType, out EventHandlerCaller caller);
}