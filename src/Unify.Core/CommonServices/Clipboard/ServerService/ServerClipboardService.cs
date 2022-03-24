using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.Clipboard.Common;
using Unify.Core.CommonServices.DataMarshal.Service;
using Unify.Core.CommonServices.FileSys.Service;

namespace Unify.Core.CommonServices.Clipboard.ServerService;
public sealed class ServerClipboardService : IServerClipboardService
{
    private readonly IClipboard _clipboard;

    public ServerClipboardService(IEventTarget eventTarget, IMarshalService marshalService, IFileSysService fileSysService)
    {
        _clipboard = new RemoteClipboard(eventTarget, marshalService, fileSysService);
    }

    public IClipboard GetClipboard()
    {
        return _clipboard;
    }
}
