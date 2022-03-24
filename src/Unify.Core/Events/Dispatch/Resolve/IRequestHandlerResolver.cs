namespace Unify.Core.Events.Dispatch.Resolve;

internal interface IRequestHandlerResolver
{
    bool TryResolve(Type requestType, out RequestHandlerCaller caller);
}