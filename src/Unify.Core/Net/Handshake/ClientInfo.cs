using ProtoBuf;

namespace Unify.Core.Net.Handshake;

/// <summary>
/// Information that is sent by that client
/// during a handshake
/// </summary>
[ProtoContract]
public class ClientInfo
{
    [ProtoMember(1)]
    public string StationName { get; set; }
}
