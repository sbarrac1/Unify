using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.FileSys.Common;
using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Windows.Shared.Clipboard.Interop;

/// <summary>
/// An IDataObject implementation that can be placed on the clipboard
/// </summary>
internal sealed class DataObjectProvider : IDataObject, IAsyncOperation, IDisposable
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IClipboard _source;
    private List<FileSysFileEntry> _fileCache;
    private byte[] _fileDescriptor;

    private IFileSysContext _cachedFileSysContext;

    public DataObjectProvider(IClipboard source)
    {
        _source = source;
    }

    public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
    {
        connection = -1;
        return 1;
    }

    public void DUnadvise(int connection)
    {

    }

    public int EnumDAdvise(out IEnumSTATDATA enumAdvise)
    {
        enumAdvise = null;
        return 1;
    }

    public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
    {
        var winFormats = GetFormats();

        Ole32.SHCreateStdEnumFmtEtc((uint)winFormats.Length, winFormats, out var enumerator);

        return enumerator;
    }

    public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
    {
        formatOut = default;
        return 1;
    }

    public void GetData(ref FORMATETC format, out STGMEDIUM medium)
    {
        medium = default;


        try
        {
            if(_logger.IsTraceEnabled)
                _logger.Trace($"GetData as {WinClipboardFormat.GetFormatName(format.cfFormat)} : {format.tymed}");

            if (format.cfFormat == WinClipboardFormat.UnifyOwned)
            {
                medium = default;
                medium.tymed = TYMED.TYMED_HGLOBAL;
                return;
            }
            else if (format.cfFormat == WinClipboardFormat.CF_UNICODETEXT && format.tymed == TYMED.TYMED_HGLOBAL)
            {
                GetData_Text(out medium);
                return;
            }
            else if (format.cfFormat == WinClipboardFormat.CFSTR_FILEDESCRIPTOR)
            {
                GetDataFileDescriptor(out medium);
                return;
            }
            else if (format.cfFormat == WinClipboardFormat.CFSTR_FILECONTENTS)
            {
                GetDataFileStream(ref format, out medium);
                return;
            }
            else if (format.cfFormat == WinClipboardFormat.PREFERREDDROPEFFECT)
            {
                GetDropEffect(out medium);
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"GetData as {WinClipboardFormat.GetFormatName(format.cfFormat)} failed");
        }
    }

    private void GetDataFileStream(ref FORMATETC format, out STGMEDIUM medium)
    {
        medium = default;

        if(_fileCache == null)
        {
            GetDataFileDescriptor(out _);
        }

        if (format.lindex == -1)
            return;

        var stream = _cachedFileSysContext.GetFileStream(_fileCache[format.lindex]);
        var s = new ManagedIStream(stream);

#pragma warning disable CA1416 // Validate platform compatibility
        var ptr = Marshal.GetComInterfaceForObject(s, typeof(IStream));
#pragma warning restore CA1416 // Validate platform compatibility


        medium.tymed = TYMED.TYMED_ISTREAM;
        medium.unionmember = ptr;
    }

    private static void GetDropEffect(out STGMEDIUM medium)
    {
        byte[] b = BitConverter.GetBytes(1);
        medium.pUnkForRelease = null;
        medium.tymed = TYMED.TYMED_HGLOBAL;
        medium.unionmember = Win32Helpers.CopyToHGlobal(b);
    }

    private void GetDataFileDescriptor(out STGMEDIUM medium)
    {
        medium = default;

        try
        {
            if (_fileCache == null)
            {
                _cachedFileSysContext = _source.GetFiles();
                var descriptor = FILEDESCRIPTOR.GenerateFileDescriptor(_cachedFileSysContext, out var files, 3000);
                _fileCache = new List<FileSysFileEntry>(files);

                _fileDescriptor = descriptor.ToArray();
            }

            nint ptr = Win32Helpers.CopyToHGlobal(_fileDescriptor);

            medium.tymed = TYMED.TYMED_HGLOBAL;
            medium.unionmember = ptr;
            medium.pUnkForRelease = null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Couldn't get file descriptor");
        }
    }

    public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
    {

    }

    public int QueryGetData(ref FORMATETC format)
    {
        if (format.cfFormat == WinClipboardFormat.UnifyOwned)
            return 0;

        return 1;
    }

    public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
    {
    }

    private FORMATETC[] GetFormats()
    {
        try
        {
            var formats = _source.GetFormats();
            List<FORMATETC> winFormats = new();

            winFormats.Add(WinClipboardFormat.CreateEtc(WinClipboardFormat.UnifyOwned, TYMED.TYMED_HGLOBAL));

            if (formats.HasFlag(ClipboardFormats.Text))
                winFormats.Add(WinClipboardFormat.CreateEtc(WinClipboardFormat.CF_UNICODETEXT, TYMED.TYMED_HGLOBAL));
            if (formats.HasFlag(ClipboardFormats.FileSys))
                winFormats.Add(WinClipboardFormat.CreateEtc(WinClipboardFormat.CFSTR_FILEDESCRIPTOR, TYMED.TYMED_HGLOBAL));
            if (formats.HasFlag(ClipboardFormats.FileSys))
                winFormats.Add(WinClipboardFormat.CreateEtc(WinClipboardFormat.CFSTR_FILECONTENTS,
                    TYMED.TYMED_ISTREAM));

            return winFormats.ToArray();
        }
        catch(Exception ex)
        {
            _logger.Error(ex,$"Failed to get clipboard formats");
            return Array.Empty<FORMATETC>();
        }
       
    }

    private void GetData_Text(out STGMEDIUM medium)
    {
        medium.tymed = TYMED.TYMED_HGLOBAL;
        using (var textHandle = _source.GetText())
        {
            medium.unionmember = Win32Helpers.CopyToHGlobal(Encoding.Unicode.GetBytes(textHandle.GetObject()));
            medium.pUnkForRelease = 0;
        }
    }


    private bool _inOperation;
    public void SetAsyncMode([In] int fDoOpAsync)
    {
    }

    public void GetAsyncMode([Out] out int pfIsOpAsync)
    {
        pfIsOpAsync = -1;
    }

    public void StartOperation([In] IBindCtx pbcReserved)
    {

        _inOperation = true;
    }

    public void InOperation([Out] out int pfInAsyncOp)
    {
        pfInAsyncOp = _inOperation ? 0 : -1;
    }

    public void EndOperation([In] int hResult, [In] IBindCtx pbcReserved, [In] uint dwEffects)
    {
        Dispose();
        _inOperation = false;
    }

    public void Dispose()
    {
        _cachedFileSysContext?.Dispose();
        _fileCache = null;
        _fileDescriptor = null;
    }
}