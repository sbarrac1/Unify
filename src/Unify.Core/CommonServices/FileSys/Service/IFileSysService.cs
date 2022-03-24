using Unify.Core.CommonServices.FileSys.Contexts;

namespace Unify.Core.CommonServices.FileSys.Service;
public interface IFileSysService
{
    IFileSysContext GetContext(Guid contextId);
}
