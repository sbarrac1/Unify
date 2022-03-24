using Unify.Core.Common.Input;
using Unify.Core.Common.Input.Types;
using Unify.Server.Common;
using Unify.Server.Config;
using Unify.Server.Input.Hotkeys;
using Unify.Server.Stations;
using Unify.Server.Stations.Types;

namespace Unify.Server.Input;

public sealed class ServerInputController : IServerInputController
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    
    public IStation Target { get; private set; }
    
    private readonly IStationRepository _stationRepository;
    private readonly ServerContext _context;
    private readonly IServerHotkeyManager _hotkeyManager;
    private readonly IServerSettingsService _configService;

    public ServerInputController(IStationRepository stationRepository,
        ServerContext serverContext,
        IServerHotkeyManager hotkeyManager,
        IServerSettingsService configService)
    {
        _stationRepository = stationRepository;
        _context = serverContext;
        _hotkeyManager = hotkeyManager;
        _configService = configService;
    }
    
    public void SetTarget(IStation station)
    {
        lock (_context.SyncObject)
        {
            _logger.Info($"Switching input target to {station.Name}");

            _stationRepository.Primary.InputHookService.SetGrabState(!station.IsPrimary);
            
            this.Target = station;
        }
    }

    public void Send(IInput input)
    {
        this.Target.InputDriverService.SendInput(input);
    }

    public void Initialize()
    {
        SetTarget(_stationRepository.Primary);

        if(_stationRepository.Primary.Config.Hotkey != null)
        {
            _hotkeyManager.RegisterHotkey(_stationRepository.Primary.Config.Hotkey, () => SetTarget(_stationRepository.Primary));
        }
    }
}