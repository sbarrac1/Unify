using Unify.Core.Common.Input.Types;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.InputDriver.Server;

public sealed class InputDriverService : IInputDriverService
{
    private readonly IEventTarget _eventTarget;

    public InputDriverService(IEventTarget eventTarget)
    {
        _eventTarget = eventTarget;
    }
    
    public void SendInput(IInput input)
    {
        _eventTarget.PostEvent(new SendInputCommand()
        {
            Input = input
        });
    }
}