using Unify.Core.CommonServices.FileSys.Controller;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.Clipboard.Controller.Handlers;

public sealed class ClipboardGetFilesHandler :
    IRequestHandler<ClipboardGetFilesRequest, ClipboardGetFilesReply>
{
    private readonly IFileSysController _fileSysController;
    private readonly IClipboardController _clipboardController;

    public ClipboardGetFilesHandler(IFileSysController fileSysController, IClipboardController clipboardController)
    {
        _fileSysController = fileSysController;
        _clipboardController = clipboardController;
    }

    public ClipboardGetFilesReply Handle(ClipboardGetFilesRequest request)
    {
        var clipboard = _clipboardController.GetClipboard();

        var hostedContext = _fileSysController.HostContext(clipboard.GetFiles());

        return new ClipboardGetFilesReply
        {
            FileSysContextId = hostedContext.ContextId
        };
    }
}
