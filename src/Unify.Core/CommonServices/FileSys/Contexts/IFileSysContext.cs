using Unify.Core.CommonServices.FileSys.Common;

namespace Unify.Core.CommonServices.FileSys.Contexts;

public interface IFileSysContext : IDisposable
{
    IEnumerable<IFileSysEntry> GetSubEntries(FileSysDirectoryEntry directory);
    Stream GetFileStream(FileSysFileEntry file);

    IEnumerable<IFileSysEntry> GetRootEntries();
}
