using System.Runtime.InteropServices;
using Unify.Core.CommonServices.FileSys.Common;
using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Windows.Shared.Clipboard.Interop;


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct FILEDESCRIPTOR
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public uint dwFlags;
    public Guid clsid;
    public System.Drawing.Size sizel;
    public System.Drawing.Point pointl;
    public uint dwFileAttributes;
    public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
    public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
    public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
    public uint nFileSizeHigh;
    public uint nFileSizeLow;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string cFileName;
    
    
    private const int FD_WRITESTIME = 0x00000020;
    private const int FD_FILESIZE = 0x00000040;
    private const int FD_PROGRESSUI = 0x00004000;

    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private static void AddDirectory(List<FileSysFileEntry> entries, IFileSysContext context, FileSysDirectoryEntry currentDir, int nMaxEntries, ref int nEntries)
    {
        try
        {
            if (nEntries > nMaxEntries)
                return;
                    
            foreach (var entry in context.GetSubEntries(currentDir))
            {
                if (nEntries > nMaxEntries)
                    return;

                if (entry is FileSysFileEntry file)
                {
                    nEntries++;
                    entries.Add(file);
                }

                if (entry is FileSysDirectoryEntry dir)
                {
                    nEntries++;
                    AddDirectory(entries, context, dir, nMaxEntries, ref nEntries);
                }
            }
        }
        catch(Exception ex)
        {
            _logger.Error(ex, "Failed to add directory {rootDir.Path}");
        }
    }

    private static void AddRootEntries(List<FileSysFileEntry> entries, IFileSysContext context, int nMaxEntries,
        ref int nEntries)
    {
        foreach (var rootEntry in context.GetRootEntries())
        {
            if (nEntries > nMaxEntries)
                break;

            if(rootEntry is FileSysFileEntry file)
            {
                entries.Add(file);
                nEntries++;
            }
            else if(rootEntry is FileSysDirectoryEntry directoryEntry)
            {
                nEntries++;
                AddDirectory(entries, context, directoryEntry, nMaxEntries, ref nEntries);
            }
        }
    }

    internal static MemoryStream GenerateFileDescriptor(IFileSysContext fileSys, out FileSysFileEntry[] files, int maxEntries)
    {
        try
        {
            var fileEntries = new List<FileSysFileEntry>();

            int nMaxEntries = maxEntries;
            int nEntries = 0;

           AddRootEntries(fileEntries, fileSys, nMaxEntries, ref nEntries);
            
            if (nEntries >= nMaxEntries)
            {
                _logger.Warn($"Entries limited to {nEntries}");
            } 
            
            files = fileEntries.ToArray();

            MemoryStream fileDescriptorMemoryStream = new MemoryStream();

            fileDescriptorMemoryStream.Write(BitConverter.GetBytes(files.Count()), 0, sizeof(uint));

            FILEDESCRIPTOR fileDescriptor = new FILEDESCRIPTOR();
            foreach (var si in files)
            {
                fileDescriptor.cFileName = si.Path;
                long FileWriteTimeUtc = 0;
                fileDescriptor.ftLastWriteTime.dwHighDateTime = (int)(FileWriteTimeUtc >> 32);
                fileDescriptor.ftLastWriteTime.dwLowDateTime = (int)(FileWriteTimeUtc & 0xFFFFFFFF);
                fileDescriptor.nFileSizeHigh = (uint)(si.Length >> 32);
                fileDescriptor.nFileSizeLow = (uint)(si.Length & 0xFFFFFFFF);
                fileDescriptor.dwFlags = FD_WRITESTIME | FD_FILESIZE | FD_PROGRESSUI;

                int fileDescriptorSize = Marshal.SizeOf(fileDescriptor);
                IntPtr fileDescriptorPointer = Marshal.AllocHGlobal(fileDescriptorSize);
                Marshal.StructureToPtr(fileDescriptor, fileDescriptorPointer, true);
                byte[] fileDescriptorByteArray = new byte[fileDescriptorSize];
                Marshal.Copy(fileDescriptorPointer, fileDescriptorByteArray, 0, fileDescriptorSize);
                Marshal.FreeHGlobal(fileDescriptorPointer);
                fileDescriptorMemoryStream.Write(fileDescriptorByteArray, 0, fileDescriptorByteArray.Length);
            }
            return fileDescriptorMemoryStream;
        }
        catch (Exception ex)
        {
            Logger.Trace(ex,"Failed to generate file descriptor");

            throw;
        }
    }
}