using ProtoBuf;
using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.Streams.Common;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Events;

/// <summary>
/// Send by station hosts when the OS clipboard is changed
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(200)]
public sealed class ClipboardChangedEvent : IEvent
{
    public Guid EventId { get; set; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(201)]
public sealed class ClipboardTakeOwnershipCommand : IEvent
{
    public Guid EventId { get; set; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(202)]
public sealed class ClipboardGetTextRequest : IRequest<ClipboardGetTextReply>
{
    public Guid EventId { get; set; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(203)]
public sealed class ClipboardGetTextReply : IEvent
{
    public Guid EventId { get; set; }

    public StreamHeader Header { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(204)]
public sealed class ClipboardGetFormatsRequest : IRequest<ClipboardGetFormatsReply> 
{
    public Guid EventId { get; set; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(205)]
public sealed class ClipboardGetFormatsReply : IEvent
{
    public Guid EventId { get; set; }
    public ClipboardFormats Formats { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(206)]
public sealed class ClipboardGetFilesRequest : IRequest<ClipboardGetFilesReply>
{
    public Guid EventId { get; set; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(207)]
public sealed class ClipboardGetFilesReply : IEvent
{
    public Guid EventId { get; set; }

    public Guid FileSysContextId { get; init; }
}