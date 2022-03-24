using ProtoBuf;
using Unify.Core.CommonServices.FileSys.Common;
using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.Events;

[ProtoContract]
public sealed class FileSysGetEntriesRequest : IRequest<FileSysGetEntriesReply>
{
    [ProtoMember(1)]
    public Guid ContextId { get; init; }
    
    [ProtoMember(2)]
    public FileSysDirectoryEntry Directory { get; init; }
}


[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class FileSysGetEntriesReply : IEvent
{
    public IFileSysEntry[] Entries { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class FileSysGetFileStreamRequest : IRequest<FileSysGetFileStreamReply>
{
    public Guid ContextId { get; init; }
    public FileSysFileEntry File { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class FileSysGetFileStreamReply : IEvent
{
    public StreamHeader Header { get; init; }
}
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)] 
public sealed class FileSysCloseContextCommand : IEvent
{
    public Guid ContextId { get; init; }
}