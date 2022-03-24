namespace Unify.Core.Events.Dispatch.Exceptions;

public sealed class NoRequestHandlerException : Exception
{
    public NoRequestHandlerException(Type requestType) : base($"No request handler registered for type {requestType}")
    {

    }
}