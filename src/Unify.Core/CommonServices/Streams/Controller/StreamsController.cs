using Unify.Core.Common;
using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.CommonServices.Streams.Controller;
public sealed class StreamsController : IStreamsController
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IRepository<Stream> _hostedStreamRepository;

    public StreamsController(IRepository<Stream> hostedStreamRepository)
    {
        _hostedStreamRepository = hostedStreamRepository;
    }

    public StreamHeader HostStream(Stream stream)
    {
        var header = new StreamHeader
        {
            StreamLength = stream.Length,
            StreamId = Guid.NewGuid()
        };

        _hostedStreamRepository.Add(header.StreamId, stream);

        if (_logger.IsDebugEnabled)
            _logger.Debug($"Hosting stream type {stream} with ID {header.StreamId}");

        return header;
    }
}
