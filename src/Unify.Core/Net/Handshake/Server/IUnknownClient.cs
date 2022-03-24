using Unify.Core.Net.IO;

namespace Unify.Core.Net.Handshake.Server;

/// <summary>
/// Represents a client that has completed
/// a handshake and provided a <see cref="ClientInfo"/> object
/// </summary>
public interface IUnknownClient
{
    ClientInfo Info { get; }
    
    IEventStream AcceptClient();
    void DeclineClient(string reason);
}