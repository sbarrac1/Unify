using System.Runtime.InteropServices;

namespace Unify.Windows.Shared.Native;

public static class Kernel32
{
    private const string _dll = "Kernel32.dll";

    [DllImport(_dll)]
    public static extern uint GetCurrentThreadId();


    [DllImport(_dll)]
    public static extern nint GlobalSize(nint hMem);

    [DllImport(_dll)]
    public static extern nint GlobalLock(nint hMem);

    [DllImport(_dll)]
    public static extern bool GlobalUnlock(nint hMem);

    [DllImport(_dll)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);
}