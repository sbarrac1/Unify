using ProtoBuf;
using Unify.Core.CommonServices.FileSys.Common;
using Unify.Core.CommonServices.Streams.Common;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Events;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(300)]
public sealed class FileSysGetEntriesRequest : IRequest<FileSysGetEntriesReply>
{
    public Guid EventId { get; set; }
    public Guid ContextId { get; init; }
    public FileSysDirectoryEntry Directory { get; init; }
}


[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(301)]
public sealed class FileSysGetEntriesReply : IEvent
{
    public Guid EventId { get; set; }
    public IFileSysEntry[] Entries { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(302)]
public class FileSysGetFileStreamRequest : IRequest<FileSysGetFileStreamReply>
{
    public Guid EventId { get; set; }
    public Guid ContextId { get; init; }
    public FileSysFileEntry File { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(303)]
public sealed class FileSysGetFileStreamReply : IEvent
{
    public Guid EventId { get; set; }
    public StreamHeader Header { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(304)]
public sealed class FileSysCloseContextCommand : IEvent
{
    public Guid EventId { get; set; }
    public Guid ContextId { get; init; }
}