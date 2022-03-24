using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Unify.Windows.Shared.Clipboard.Interop;

/// <summary>
/// Wraps a <see cref="Stream"/> into an <see cref="IStream"/>
/// </summary>
public sealed class ManagedIStream : Component, IStream
{
    private readonly Stream _baseStream;

    public ManagedIStream(Stream baseStream)
    {
        _baseStream = baseStream;
    }

    public void Clone(out IStream ppstm)
    {
        throw new NotImplementedException();
    }

    public void Commit(int grfCommitFlags)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
    {
        throw new NotImplementedException();
    }

    public void LockRegion(long libOffset, long cb, int dwLockType)
    {
        throw new NotImplementedException();
    }

    public void Read(byte[] pv, int cb, IntPtr pcbRead)
    {
        int bIn = _baseStream.Read(pv, 0, cb);

        Marshal.WriteIntPtr(pcbRead, (IntPtr)bIn);
    }

    public void Revert()
    {
        throw new NotImplementedException();
    }

    public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
    {
        SeekOrigin origin = dwOrigin switch
        {
            0 => SeekOrigin.Begin,
            1 => SeekOrigin.Current,
            2 => SeekOrigin.End,
            _ => throw new ArgumentException(),
        };

        _baseStream.Seek(dlibMove, origin);
        Marshal.WriteIntPtr(plibNewPosition, (IntPtr)_baseStream.Position);
    }

    public void SetSize(long libNewSize)
    {
        throw new NotImplementedException();
    }

    public void Stat(out STATSTG pstatstg, int grfStatFlag)
    {
        throw new NotImplementedException();
    }

    public void UnlockRegion(long libOffset, long cb, int dwLockType)
    {
        throw new NotImplementedException();
    }

    public void Write(byte[] pv, int cb, IntPtr pcbWritten)
    {
        throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
        _baseStream.Dispose();
    }
}
