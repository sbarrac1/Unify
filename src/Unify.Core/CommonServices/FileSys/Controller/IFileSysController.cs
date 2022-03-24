using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Core.CommonServices.FileSys.Controller;
public interface IFileSysController
{
    IHostedFileSysContext HostContext(IFileSysContext context);
}
