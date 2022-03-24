using ProtoBuf;
using Unify.Core.Net.Handshake;

namespace Unify.Core.Events;

[ProtoContract]
public sealed class RequestFailedEvent : IEvent
{
    [ProtoMember(1)]
    public string Reason { get; init; }
}

[ProtoContract]
public sealed class ClientHandshakeEvent : IEvent
{
    [ProtoMember(1)]
    public ClientInfo Info { get; init; }
}

[ProtoContract]
public sealed class ServerAcceptedHandshakeEvent : IEvent
{
    
}

[ProtoContract]
public sealed class ServerDeclineHandshakeEvent : IEvent
{
    [ProtoMember(1)]
    public string Reason { get; init; }
}