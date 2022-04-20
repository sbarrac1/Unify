using System.Buffers;
using Unify.Core.Events;

namespace Unify.Core.Net.Formatting.Formatters;
public sealed class StreamReadReplyFormatter : IFormatter<StreamReadReply>
{
    public StreamReadReply Read(Stream stream, ObjectPrefix prefix)
    {
        //Read the EventID
        Span<byte> idBuffer = stackalloc byte[16];
        stream.ReadExact(idBuffer, 16);
        Guid eventId = new Guid(idBuffer);

        //The remaining data is the actual stream data
        int bIn = prefix.ObjectLength - 16;

        //Allocate buffer from memory pool
        IMemoryOwner<byte> memory = MemoryPool<byte>.Shared.Rent(prefix.ObjectLength);

        try
        {
            stream.ReadExact(memory.Memory.Span, bIn);

            return new StreamReadReply
            {
                Memory = memory,
                BIn = bIn,
                EventId = eventId
            };
        }
        catch(Exception)
        {
            memory.Dispose();
            throw;
        }
    }

    public void Write(Stream stream, StreamReadReply value)
    {
        ObjectManager.Instance.WritePrefixForType<StreamReadReply>(stream, value.BIn + 16);

        //Write the EventID
        Span<byte> idBuffer = stackalloc byte[16];
        value.EventId.TryWriteBytes(idBuffer);
        stream.Write(idBuffer);

        //Write the actual return data
        stream.Write(value.Memory.Memory.Span.Slice(0, value.BIn));
    }
}
