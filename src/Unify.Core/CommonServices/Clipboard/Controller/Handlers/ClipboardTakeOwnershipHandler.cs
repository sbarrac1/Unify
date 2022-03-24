using Unify.Core.Events;

namespace Unify.Core.CommonServices.Clipboard.Controller.Handlers;

public sealed class ClipboardTakeOwnershipHandler : IEventHandler<ClipboardTakeOwnershipCommand>
{
    private readonly IStationClipboardController _clipboardController;

    public ClipboardTakeOwnershipHandler(IStationClipboardController clipboardController)
    {
        _clipboardController = clipboardController;
    }
    
    public void Handle(ClipboardTakeOwnershipCommand evt)
    {
        _clipboardController.TakeOwnership();
    }
}