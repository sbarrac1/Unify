using System.Buffers.Binary;
using System.Net;
using Nerdbank.Streams;
using ProtoBuf;
using Unify.Core.Events;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Net.IO;

/// <summary>
/// IEventStream implementation using protobuf-net for serialization
/// </summary>
public sealed class ProtoEventStream : IEventStream
{
    private readonly Stream _stream;

    private readonly MemoryStream _writeBuffer = new();
    private readonly MemoryStream _readBuffer = new();

    /// <summary>
    /// Creates a pair of ProtoEventStreams that emulate two TCP connections
    /// </summary>
    /// <returns></returns>
    public static (IEventStream, IEventStream) CreateDuplexPair()
    {
        var (streamA, streamB) = FullDuplexStream.CreatePair();

        return (new ProtoEventStream(streamA), new ProtoEventStream(streamB));
    }

    public ProtoEventStream(Stream stream)
    {
        _stream = stream;

        ObjectManager.Instance.Setup();
    }

    public void WriteEvent(IEvent @event)
    {
        _writeBuffer.Position = 0;
        ObjectManager.Instance.GetWriter(@event.GetType())(_writeBuffer, @event);

        int objectLength = (int)_writeBuffer.Position;
        Span<byte> buffer = stackalloc byte[objectLength];
        _writeBuffer.Position = 0;
        _writeBuffer.Read(buffer);

        _stream.Write(buffer);
    }

    public IEvent ReadEvent()
    {
        try
        {
            var prefix = ObjectPrefix.Read(_stream);

            return (IEvent)ObjectManager.Instance.GetReader(prefix.ObjectId)(_stream, prefix);
        }
        catch (NullReferenceException)
        {
            throw new IOException("Stream closed");
        }
    }

    public ValueTask WriteEventAsync(IEvent @event, CancellationToken ct = default)
    {
        using(MemoryStream ms = new MemoryStream())
        {
            ObjectManager.Instance.GetWriter(@event.GetType())(ms, @event);

            int objectLength = (int)ms.Position;
            byte[] buffer = new byte[objectLength];
            ms.Position = 0;
            ms.Read(buffer);

            return _stream.WriteAsync(buffer, ct);
        }
    }

    public async ValueTask<IEvent> ReadEventAsync(CancellationToken ct = default)
    {
        var prefix = ObjectPrefix.Read(_stream);

        byte[] buffer = new byte[prefix.ObjectLength];

        await _stream.ReadExactAsync(buffer, prefix.ObjectLength, ct);

        using(var ms = new MemoryStream(buffer))
        {
            ms.Write(buffer);
            ms.Position = 0;
            return (IEvent)ObjectManager.Instance.GetReader(prefix.ObjectId)(ms, prefix);
        }
    }

    public void Dispose()
    {
        _stream.Dispose();
    }
}