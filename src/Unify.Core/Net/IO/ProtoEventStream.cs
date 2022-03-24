using System.Buffers.Binary;
using Nerdbank.Streams;
using ProtoBuf;
using Unify.Core.Events;

namespace Unify.Core.Net.IO;

/// <summary>
/// IEventStream implementation using protobuf-net for serialization
/// </summary>
public sealed class ProtoEventStream : IEventStream
{
    private readonly Stream _stream;

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
    }

    public void WriteEvent(EventWrapper wrapper)
    {
        Serializer.SerializeWithLengthPrefix(_stream, wrapper, PrefixStyle.Fixed32);
    }

    public EventWrapper ReadEvent()
    {
        try
        {
            var next = Serializer.DeserializeWithLengthPrefix<EventWrapper>(_stream, PrefixStyle.Fixed32);

            if (next.Event == null)
                throw new IOException("End of stream");

            return next;
        }
        catch (NullReferenceException)
        {
            throw new IOException("Stream closed");
        }
    }

    public Task WriteEventAsync(EventWrapper wrapper, CancellationToken ct = default)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.SerializeWithLengthPrefix(ms, wrapper, PrefixStyle.Fixed32);
            ms.Position = 0;
            return ms.CopyToAsync(_stream, ct);
        }
    }

    public async Task<EventWrapper> ReadEventAsync(CancellationToken ct = default)
    {
        byte[] prefixBuffer = new byte[4];
        await _stream.ReadExactAsync(prefixBuffer, 4, ct);
        int prefix = BinaryPrimitives.ReadInt32LittleEndian(prefixBuffer);

        if (prefix > 300 * 1024)
            throw new IOException("Invalid prefix size");
            
        byte[] buffer = new byte[prefix];
        await _stream.ReadExactAsync(buffer, prefix, ct);

        using (MemoryStream ms = new MemoryStream(buffer))
        {
            ms.Position = 0;
            var next = Serializer.Deserialize<EventWrapper>(ms, length: prefix);

            if (next.Event == null)
                throw new IOException("End of stream");

            return next;
        }
    }
    
    public void Dispose()
    {
        _stream.Dispose();
    }
}