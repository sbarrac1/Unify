using Unify.Core.Common;
using Unify.Core.Events;
using Unify.Core.Events.Target.Exceptions;

namespace Unify.Core.CommonServices.FileSys.Controller.Handlers;
public sealed class FileSysGetEntriesHandler : IRequestHandler<
    FileSysGetEntriesRequest, FileSysGetEntriesReply>
{
    private readonly IRepository<IHostedFileSysContext> _hostedContextRepository;

    public FileSysGetEntriesHandler(IRepository<IHostedFileSysContext> hostedContextRepository)
    {
        _hostedContextRepository = hostedContextRepository;
    }

    public FileSysGetEntriesReply Handle(FileSysGetEntriesRequest request)
    {
        if (!_hostedContextRepository.TryGet(request.ContextId, out var hostedContext))
            throw new RequestFailedException($"Invalid context ID {request.ContextId}");

        return new FileSysGetEntriesReply
        {
            Entries = hostedContext.Context.GetSubEntries(request.Directory).ToArray()
        };
    }
}
