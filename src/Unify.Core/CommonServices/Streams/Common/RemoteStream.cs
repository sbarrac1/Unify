using Unify.Core.Events;

namespace Unify.Core.CommonServices.Streams.Common;

/// <summary>
/// Reads data from a stream hosted via <see cref="IEventTarget"/>
/// </summary>
public sealed class RemoteStream : Stream
{
    public override bool CanRead => _eventTarget.Connected && !_disposed;
    public override bool CanSeek => _eventTarget.Connected && !_disposed;
    public override bool CanWrite => false;
    public override long Length => _header.StreamLength;

    private long _position;
    public override long Position { get => _position; set => Seek(value, SeekOrigin.Begin); }

    private readonly IEventTarget _eventTarget;
    private readonly StreamHeader _header;
    private readonly object _lockObject = new();
    private bool _disposed;

    public RemoteStream(IEventTarget eventTarget, StreamHeader header)
    {
        _eventTarget = eventTarget;
        _header = header;   
    }


    public override int Read(byte[] buffer, int offset, int count)
    {
        lock (_lockObject)
        {
            if (_position == Length)
                return 0;

            var dataRead = _eventTarget.SendRequest(new StreamReadRequest
            {
                BytesToRead = count,
                StartPosition = _position,
                StreamId = _header.StreamId
            }).Data;

            _position += dataRead.LongLength;
            Buffer.BlockCopy(dataRead, 0, buffer, offset, dataRead.Length);
            return dataRead.Length;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        //Lock here to make sure we dont change _position while we are also waiting
        //for a reply, which would cause _position to be incorrect

        lock (_lockObject)
        {
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
            return _position = origin switch
            {
                SeekOrigin.Begin => offset, 
                SeekOrigin.Current => offset+ _position, 
                SeekOrigin.End => Length+offset
            };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
        }
    }

    protected override void Dispose(bool disposing)
    {
        lock (_lockObject)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_eventTarget.Connected)
            {
                _eventTarget.PostEvent(new StreamDisposeEvent
                {
                    StreamId = _header.StreamId
                });
            }
        }

        base.Dispose(disposing);
    }

    #region NotSupported
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    #endregion
}
