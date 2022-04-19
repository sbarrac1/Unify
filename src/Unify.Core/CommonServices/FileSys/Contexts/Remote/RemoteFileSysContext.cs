using Unify.Core.CommonServices.FileSys.Common;
using Unify.Core.CommonServices.Streams.Service;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.FileSys.Contexts.Remote;
public sealed class RemoteFileSysContext : IFileSysContext
{
    private readonly Guid _contextId;
    private readonly IEventTarget _eventTarget;
    private readonly IStreamsService _streamService;

    private readonly List<Stream> _ownedStreams = new();

    public RemoteFileSysContext(Guid contextId,
        IEventTarget eventTarget,
        IStreamsService streamService)
    {
        _contextId = contextId;
        _eventTarget = eventTarget;
        _streamService = streamService;
    }

    public IEnumerable<IFileSysEntry> GetSubEntries(FileSysDirectoryEntry fileSysDirectory)
    {
        var reply = _eventTarget.SendRequest(new FileSysGetEntriesRequest()
        {
            Directory = fileSysDirectory,
            ContextId = _contextId
        });

        List<IFileSysEntry> entries = new();

        if(reply.DirectoryEntries != null)
            entries.AddRange(reply.DirectoryEntries);
        
        if(reply.FileEntries != null)
            entries.AddRange(reply.FileEntries);

        return entries;
    }

    public Stream GetFileStream(FileSysFileEntry fileEntry)
    {
        var streamHandleInfo = _eventTarget.SendRequest(new FileSysGetFileStreamRequest()
        {
            File = fileEntry,
            ContextId = _contextId
        }).Header;

        var stream = _streamService.GetStream(streamHandleInfo);
        _ownedStreams.Add(stream);

        return stream;
    }

    public void Dispose()
    {
        _eventTarget.PostEvent(new FileSysCloseContextCommand
        {
            ContextId = _contextId,
        });

        foreach (var handle in _ownedStreams)
        {
            handle.Dispose();
        }
    }


    public IEnumerable<IFileSysEntry> GetRootEntries()
    {
        return GetSubEntries(new FileSysDirectoryEntry
        {
            Name = "",
            NodeId = Guid.Empty,
            Path = ""
        });
    }
}
