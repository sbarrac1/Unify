using Unify.Core.Events.Dispatch.Exceptions;
using Unify.Core.Events.Dispatch.Resolve;

namespace Unify.Core.Events.Dispatch;

/// <inheritdoc cref="IDispatcher"/>
public sealed class Dispatcher : IDispatcher
{
    private readonly Dictionary<Type, EventHandlerCaller> _eventHandlerCallers = new();
    private readonly Dictionary<Type, RequestHandlerCaller> _requestHandlerCallers = new();

    private readonly IEventHandlerResolver _eventHandlerResolver;
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    private bool _disposed;

    public Dispatcher(IComponentContext context)
    {
        _eventHandlerResolver = new EventHandlerResolver(context);
        _requestHandlerResolver = new RequestHandlerResolver(context);
    }

    public bool DispatchEvent(IEvent @event)
    {
        var caller = GetEventCaller(@event.GetType());

        if (caller == null)
            return false;

        caller(@event);
        return true;
    }

    public TReply DispatchRequest<TReply>(IRequest<TReply> request) where TReply : IEvent
    {
        var caller = GetRequestCaller(request.GetType());

        if (caller == null)
            throw new NoRequestHandlerException(request.GetType());

        return (TReply)caller(request);
    }

    public IEvent DispatchRequestUnsafe(IRequest request)
    {
        var caller = GetRequestCaller(request.GetType());

        if (caller == null)
            throw new NoRequestHandlerException(request.GetType());

        return caller(request);
    }

    public void Dispose()
    {
        lock (_eventHandlerCallers)
        {
            lock (_requestHandlerCallers)
            {
                _disposed = true;

                _eventHandlerCallers.Clear();
                _requestHandlerCallers.Clear();
            }
        }
    }

    private EventHandlerCaller GetEventCaller(Type eventType)
    {
        lock (_eventHandlerCallers)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Dispatcher));

            if (_eventHandlerCallers.TryGetValue(eventType, out var caller))
                return caller;

            if (_eventHandlerResolver.TryResolve(eventType, out caller))
            {
                _eventHandlerCallers.Add(eventType, caller!);
                return caller;
            }

            return null;
        }
    }

    private RequestHandlerCaller GetRequestCaller(Type requestType)
    {
        lock (_requestHandlerCallers)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Dispatcher));

            if (_requestHandlerCallers.TryGetValue(requestType, out var caller))
                return caller;

            if (_requestHandlerResolver.TryResolve(requestType, out caller))
            {
                _requestHandlerCallers.Add(requestType, caller!);
                return caller;
            }

            return null;
        }
    }
}