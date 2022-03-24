using Unify.Core.Common.Input.Types;

namespace Unify.Core.CommonServices.InputDriver.Server;

public interface IInputDriverService
{
    void SendInput(IInput input);
}