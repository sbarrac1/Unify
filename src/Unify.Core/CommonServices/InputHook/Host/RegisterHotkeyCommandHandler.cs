using Unify.Core.CommonServices.InputHook.Controller;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.InputHook.Host;

public sealed class RegisterHotkeyCommandHandler : IEventHandler<RegisterHotkeyCommand>
{
    private readonly IInputHookController _inputHookController;

    public RegisterHotkeyCommandHandler(IInputHookController inputHookController)
    {
        _inputHookController = inputHookController;
    }

    public void Handle(RegisterHotkeyCommand evt)
    {
        _inputHookController.RegisterHotkey(evt.Hotkey);
    }
}
