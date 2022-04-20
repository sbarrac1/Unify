using Unify.Core.Events;
using Unify.Core.Net.Handshake.Server;
using Unify.Core.Net.IO;

namespace Unify.Core.Net.Handshake;

public sealed class ServerHandshakeRunner : IServerHandshakeRunner
{
    public async Task<IUnknownClient> DoHandshakeAsync(IEventStream eventStream, CancellationToken ct = default)
    {
        var next = await eventStream.ReadEventAsync(ct);

        if (next is ClientHandshakeEvent clientHandshakeEvent)
            return new UnknownClient(eventStream, clientHandshakeEvent.Info);

        throw new IOException($"Client sent invalid event {next}");
    }
}