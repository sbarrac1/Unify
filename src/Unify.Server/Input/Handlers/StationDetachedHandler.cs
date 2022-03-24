using Unify.Core.Events.Handlers;
using Unify.Server.Common;
using Unify.Server.Stations;
using Unify.Server.Stations.Types;

namespace Unify.Server.Input.Handlers;

public sealed class StationDetachedHandler : IEventHandler<StationDetachedEvent>
{
    private readonly IStation _sender;
    private readonly IServerInputController _inputController;
    private readonly IStationRepository _stationRepository;
    private readonly ServerContext _context;

    public StationDetachedHandler(IStation sender, IServerInputController inputController, IStationRepository stationRepository, ServerContext context)
    {
        _sender = sender;
        _inputController = inputController;
        _stationRepository = stationRepository;
        _context = context;
    }
    
    public void Handle(StationDetachedEvent evt)
    {
        lock (_context.SyncObject)
        {
            if (_inputController.Target == _sender)
            {
                _inputController.SetTarget(_stationRepository.Primary);
            }
        }
    }
}