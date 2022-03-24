using Unify.Core.Common.Input.Types;
using Unify.Server.Stations.Types;

namespace Unify.Server.Input;

public interface IServerInputController
{
    IStation Target { get; }

    void SetTarget(IStation station);
    void Send(IInput input);

    void Initialize();
}