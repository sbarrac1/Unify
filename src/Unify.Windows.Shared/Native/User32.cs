using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable All

namespace Unify.Windows.Shared.Native;

public static partial class User32
{
    private const string _dll = "user32.dll";

    public delegate IntPtr HookCallback(int nCode, nint wParam, nint lParam);
    [DllImport(_dll)]
    public static extern int GetClipboardFormatName(uint format, [Out] StringBuilder
        lpszFormatName, int cchMaxCount);

    [DllImport(_dll, SetLastError = true)]
    public static extern uint SendInput(uint nInputs, [In] WinInputStruct[] pInputs, int cbSize);

    [DllImport(_dll)]
    public static extern int GetSystemMetrics(int smIndex);

    [DllImport(_dll, SetLastError = true)]
    public static extern bool GetCursorPos(out POINT pos);

    [DllImport(_dll, SetLastError = true)]
    public static extern bool UnhookWindowsHookEx(nint hook);

    [DllImport(_dll, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern nint CallNextHookEx(nint hook, int nCode, nint wParam, nint lParam);

    [DllImport(_dll, SetLastError = true)]
    public static extern nint SetWindowsHookEx(int idHook,
        HookCallback lpfn, nint hMod, uint dwThreadId);

    [DllImport(_dll, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint  RegisterClipboardFormat(string format);

    [DllImport(_dll, SetLastError = true)]
    public static extern bool AddClipboardFormatListener(nint hWnd);
    
    [DllImport(_dll)]
    public static extern bool PostMessage(nint hwnd, WindowMessage msg, nint wparam, nint lparam);
    
    [DllImport(_dll)]
    public static extern int GetMessage([In, Out] ref MSG msg, nint hWnd, int uMsgFilterMin, int uMsgFilterMax);
    
    [DllImport(_dll, SetLastError=true)]
    public static extern IntPtr CreateWindowEx(int exStyle, [MarshalAs(UnmanagedType.LPStr)] string lpszClassName, string lpszWindowName, int style, int x, int y, int width,
        int height, nint hWndParent, nint hMenu, nint hInst, object pvParam);
    
    [DllImport(_dll)]
    public static extern IntPtr DefWindowProc(nint hWnd, WindowMessage msg, nint wParam, nint lParam);
    
    [DllImport(_dll)]
    public static extern bool DestroyWindow(nint hWnd);
    
    [DllImport(_dll)]
    public static extern int DispatchMessage([In] ref MSG msg);
    
    [DllImport(_dll)]
    public static extern short RegisterClass(WNDCLASS wc);
    
    [DllImport(_dll)]
    public static extern short UnregisterClass([MarshalAs(UnmanagedType.LPStr)] string lpClassName, nint hInstance);
}