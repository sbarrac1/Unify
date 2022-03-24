namespace Unify.Core.Events.Handlers;

/// <summary>
/// Handles an event. Multiple event handlers can be registered for a single event type
/// </summary>
/// <typeparam name="TEvent">The type of event to handle</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    void Handle(TEvent evt);
}
