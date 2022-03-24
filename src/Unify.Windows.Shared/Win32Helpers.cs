using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Unify.Windows.Shared;

public static class Win32Helpers
{
    public static nint GetHInstance() => Process.GetCurrentProcess().Handle;
    public static nint GetHModule() => Kernel32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

    public static bool TryGetClipboardObject(int attempts, out IDataObject dataObject)
    {
        for (int i = 0; i < attempts; i++)
        {
            if (Ole32.OleGetClipboard(out dataObject) == 0)
                return true;

            Thread.Sleep(25);
        }

        dataObject = null;
        return false;
    }

    public static byte[] CopyFromPointer(nint ptr)
    {
        nint lockedPtr = Kernel32.GlobalLock(ptr);

        try
        {
            nint bufferSize = Kernel32.GlobalSize(lockedPtr);

            byte[] buffer = new byte[bufferSize];

            Marshal.Copy(lockedPtr, buffer, 0, buffer.Length);
            return buffer;
        }
        finally
        {
            Kernel32.GlobalUnlock(lockedPtr);
        }
    }

    public static nint CopyToHGlobal(byte[] data)
    {
        nint ptr = Marshal.AllocHGlobal(data.Length);

        if (ptr == 0)
            throw new Win32Exception();

        Marshal.Copy(data, 0, ptr, data.Length);
        return ptr;
    }
}