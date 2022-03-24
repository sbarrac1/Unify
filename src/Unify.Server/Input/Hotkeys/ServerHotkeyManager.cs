using Unify.Core.Common.Input;
using Unify.Server.Stations;

namespace Unify.Server.Input.Hotkeys;

public sealed class ServerHotkeyManager : IServerHotkeyManager
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IStationRepository _stationRepository;
    
    private readonly Dictionary<Hotkey, Action> _callbacks = new();

    public ServerHotkeyManager(IStationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }
    
    public void RegisterHotkey(Hotkey hk, Action callback)
    {
        lock (_callbacks)
        {
            if(!_callbacks.TryAdd(hk, callback))
            {
                _logger.Warn($"Failed to add hotkey {hk}: Hotkey already registered");
                return;
            }
                
            
            _stationRepository.Primary.InputHookService.RegisterHotkey(hk);
        }
    }

    public void OnHotkeyPressed(Hotkey hk)
    {
        lock (_callbacks)
        {
            if (!_callbacks.TryGetValue(hk, out var callback))
            {
                _logger.Warn($"No callback defined for hotkey {hk}");
                return;
            }

            callback();
        }
    }
}