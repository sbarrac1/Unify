using ProtoBuf;

namespace Unify.Core.CommonServices.Streams.Common;


/// <summary>
/// Contains information required to construct a <see cref="RemoteStream"/>
/// for a stream that is hosted via <see cref="IEventTarget"/>
/// </summary>
[ProtoContract]
public struct StreamHeader
{
    [ProtoMember(1)]
    public Guid StreamId { get; set; }
    [ProtoMember(2)]
    public long StreamLength { get; set; }
}
