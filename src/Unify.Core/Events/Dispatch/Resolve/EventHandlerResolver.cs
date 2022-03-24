namespace Unify.Core.Events.Dispatch.Resolve;

internal sealed class EventHandlerResolver : IEventHandlerResolver
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IComponentContext _context;

    public EventHandlerResolver(IComponentContext context)
    {
        _context = context;
    }

    public bool TryResolve(Type eventType, out EventHandlerCaller caller)
    {
        caller = this.CallGenericMethod<EventHandlerCaller>(nameof(InternalResolve), new[] { eventType });

        return caller != null;
    }

    private EventHandlerCaller InternalResolve<TEvent>()
        where TEvent : IEvent
    {
        if (!_context.TryResolve<IEnumerable<IEventHandler<TEvent>>>(out var handlers))
        {
            _logger.Warn($"Resolved 0 event handlers for event type '{typeof(TEvent)}'");
            return null;
        }

        //context.tryresolve returns true even if no handlers are returned
        if (!handlers.Any())
        {
            _logger.Warn($"Resolved 0 event handlers for event type '{typeof(TEvent)}'");
            return null;
        }

        EventHandlerCaller caller = null;

        foreach (var handler in handlers)
        {
            _logger.Trace($"Resolved handler type '{handler.GetType()}' for event type '{typeof(TEvent)}'");

            caller += (@event) =>
            {
                try
                {
                    handler.Handle((TEvent)@event);
                }
                catch (Exception ex)
                {
                    throw new AggregateException($"The event handler type {handler.GetType()} threw an exception", ex);
                }
            };
        }

        return caller;
    }
}