using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Unify.Windows.Shared.Native;

public static class Ole32
{
    private const string dll = "ole32.dll";

    [DllImport("shell32.dll")]
    internal static extern IntPtr SHCreateStdEnumFmtEtc(uint format, FORMATETC[] formats, out IEnumFORMATETC etc);

    [DllImport("ole32.dll")]
    internal static extern nint OleGetClipboard([MarshalAs(UnmanagedType.Interface)] out IDataObject ppDataObj);

    [DllImport("ole32.dll")]
    internal static extern nint OleIsCurrentClipboard([MarshalAs(UnmanagedType.Interface)] IDataObject ppDataObj);

    [DllImport(dll, PreserveSig = false)]
    internal static extern void OleInitialize(nint pvReserved);

    [DllImport("ole32.dll", SetLastError = true, PreserveSig = false)]
    internal static extern void OleSetClipboard([In] IDataObject pDataObj);
}