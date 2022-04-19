using ProtoBuf;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Events;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(600)]
public sealed class StreamReadRequest : IRequest<StreamReadReply>
{
    public Guid EventId { get; set; }
    public Guid StreamId { get; init; }
    public int BytesToRead { get; init; }
    public long StartPosition { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(601)]
public sealed class StreamReadReply : IEvent
{
    public Guid EventId { get; set; }

    //Todo - this is really inefficient
    public byte[] Data { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(602)]
public sealed class StreamDisposeEvent : IEvent
{
    public Guid EventId { get; set; }

    public Guid StreamId { get; init; }
}