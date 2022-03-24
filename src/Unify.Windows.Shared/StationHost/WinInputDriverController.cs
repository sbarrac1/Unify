using Unify.Core.Common.Input.Types;
using Unify.Core.CommonServices.InputDriver.Controller;
using Unify.Windows.Shared.Input.Driver;

namespace Unify.Windows.Shared.StationHost;

public sealed class WinInputDriverController : IInputDriverController
{
    private readonly IWinInputDriver _inputDriver;

    public WinInputDriverController(IWinInputDriver inputDriver)
    {
        _inputDriver = inputDriver;
    }
    
    public void SendInput(IInput input)
    {
        _inputDriver.SendInput(input);
    }
}