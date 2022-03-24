namespace Unify.Core.CommonServices.FileSys.Contexts.Local;
public sealed class FileSysRootDirectory
{
    public FileSysRootDirectory(string actualPath, Guid nodeId)
    {
        ActualPath = actualPath;
        NodeId = nodeId;
    }

    public string ActualPath { get; }
    public Guid NodeId { get; }
}
