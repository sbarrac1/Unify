using ProtoBuf;

namespace Unify.Core.Events;

//Netevents - 100-199
//ClipboardEvents - 200-299
//FileSysEvents - 300-399
//InputEvents - 400-499
//NetEvents - 500-599
//StreamEvents - 600-699
//IInput objects - 700-799

/// <summary>
/// Marker class for events
/// </summary>
[ProtoContract]
[ProtoInclude(5, typeof(IRequest))]
[ProtoInclude(6, typeof(RequestFailedEvent))]
[ProtoInclude(7, typeof(ClipboardChangedEvent))]
[ProtoInclude(8, typeof(ClipboardGetFormatsReply))]
[ProtoInclude(9, typeof(ClipboardGetTextReply))]
[ProtoInclude(10, typeof(ClipboardTakeOwnershipCommand))]
[ProtoInclude(11, typeof(ClientHandshakeEvent))]
[ProtoInclude(12, typeof(ServerAcceptedHandshakeEvent))]
[ProtoInclude(13, typeof(ServerDeclineHandshakeEvent))]
[ProtoInclude(14, typeof(StreamDisposeEvent))]
[ProtoInclude(15, typeof(StreamReadReply))]
[ProtoInclude(16, typeof(FileSysGetEntriesReply))]
[ProtoInclude(17, typeof(FileSysGetFileStreamReply))]
[ProtoInclude(18, typeof(FileSysCloseContextCommand))]
[ProtoInclude(19, typeof(ClipboardGetFilesReply))]
[ProtoInclude(20, typeof(SendInputCommand))]
[ProtoInclude(21, typeof(SideHitEvent))]
public interface IEvent
{

    Guid EventId { get; set; }
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