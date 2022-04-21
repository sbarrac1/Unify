using Unify.Core.CommonServices.InputHook.Controller;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.InputHook.Host;

public sealed class UnregisterHotkeyCommandHandler : IEventHandler<UnregisterHotkeyCommand>
{
    private readonly IInputHookController _inputHookController;

    public UnregisterHotkeyCommandHandler(IInputHookController inputHookController)
    {
        _inputHookController = inputHookController;
    }

    public void Handle(UnregisterHotkeyCommand evt)
    {
        _inputHookController.UnregisterHotkey(evt.Hotkey);
    }
}
