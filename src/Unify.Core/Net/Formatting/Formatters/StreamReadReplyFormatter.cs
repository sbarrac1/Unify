using System.Buffers;
using Unify.Core.Events;

namespace Unify.Core.Net.Formatting.Formatters;
public sealed class StreamReadReplyFormatter : IFormatter<StreamReadReply>
{
    public StreamReadReply Read(Stream stream, ObjectPrefix prefix)
    {
        Span<byte> idBuffer = stackalloc byte[16];
        stream.ReadExact(idBuffer, 16);

        Guid eventId = new Guid(idBuffer);

        int bIn = prefix.ObjectLength - 16;

        IMemoryOwner<byte> memory = MemoryPool<byte>.Shared.Rent(prefix.ObjectLength);

        try
        {
            stream.ReadExact(memory.Memory.Span, bIn);
        }
        catch(Exception)
        {
            memory.Dispose();
            throw;
        }

        return new StreamReadReply
        {
            Memory = memory,
            BIn = bIn,
            EventId = eventId
        };
    }

    public void Write(Stream stream, StreamReadReply value)
    {
        ObjectManager.Instance.WritePrefixForType<StreamReadReply>(stream, value.BIn + 16);

        Span<byte> idBuffer = stackalloc byte[16];
        value.EventId.TryWriteBytes(idBuffer);

        stream.Write(idBuffer);

        stream.Write(value.Memory.Memory.Span.Slice(0, value.BIn));
    }
}
