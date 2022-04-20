using System.Runtime.InteropServices;

namespace Unify.Windows.Shared.Input.Hooks;

public delegate bool LLMouseCallback(WindowMessage message, User32.MSLLHOOKSTRUCT mouseData);

public sealed class LLMouseHook : LLhookBase
{
    private readonly LLMouseCallback _callback;

    public LLMouseHook(IWindow context, LLMouseCallback callback) : base(context, 14)
    {
        _callback = callback;
    }

    private readonly User32.MSLLHOOKSTRUCT _hookData = new();
    protected override nint OnHookEvent(int nCode, nint wParam, nint lParam)
    {
        WindowMessage message = (WindowMessage)wParam;
        Marshal.PtrToStructure<User32.MSLLHOOKSTRUCT>(lParam, _hookData);

        return _callback(message, _hookData) ?  User32.CallNextHookEx(0, nCode, wParam, lParam) : -1;
    }
}