using Unify.Core.Events.Handlers;
using Unify.Server.Common;
using Unify.Server.Stations;
using Unify.Server.Stations.Types;

namespace Unify.Server.Clipboard.Handlers;

public sealed class StationDetachedHandler : IEventHandler<StationDetachedEvent>
{
    private readonly IStation _sender;
    private readonly IServerClipboardController _clipboardController;
    private readonly IStationRepository _stationRepository;

    public StationDetachedHandler(
        IStation sender,
        IServerClipboardController clipboardController,
        IStationRepository stationRepository)
    {
        _sender = sender;
        _clipboardController = clipboardController;
        _stationRepository = stationRepository;
    }
    
    public void Handle(StationDetachedEvent evt)
    {
        if (_clipboardController.Owner == _sender)
        {
            _clipboardController.SetOwner(_stationRepository.Primary);
        }
    }
}