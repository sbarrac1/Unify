using System.Runtime.InteropServices;

namespace Unify.Windows.Shared.Input.Hooks;

public delegate bool LLKeyboardCallback(WindowMessage message, User32.KBDLLHOOKSTRUCT keyboardData);

public sealed class LLKeyboardHook : LLhookBase
{
    private readonly LLKeyboardCallback _callback;

    public LLKeyboardHook(IWindow context, LLKeyboardCallback callback) : base(context, 13)
    {
        _callback = callback;
    }

    private readonly User32.KBDLLHOOKSTRUCT _hookData = new();
    protected override nint OnHookEvent(int nCode, nint wParam, nint lParam)
    {
        WindowMessage message = (WindowMessage)wParam;
        Marshal.PtrToStructure<User32.KBDLLHOOKSTRUCT>(lParam, _hookData);

        return _callback(message, _hookData) ? User32.CallNextHookEx(0, nCode, wParam, lParam) : -1;
    }
}