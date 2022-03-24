using Unify.Core.Events;
using Unify.Core.Events.Handlers;
using Unify.Server.Stations.Types;

namespace Unify.Server.Input.Hotkeys.Handlers;

public sealed class HotkeyPressedHandler : IEventHandler<HotkeyPressedEvent>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    
    private readonly IStation _sender;
    private readonly IServerHotkeyManager _hotkeyManager;

    public HotkeyPressedHandler(IStation sender, IServerHotkeyManager hotkeyManager)
    {
        _sender = sender;
        _hotkeyManager = hotkeyManager;
    }
    
    public void Handle(HotkeyPressedEvent evt)
    {
        if (!_sender.IsPrimary)
            throw new InvalidOperationException("Non-primary station sent hotkey pressed event");
        
        _logger.Info($"{_sender.Name}: Pressed hotkey {evt.Hotkey}");

        _hotkeyManager.OnHotkeyPressed(evt.Hotkey);
    }
}