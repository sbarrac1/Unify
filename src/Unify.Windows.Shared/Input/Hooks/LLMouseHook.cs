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

    protected override nint OnHookEvent(int nCode, nint wParam, nint lParam)
    {
        WindowMessage message = (WindowMessage)wParam;
        var mouseData = Marshal.PtrToStructure<User32.MSLLHOOKSTRUCT>(lParam);

        return _callback(message, mouseData) ?  User32.CallNextHookEx(0, nCode, wParam, lParam) : -1;
    }
}