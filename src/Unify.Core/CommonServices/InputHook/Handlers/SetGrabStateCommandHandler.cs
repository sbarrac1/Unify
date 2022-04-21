using Unify.Core.CommonServices.InputHook.Controller;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.InputHook.Host;

public sealed class SetGrabStateCommandHandler : IEventHandler<SetGrabStateCommand>
{
    private readonly IInputHookController _inputHookController;

    public SetGrabStateCommandHandler(IInputHookController inputHookController)
    {
        _inputHookController = inputHookController;
    }

    public void Handle(SetGrabStateCommand evt)
    {
        _inputHookController.SetGrabState(evt.State);
    }
}
