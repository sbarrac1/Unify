using Unify.Core.Common;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.Streams.Controller.Handlers;
public sealed class StreamDisposeHandler : IEventHandler<StreamDisposeEvent>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IRepository<Stream> _hostedStreamRepository;

    public StreamDisposeHandler(IRepository<Stream> hostedStreamRepository)
    {
        _hostedStreamRepository = hostedStreamRepository;
    }

    public void Handle(StreamDisposeEvent evt)
    {
        if (!_hostedStreamRepository.TryGet(evt.StreamId, out var stream))
            throw new ArgumentException($"Could not find stream with ID {evt.StreamId}");


        if (_logger.IsTraceEnabled)
            _logger.Trace($"Closing stream {evt.StreamId}");

        _hostedStreamRepository.Remove(evt.StreamId);
        stream.Dispose();
    }
}
