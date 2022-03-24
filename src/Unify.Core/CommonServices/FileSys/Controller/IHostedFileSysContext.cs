using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Core.CommonServices.FileSys.Controller;
public interface IHostedFileSysContext : IDisposable
{
    Guid ContextId { get; }
    IFileSysContext Context { get; }
}
