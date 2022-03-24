using Unify.Core.Events;
using Unify.Core.Events.Handlers;
using Unify.Server.Common;
using Unify.Server.Config;
using Unify.Server.Stations;
using Unify.Server.Stations.Types;

namespace Unify.Server.Input.Handlers;

public sealed class SideHitHandler : IEventHandler<SideHitEvent>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    
    private readonly IStation _sender;
    private readonly IServerSettingsService _configService;
    private readonly IServerInputController _inputController;
    private readonly IStationRepository _stationRepository;
    private readonly ServerContext _context;

    public SideHitHandler(IStation sender, 
        IServerSettingsService configService,
        IServerInputController inputController,
        IStationRepository stationRepository,
        ServerContext context)
    {
        _sender = sender;
        _configService = configService;
        _inputController = inputController;
        _stationRepository = stationRepository;
        _context = context;
    }
    
    public void Handle(SideHitEvent evt)
    {
        if(_logger.IsTraceEnabled)
            _logger.Trace($"{_sender.Name} hit side {evt.Side} : {evt.X}:{evt.Y}");

        if (_inputController.Target != _sender) return;

        var stationConfig = _configService.GetStationConfig(_sender.Name);

        if (stationConfig.TryGetStationAtSide(evt.Side, out var targetStation))
        {
            lock (_context.SyncObject)
            {
                if (_stationRepository.TryGetStation(targetStation, out var target))
                {
                    _inputController.SetTarget(target);
                }
            }
        }
    }
}