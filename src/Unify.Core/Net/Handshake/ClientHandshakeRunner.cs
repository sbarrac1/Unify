using Unify.Core.Events;
using Unify.Core.Net.IO;

namespace Unify.Core.Net.Handshake;

public sealed class ClientHandshakeRunner : IClientHandshakeRunner
{
    public async Task DoHandshakeAsync(IEventStream eventStream, ClientInfo clientInfo, CancellationToken ct)
    {
        try
        {
            await eventStream.WriteEventAsync(new ClientHandshakeEvent()
            {
                Info = clientInfo
            });

            var reply = await eventStream.ReadEventAsync(ct);

            if (reply is ServerAcceptedHandshakeEvent)
                return;

            if (reply is ServerDeclineHandshakeEvent declineHandshakeEvent)
                throw new IOException($"The server declined the connection: {declineHandshakeEvent.Reason}");

            throw new IOException($"The server sent an invalid event {reply}");

        }
        catch(Exception ex)
        {
            throw new IOException("The handshake failed", ex);
        }
    }
}