using System.Diagnostics;
using Unify.Core.CommonServices.FileSys.Common;

namespace Unify.Core.CommonServices.FileSys.Contexts.Local;
public sealed class LocalFileSysContext : IFileSysContext
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private List<Stream> _ownedStreams = new();
    private readonly object _lock = new object();
    private bool _disposed;

    private readonly Dictionary<Guid, FileSysRootDirectory> _rootDirs = new();
    private readonly Dictionary<Guid, FileSysRootFile> _rootFiles = new();

    public LocalFileSysContext(string[] rootPaths)
    {
        foreach (var path in rootPaths)
        {
            var fi = new FileInfo(path);
            Guid nodeId = Guid.NewGuid();

            if (fi.Attributes.HasFlag(FileAttributes.Directory))
            {
                var dir = new FileSysRootDirectory(fi.FullName, nodeId);

                _rootDirs[nodeId] = dir;
            }
            else
            {
                var file = new FileSysRootFile(fi.FullName, nodeId);

                _rootFiles[nodeId] = file;
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var handle in _ownedStreams)
            {
                handle.Dispose();
            }

            _ownedStreams.Clear();

            if (_logger.IsTraceEnabled)
                _logger.Trace($"Dispose local file system");
        }
    }

    public IEnumerable<IFileSysEntry> GetSubEntries(FileSysDirectoryEntry directoryEntry)
    {
        try
        {
            if (_logger.IsTraceEnabled)
                _logger.Trace($"Get sub entries {directoryEntry.Path}");

            if (directoryEntry.Name == "")
                return GetRootEntries();

            if (!_rootDirs.TryGetValue(directoryEntry.NodeId, out var rootDirEntry))
                throw new ArgumentException($"Invalid root node");

            if (directoryEntry.Name.Contains("Fira"))
                Debugger.Break();

            string path = Directory.GetParent(rootDirEntry.ActualPath) + "\\" + directoryEntry.Path;

            List<IFileSysEntry> entries = new();

            foreach (var entry in Directory.GetFileSystemEntries(path))
            {
                FileInfo fi = new FileInfo(entry);

                if (fi.Attributes.HasFlag(FileAttributes.Directory))
                {
                    entries.Add(new FileSysDirectoryEntry()
                    {
                        Name = fi.Name,
                        Path = directoryEntry.Path + "\\" + fi.Name,
                        NodeId = rootDirEntry.NodeId
                    });
                }
                else
                {
                    entries.Add(new FileSysFileEntry()
                    {
                        Name = fi.Name,
                        Path = directoryEntry.Path + "\\" + fi.Name,
                        Length = fi.Length,
                        NodeId = rootDirEntry.NodeId
                    });
                }
            }

            return entries;

        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get sub directories of {directoryEntry}: {ex.ToString()}");

            return new List<IFileSysEntry>();
        }

    }

    public IEnumerable<IFileSysEntry> GetRootEntries()
    {
        List<IFileSysEntry> entries = new();

        foreach (var rootDir in _rootDirs)
        {
            FileInfo fi = new FileInfo(rootDir.Value.ActualPath);

            entries.Add(new FileSysDirectoryEntry
            {
                Name = fi.Name,
                Path = fi.Name,
                NodeId = rootDir.Key
            });
        }

        foreach (var rootFile in _rootFiles)
        {
            FileInfo fi = new FileInfo(rootFile.Value.ActualPath);

            entries.Add(new FileSysFileEntry
            {
                Name = fi.Name,
                Path = fi.Name,
                NodeId = rootFile.Key,
                Length = fi.Length
            });
        }

        return entries;
    }

    public Stream GetFileStream(FileSysFileEntry fileEntry)
    {
        lock (_lock)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LocalFileSysContext));

            _logger.Trace($"Local file system -> Get files stream {fileEntry.Path}");

            if (_rootFiles.TryGetValue(fileEntry.NodeId, out var rootFileEntry))
            {
                var fileStream = File.OpenRead(rootFileEntry.ActualPath);
                _ownedStreams.Add(fileStream);

                return fileStream;
            }
            else
            {
                if (!_rootDirs.TryGetValue(fileEntry.NodeId, out var rootDirectory))
                    throw new ArgumentException($"Invalid root node");

                string actualPath = Directory.GetParent(rootDirectory.ActualPath) + "\\" + fileEntry.Path;

                var fileStream = File.OpenRead(actualPath);
                _ownedStreams.Add(fileStream);

                return fileStream;
            }
        }
    }
}
