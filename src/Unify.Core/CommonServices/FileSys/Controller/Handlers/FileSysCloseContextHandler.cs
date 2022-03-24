using Unify.Core.Common;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.FileSys.Controller.Handlers;
public sealed class FileSysCloseContextHandler : IEventHandler<FileSysCloseContextCommand>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IRepository<IHostedFileSysContext> _hostedContextRepository;

    public FileSysCloseContextHandler(IRepository<IHostedFileSysContext> hostedContextRepository)
    {
        _hostedContextRepository = hostedContextRepository;
    }

    public void Handle(FileSysCloseContextCommand evt)
    {
        if (!_hostedContextRepository.TryGet(evt.ContextId, out var context))
        {
            _logger.Warn($"Could not close filesys context: Invalid context Id");
            return;
        }

        context.Dispose();
    }
}
