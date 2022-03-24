using Unify.Core.Events;
using Unify.Core.Events.Handlers;
using Unify.Server.Stations.Types;

namespace Unify.Server.Input.Handlers;

public sealed class InputReceivedHandler : IEventHandler<InputReceivedEvent>
{
    private readonly IStation _sender;
    private readonly IServerInputController _inputController;

    public InputReceivedHandler(IStation sender, IServerInputController inputController)
    {
        _sender = sender;
        _inputController = inputController;
    }
    
    public void Handle(InputReceivedEvent evt)
    {
        if (!_sender.IsPrimary)
            throw new InvalidOperationException("Non primary station sent input event");
        
        _inputController.Send(evt.Input);
    }
}