namespace Unify.Core.Net.Processing;

/// <summary>
/// Processes incoming events, and manages outgoing requests
/// </summary>
public interface IEventProcessor : IEventTarget, IDisposable
{
    /// <summary>
    /// Start processing events in the background. If the underlying
    /// <see cref="IEventStream"/> fails, the <paramref name="onFault"/> 
    /// callback will be called. If the event processor is disposed, the
    /// callback will not be called
    /// </summary>
    /// <param name="onFault"></param>
    void BeginBackgroundWorker(Action<Exception> onFault);
}