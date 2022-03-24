using Unify.Core.Common.Input;

namespace Unify.Windows.Shared.Input;

/// <summary>
/// Keeps a local keyboard state
/// </summary>
public sealed class KeyboardState
{
    private readonly bool[] _states = new Boolean[255];
    
    public bool IsPressed(WinVirtualkey vKey)
    {
        return _states[(int)vKey];
    }
    
    public bool IsPressedAny(params WinVirtualkey[] vKeys)
    {
        foreach (var vKey in vKeys)
        {
            if (IsPressed(vKey))
                return true;
        }

        return false;
    }

    public bool CheckModifiers(KeyModifiers modifiers)
    {
        if(modifiers.HasFlag(KeyModifiers.Alt))
            if (!IsPressedAny(WinVirtualkey.LeftMenu, WinVirtualkey.RightMenu, WinVirtualkey.Menu))
                return false;
        
        if(modifiers.HasFlag(KeyModifiers.Ctrl))
            if (!IsPressedAny(WinVirtualkey.LeftControl, WinVirtualkey.RightControl, WinVirtualkey.Control))
                return false;
        
        if(modifiers.HasFlag(KeyModifiers.Shift))
            if (!IsPressedAny(WinVirtualkey.LeftShift, WinVirtualkey.RightShift, WinVirtualkey.Shift))
                return false;
        
        if(modifiers.HasFlag(KeyModifiers.Win))
            if (!IsPressedAny(WinVirtualkey.LeftWindows, WinVirtualkey.RightWindows))
                return false;

        return true;
    }

    public void ProcessMessage(WindowMessage message, User32.KBDLLHOOKSTRUCT keyboardData)
    {
        if (message is WindowMessage.WM_KEYDOWN or WindowMessage.WM_SYSKEYDOWN)
        {
            _states[(int)keyboardData.vkCode] = true;
        }
        else
        {
            _states[(int)keyboardData.vkCode] = false;
        }
    }
}