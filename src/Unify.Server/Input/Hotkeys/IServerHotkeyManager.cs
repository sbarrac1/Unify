using Unify.Core.Common.Input;

namespace Unify.Server.Input.Hotkeys;

public interface IServerHotkeyManager
{
    void RegisterHotkey(Hotkey hk, Action callback);

    void OnHotkeyPressed(Hotkey hk);
}