namespace Unify.Core.Events.Target.Exceptions;

/// <summary>
/// Thrown when a request to an event target fails.
/// </summary>
public sealed class RequestFailedException : Exception
{
    public RequestFailedException()
    {

    }

    public RequestFailedException(string message) : base(message)
    {
    }

    public RequestFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}