namespace Unify.Core.Events.Dispatch.Resolve;

internal sealed class RequestHandlerResolver : IRequestHandlerResolver
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IComponentContext _context;

    public RequestHandlerResolver(IComponentContext context)
    {
        _context = context;
    }

    public bool TryResolve(Type requestType, out RequestHandlerCaller caller)
    {
        if (!TryGetReturnType(requestType, out var replyType))
            throw new InvalidOperationException($"Invalid request type {requestType}");

        Type[] typeArgs = { requestType, replyType };
        caller = this.CallGenericMethod<RequestHandlerCaller>(nameof(InternalResolve), typeArgs);

        return caller != null;
    }

    private static bool TryGetReturnType(Type requestType, [MaybeNullWhen(false)] out Type replyType)
    {
        replyType = null;

        var baseInterface = requestType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        if (baseInterface == null)
            return false;

        replyType = baseInterface.GetGenericArguments()[0];

        return replyType.IsAssignableTo(typeof(IEvent));
    }

    private RequestHandlerCaller InternalResolve<TRequest, TReply>()
        where TRequest : IRequest<TReply>
        where TReply : IEvent
    {
        if (!_context.TryResolve<IRequestHandler<TRequest, TReply>>(out var handler))
        {
            _logger.Warn($"Resolved 0 request handlers for request type '{typeof(TRequest)}'");
            return null;
        }

        _logger.Trace($"Resolved request handler '{handler}' for request type '{typeof(TRequest)}'");

        return (request) =>
        {
            try
            {
                return handler.Handle((TRequest)request);
            }
            catch (Exception ex)
            {
                throw new AggregateException($"The request handler type '{handler.GetType()}' threw an exception", ex);
            }
        };
    }
}