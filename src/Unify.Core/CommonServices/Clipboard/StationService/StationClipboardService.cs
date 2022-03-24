using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.Clipboard.Common;
using Unify.Core.CommonServices.DataMarshal.Service;
using Unify.Core.CommonServices.FileSys.Service;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.Clipboard.StationService;
public sealed class StationClipboardService : IStationClipboardService
{
    private readonly IEventTarget _eventTarget;
    private readonly IMarshalService _marshalService;
    private readonly IFileSysService _fileSysService;

    public StationClipboardService(IEventTarget eventTarget,
        IMarshalService marshalService,
        IFileSysService fileSysService)
    {
        _eventTarget = eventTarget;
        _marshalService = marshalService;
        _fileSysService = fileSysService;
    }

    public IClipboard GetClipboard()
    {
        return new RemoteClipboard(_eventTarget, _marshalService, _fileSysService);
    }

    public void TakeOwnership()
    {
        _eventTarget.PostEvent(new ClipboardTakeOwnershipCommand());
    }
}
