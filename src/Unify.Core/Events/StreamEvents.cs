using ProtoBuf;
using System.Buffers;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Events;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(600)]
public sealed class StreamReadRequest : IRequest<StreamReadReply>
{
    public Guid EventId { get; set; }
    public Guid StreamId { get; init; }
    public int BytesToRead { get; init; }
    public long StartPosition { get; init; }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(601)]
public sealed class StreamReadReply : IEvent, IDisposable
{
    public Guid EventId { get; set; }

    /// <summary>
    /// The correct amount of bytes in <seealso cref="Memory"/>
    /// </summary>
    public int BIn { get; init; }

    /// <summary>
    /// A handle to memory that contains the returned data
    /// </summary>
    public IMemoryOwner<byte> Memory { get; init; }

    public void Dispose()
    {
        Memory?.Dispose();
    }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(602)]
public sealed class StreamDisposeEvent : IEvent
{
    public Guid EventId { get; set; }

    public Guid StreamId { get; init; }
}