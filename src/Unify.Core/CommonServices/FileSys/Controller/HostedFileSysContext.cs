using Unify.Core.Common;
using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Core.CommonServices.FileSys.Controller;
public sealed class HostedFileSysContext : IHostedFileSysContext
{
    private readonly IRepository<IHostedFileSysContext> _hostedContextRepository;

    public Guid ContextId { get; }
    public IFileSysContext Context { get; }

    public HostedFileSysContext(IFileSysContext context, IRepository<IHostedFileSysContext> hostedContextRepository)
    {
        _hostedContextRepository = hostedContextRepository;
        Context = context;
        ContextId = Guid.NewGuid();

        _hostedContextRepository.Add(ContextId, this);
    }

    public void Dispose()
    {
        _hostedContextRepository.Remove(ContextId);
        Context.Dispose();
    }
}
