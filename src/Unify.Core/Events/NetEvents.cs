using ProtoBuf;
using Unify.Core.Net.Formatting;
using Unify.Core.Net.Handshake;

namespace Unify.Core.Events;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(500)]
public sealed class RequestFailedEvent : IEvent
{
    public Guid EventId { get; set; }
    public string Reason { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(501)]
public sealed class ClientHandshakeEvent : IEvent
{
    public Guid EventId { get; set; }
    public ClientInfo Info { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(502)]
public sealed class ServerAcceptedHandshakeEvent : IEvent
{
    public Guid EventId { get; set; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(503)]
public sealed class ServerDeclineHandshakeEvent : IEvent
{
    public Guid EventId { get; set; }

    public string Reason { get; init; }
}