using ProtoBuf;

namespace Unify.Core.CommonServices.FileSys.Common;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class FileSysFileEntry : IFileSysEntry
{
    public string Name { get; init; }
    public string Path { get; init; }
    public long Length { get; init; }

    public Guid NodeId { get; init; }
}
