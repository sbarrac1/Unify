using Unify.Core.CommonServices.FileSys.Contexts;
using Unify.Core.CommonServices.FileSys.Contexts.Remote;
using Unify.Core.CommonServices.Streams.Service;

namespace Unify.Core.CommonServices.FileSys.Service;
public sealed class FileSysService : IFileSysService
{
    private readonly IEventTarget _eventTarget;
    private readonly IStreamsService _streamService;

    public FileSysService(IEventTarget eventTarget, IStreamsService streamService)
    {
        _eventTarget = eventTarget;
        _streamService = streamService;
    }

    public IFileSysContext GetContext(Guid contextId)
    {
        return new RemoteFileSysContext(contextId, _eventTarget, _streamService);
    }
}
