using Unify.Core.Common;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.Streams.Controller.Handlers;
public sealed class StreamReadHandler : IRequestHandler<StreamReadRequest, StreamReadReply>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IRepository<Stream> _hostedStreamRepository;

    public StreamReadHandler(IRepository<Stream> hostedStreamRepository)
    {
        _hostedStreamRepository = hostedStreamRepository;
    }

    public StreamReadReply Handle(StreamReadRequest request)
    {
        if (!_hostedStreamRepository.TryGet(request.StreamId, out var stream))
            throw new ArgumentException($"Could not find stream with ID {request.StreamId}");

        if (_logger.IsTraceEnabled)
            _logger.Trace($"Reading {request.BytesToRead} bytes from position {request.StartPosition} of stream {request.StreamId}");

        //Todo - wrap the Stream object in an interface to allow proper locking
        //of the stream instance. If this stream was read from anywhere else, the returned
        //data will likely be invalid
        lock (stream)
        {
            ValidateRequest(request, stream);
            stream.Seek(request.StartPosition, SeekOrigin.Begin);

            byte[] buffer = new byte[request.BytesToRead];
            int bIn = stream.Read(buffer, 0, buffer.Length);

            if (_logger.IsTraceEnabled)
                _logger.Trace($"Read {bIn} bytes from stream {request.StreamId}");

            return new StreamReadReply
            {
                Data = EnsureBufferSize(buffer, bIn)
            };
        }
    }

    private byte[] EnsureBufferSize(byte[] buffer, int bIn)
    {
        if(buffer.Length == bIn)
            return buffer;

        byte[] resized = new byte[bIn];
        Buffer.BlockCopy(buffer, 0, resized, 0, bIn);
        return resized;
    }

    private void ValidateRequest(StreamReadRequest request, Stream stream)
    {
        if (request.BytesToRead > 275 * 1024)
            throw new ArgumentException("BytesToRead was too large");

        if (request.StartPosition > stream.Length)
            throw new ArgumentException("Stream start position was invalid");
    }
}
