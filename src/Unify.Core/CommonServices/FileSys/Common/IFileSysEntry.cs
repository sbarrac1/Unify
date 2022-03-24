using ProtoBuf;

namespace Unify.Core.CommonServices.FileSys.Common;

[ProtoContract]
[ProtoInclude(1, typeof(FileSysDirectoryEntry))]
[ProtoInclude(2, typeof(FileSysFileEntry))]
public interface IFileSysEntry
{
    Guid NodeId { get; }

    string Name { get; }
    string Path { get; }
}
