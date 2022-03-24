using Unify.Core.Net.Handshake.Server;
using Unify.Core.Net.IO;

namespace Unify.Core.Net.Handshake;

public interface IServerHandshakeRunner
{
    Task<IUnknownClient> DoHandshakeAsync(IEventStream eventStream, CancellationToken ct = default);
}