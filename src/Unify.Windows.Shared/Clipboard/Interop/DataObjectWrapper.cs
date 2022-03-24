using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Unify.Core.Common.Clipboard;

namespace Unify.Windows.Shared.Clipboard.Interop;

/// <summary>
/// Reads data from an IDataObject
/// </summary>
internal sealed class DataObjectWrapper : System.IDisposable
{
    private readonly IDataObject _dataObject;
    private readonly IWindow _context;
    private bool _disposed;

    public DataObjectWrapper(IDataObject dataObject, IWindow context)
    {
        _dataObject = dataObject;
        _context = context;
    }

    public string[] GetFileList()
    {
        ThrowIfNotWindowThread();

        FORMATETC nativeFormat = WinClipboardFormat.CreateEtc(WinClipboardFormat.CF_HDROP, TYMED.TYMED_HGLOBAL);
        _dataObject.GetData(ref nativeFormat, out var medium);

        byte[] data = Win32Helpers.CopyFromPointer(medium.unionmember);
        string str = Encoding.Unicode.GetString(data);

        //Remove the header
        str = str[10..];
        //Remove the double null terminator at the end of the string
        str = str[0..^2];
        //Split the string into seperate files
        var result = str.Split('\0');
        return result;
    }

    public bool IsUnifyObject()
    {
        ThrowIfNotWindowThread();

        var nativeFormat = WinClipboardFormat.CreateEtc(WinClipboardFormat.UnifyOwned, TYMED.TYMED_HGLOBAL);

        return _dataObject.QueryGetData(ref nativeFormat) == 0;
    }

    public string GetText()
    {
        ThrowIfNotWindowThread();

        FORMATETC nativeFormat = WinClipboardFormat.CreateEtc(WinClipboardFormat.CF_UNICODETEXT, TYMED.TYMED_HGLOBAL);
        _dataObject.GetData(ref nativeFormat, out var medium);

        byte[] data = Win32Helpers.CopyFromPointer(medium.unionmember);

        return Encoding.Unicode.GetString(data);
    }

    public ClipboardFormats GetSupportedFormats()
    {
        try
        {
            var enumerator = _dataObject.EnumFormatEtc(DATADIR.DATADIR_GET);

            ClipboardFormats actualFormats = 0;

            if (enumerator == null)
                return 0;

            FORMATETC[] formats = new FORMATETC[12];
            int[] fetched = new int[1];
            while (enumerator.Next(1, formats, fetched) == 0)
            {
                if (formats[0].cfFormat == WinClipboardFormat.CF_TEXT)
                    actualFormats |= ClipboardFormats.Text;
                if (formats[0].cfFormat == WinClipboardFormat.CF_UNICODETEXT)
                    actualFormats |= ClipboardFormats.Text;
                else if (formats[0].cfFormat == WinClipboardFormat.CF_HDROP)
                    actualFormats |= ClipboardFormats.FileSys;
            }

            return actualFormats;
        }
        catch (OutOfMemoryException)
        {
            //TODO - why does EnumFormatEtc throw out of memory

            return 0;
        }
    }

    private void ThrowIfNotWindowThread()
    {
        if (_context.InvokeRequired())
            throw new InvalidOperationException("Attempted to read clipboard from another thread");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
#pragma warning disable CA1416
            Marshal.ReleaseComObject(_dataObject);
#pragma warning restore CA1416
            _disposed = true;
        }
    }
}
