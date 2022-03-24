using ProtoBuf;

namespace Unify.Core.Events;

[ProtoContract]
public sealed class StreamReadRequest : IRequest<StreamReadReply>
{
    [ProtoMember(1)]
    public Guid StreamId { get; init; }
    [ProtoMember(2)]
    public int BytesToRead { get; init; }
    [ProtoMember(3)]
    public long StartPosition { get; init; }
}

[ProtoContract]
public sealed class StreamReadReply : IEvent
{
    //Todo - this is really inefficient
    [ProtoMember(1)]
    public byte[] Data { get; init; }
}

[ProtoContract]
public sealed class StreamDisposeEvent : IEvent
{
    [ProtoMember(1)]
    public Guid StreamId { get; init; }
}