namespace Unify.Server.Common;

/// <summary>
/// The shared server state
/// </summary>
public class ServerContext
{
    /// <summary>
    /// When cancellated, the server will be stopped
    /// </summary>
    public CancellationTokenSource ServerShutdownCts { get; set; }


    public object SyncObject { get; set; }

    /// <summary>
    /// If true, the server is currently stopping
    /// </summary>
    public bool Stopping => ServerShutdownCts.IsCancellationRequested;

    public ServerContext()
    {
        SyncObject = new();
        ServerShutdownCts = new CancellationTokenSource();
    }
}
