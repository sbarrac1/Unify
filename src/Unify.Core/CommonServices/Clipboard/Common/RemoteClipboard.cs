using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.DataMarshal.Data;
using Unify.Core.CommonServices.DataMarshal.Service;
using Unify.Core.CommonServices.FileSys.Contexts;
using Unify.Core.CommonServices.FileSys.Service;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.Clipboard.Common;

/// <summary>
/// A remote clipboard source
/// </summary>
public sealed class RemoteClipboard : IClipboard
{
    private readonly IEventTarget _eventTarget;
    private readonly IMarshalService _marshalService;
    private readonly IFileSysService _fileSysService;

    public RemoteClipboard(IEventTarget eventTarget,
        IMarshalService marshalService,
        IFileSysService fileSysService)
    {
        _eventTarget = eventTarget;
        _marshalService = marshalService;
        _fileSysService = fileSysService;
    }

    public ClipboardFormats GetFormats()
    {
        return _eventTarget.SendRequest(new ClipboardGetFormatsRequest()).Formats;
    }

    public IDataContainer<string> GetText()
    {
        var stringStreamHeader = _eventTarget.SendRequest(new ClipboardGetTextRequest()).Header;

        return _marshalService.GetContainer<string>(stringStreamHeader);
    }

    public IFileSysContext GetFiles()
    {
        var contextId = _eventTarget.SendRequest(new ClipboardGetFilesRequest()).FileSysContextId;

        return _fileSysService.GetContext(contextId);
    }
}
