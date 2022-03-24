using ProtoBuf;
using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.Events;

/// <summary>
/// Send by station hosts when the OS clipboard is changed
/// </summary>
[ProtoContract]
public sealed class ClipboardChangedEvent : IEvent
{

}

[ProtoContract]
public sealed class ClipboardTakeOwnershipCommand : IEvent
{
    
}

[ProtoContract]
public sealed class ClipboardGetTextRequest : IRequest<ClipboardGetTextReply>
{

}

[ProtoContract]
public sealed class ClipboardGetTextReply : IEvent
{
    [ProtoMember(1)]
    public StreamHeader Header { get; init; }
}

[ProtoContract]
public sealed class ClipboardGetFormatsRequest : IRequest<ClipboardGetFormatsReply> { }

[ProtoContract]
public sealed class ClipboardGetFormatsReply : IEvent
{
    [ProtoMember(1)]
    public ClipboardFormats Formats { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class ClipboardGetFilesRequest : IRequest<ClipboardGetFilesReply>
{
    
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class ClipboardGetFilesReply : IEvent
{
    public Guid FileSysContextId { get; init; }
}