using System.Threading.Tasks.Sources;
using Unify.Core.Events.Handlers;
using Unify.Server.Common;
using Unify.Server.Config;
using Unify.Server.Input.Hotkeys;
using Unify.Server.Stations.Types;

namespace Unify.Server.Input.Handlers;

public sealed class StationAttachedHandler : IEventHandler<StationAttachedEvent>
{
    private readonly IStation _sender;
    private readonly IServerHotkeyManager _hotkeyManager;
    private readonly IServerInputController _inputController;

    public StationAttachedHandler(IStation sender,
        IServerHotkeyManager hotkeyManager,
        IServerInputController inputController)
    {
        _sender = sender;
        _hotkeyManager = hotkeyManager;
        _inputController = inputController;
    }
    
    public void Handle(StationAttachedEvent evt)
    {
        if (_sender.Config.Hotkey != null)
        {
            _hotkeyManager.RegisterHotkey(_sender.Config.Hotkey, () => _inputController.SetTarget(_sender));
        }
    }
}