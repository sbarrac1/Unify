using Unify.Core.Net.IO;

namespace Unify.Core.Net.Handshake;
public interface IClientHandshakeRunner
{
    /// <summary>
    /// Attempts to perform a handshake with the server
    /// </summary>
    /// <param name="eventStream">Server event stream</param>
    /// <param name="clientInfo"></param>
    /// <exception cref="IOException">The handshake failed</exception>
    /// <returns></returns>
    Task DoHandshakeAsync(IEventStream eventStream, ClientInfo clientInfo, CancellationToken ct);
}
