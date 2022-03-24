using Unify.Core.Common;
using Unify.Core.CommonServices.Streams.Controller;
using Unify.Core.Events;
using Unify.Core.Events.Target.Exceptions;

namespace Unify.Core.CommonServices.FileSys.Controller.Handlers;
public sealed class FileSysGetFileHandler :
    IRequestHandler<FileSysGetFileStreamRequest, FileSysGetFileStreamReply>
{
    private readonly IRepository<IHostedFileSysContext> _hostedContextRepository;
    private readonly IStreamsController _streamsController;

    public FileSysGetFileHandler(IRepository<IHostedFileSysContext> hostedContextRepository,
        IStreamsController streamsController)
    {
        _hostedContextRepository = hostedContextRepository;
        _streamsController = streamsController;
    }

    public FileSysGetFileStreamReply Handle(FileSysGetFileStreamRequest request)
    {
        if (!_hostedContextRepository.TryGet(request.ContextId, out var hostedContext))
            throw new RequestFailedException($"Invalid context ID {request.ContextId}");

        var streamHandle = hostedContext.Context.GetFileStream(request.File);

        return new FileSysGetFileStreamReply
        {
            Header = _streamsController.HostStream(streamHandle)
        };
    }
}
