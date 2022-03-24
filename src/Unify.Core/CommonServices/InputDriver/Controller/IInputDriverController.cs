using Unify.Core.Common.Input.Types;

namespace Unify.Core.CommonServices.InputDriver.Controller;

public interface IInputDriverController
{
    void SendInput(IInput input);
}