using Unify.Core.Common;
using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Core.CommonServices.FileSys.Controller;
public sealed class FileSysController : IFileSysController
{
    private readonly IRepository<IHostedFileSysContext> _hostedContextRepository;

    public FileSysController(IRepository<IHostedFileSysContext> hostedContextRepository)
    {
        _hostedContextRepository = hostedContextRepository;
    }

    public IHostedFileSysContext HostContext(IFileSysContext context)
    {
        return new HostedFileSysContext(context, _hostedContextRepository);
    }
}
