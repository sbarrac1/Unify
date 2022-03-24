using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.CommonServices.Streams.Service;

public sealed class StreamsService : IStreamsService
{
    private readonly IEventTarget _eventTarget;

    public StreamsService(IEventTarget eventTarget)
    {
        _eventTarget = eventTarget;
    }

    public Stream GetStream(StreamHeader header)
    {
        return new RemoteStream(_eventTarget, header);
    }
}
