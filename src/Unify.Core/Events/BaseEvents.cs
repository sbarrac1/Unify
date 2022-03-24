using ProtoBuf;

namespace Unify.Core.Events;

/// <summary>
/// Marker class for events
/// </summary>
[ProtoContract]
[ProtoInclude(1, typeof(IRequest))]
[ProtoInclude(2, typeof(RequestFailedEvent))]
[ProtoInclude(3, typeof(ClipboardChangedEvent))]
[ProtoInclude(4, typeof(ClipboardGetFormatsReply))]
[ProtoInclude(5, typeof(ClipboardGetTextReply))]
[ProtoInclude(6, typeof(ClipboardTakeOwnershipCommand))]
[ProtoInclude(7, typeof(ClientHandshakeEvent))]
[ProtoInclude(8, typeof(ServerAcceptedHandshakeEvent))]
[ProtoInclude(9, typeof(ServerDeclineHandshakeEvent))]
[ProtoInclude(10, typeof(StreamDisposeEvent))]
[ProtoInclude(11, typeof(StreamReadReply))]
[ProtoInclude(12, typeof(FileSysGetEntriesReply))]
[ProtoInclude(13, typeof(FileSysGetFileStreamReply))]
[ProtoInclude(14, typeof(FileSysCloseContextCommand))]
[ProtoInclude(15, typeof(ClipboardGetFilesReply))]
[ProtoInclude(16, typeof(SendInputCommand))]
[ProtoInclude(17, typeof(SideHitEvent))]

public interface IEvent
{

}

/// <summary>
/// Marker class for request events. Inherits from IEvent
/// </summary>
[ProtoContract]
[ProtoInclude(1, typeof(ClipboardGetFormatsRequest))]
[ProtoInclude(2, typeof(ClipboardGetTextRequest))]
[ProtoInclude(3, typeof(StreamReadRequest))]
[ProtoInclude(4, typeof(FileSysGetEntriesRequest))]
[ProtoInclude(5, typeof(FileSysGetFileStreamRequest))]
[ProtoInclude(6, typeof(ClipboardGetFilesRequest))]
public interface IRequest : IEvent
{

}

/// <summary>
/// Generic marker class for requests that require
/// a specific return type
/// </summary>
/// <typeparam name="TReply">The return type of the request</typeparam>
public interface IRequest<TReply> : IRequest
{

}

/// <summary>
/// Wraps an event and an event ID into one object
/// </summary>
[ProtoContract]
public struct EventWrapper
{
    [ProtoMember(1)]
    public Guid EventId { get; init; }

    [ProtoMember(2)]
    public IEvent Event { get; init; }
}