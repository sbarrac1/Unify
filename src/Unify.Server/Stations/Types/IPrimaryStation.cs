using Unify.Core.CommonServices.InputHook.Service;

namespace Unify.Server.Stations.Types;
public interface IPrimaryStation : IStation
{
    IInputHookService InputHookService { get; }
}
