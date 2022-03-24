using Unify.Core.Common.Input;

namespace Unify.Core.CommonServices.InputHook.Controller;
public interface IInputHookController
{
    void SetGrabState(bool grabState);

    void RegisterHotkey(Hotkey hk);
    void UnregisterHotkey(Hotkey hk);
}
