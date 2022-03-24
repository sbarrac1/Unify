using Unify.Core.CommonServices.InputDriver.Controller;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.InputDriver.Handlers;

public sealed class SendInputCommandHandler : IEventHandler<SendInputCommand>
{
    private readonly IInputDriverController _inputDriverController;

    public SendInputCommandHandler(IInputDriverController inputDriverController)
    {
        _inputDriverController = inputDriverController;
    }
    
    public void Handle(SendInputCommand evt)
    {
        _inputDriverController.SendInput(evt.Input);
    }
}