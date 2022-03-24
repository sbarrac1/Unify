using System.ComponentModel;

namespace Unify.Windows.Shared.Input.Hooks;

/// <summary>
/// Manages a low level keyboard or mouse hook
/// </summary>
public abstract class LLhookBase : IDisposable
{
    private readonly IWindow _context;
    private readonly nint _hHook;
    
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly User32.HookCallback _hookCallback;


    public LLhookBase(IWindow context, int hookType)
    {
        _context = context;
        _hookCallback = OnHookEvent;

        _hHook = context.WindowDispatcher.InvokeReturn(() =>
        {
            //uncomment next line to disable low level hooks
            //return 0;
            nint value = User32.SetWindowsHookEx(hookType, _hookCallback, Win32Helpers.GetHModule(), 0);

            return value == 0 ? throw new Win32Exception() : value; ;
        });
    }

    protected abstract nint OnHookEvent(int nCode, nint wParam, nint lParam);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.WindowDispatcher.InvokePost(() =>
            {
                User32.UnhookWindowsHookEx(_hHook);
            });
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}