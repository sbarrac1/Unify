using Unify.Core.Common.Input;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.InputHook.Service;

public sealed class InputHookService : IInputHookService
{
    public bool InputGrabbed { get; private set; }
    
    private readonly IEventTarget _eventTarget;

    public InputHookService(IEventTarget eventTarget)
    {
        _eventTarget = eventTarget;
    }

    public void RegisterHotkey(Hotkey hk)
    {
        _eventTarget.PostEvent(new RegisterHotkeyCommand
        {
            Hotkey = hk
        });
    }
    
    public void SetGrabState(bool state)
    {
        _eventTarget.PostEvent(new SetGrabStateCommand
        {
            State = state
        });

        InputGrabbed = state;
    }

    public void UnregisterHotkey(Hotkey hk)
    {
        _eventTarget.PostEvent(new UnregisterHotkeyCommand
        {
            Hotkey = hk
        });
    }
}
