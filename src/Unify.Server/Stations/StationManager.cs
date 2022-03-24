using Unify.Server.Stations.Types;

namespace Unify.Server.Stations;
public sealed class StationManager : IStationRepository, IStationManager
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly object _lockObject = new();

    private IPrimaryStation _primaryStation;
    public IPrimaryStation Primary { get
        {
            lock (_lockObject)
            {
                return _primaryStation;
            }
        }
    }

    public IReadOnlyList<IStation> Stations
    {
        get
        {
            lock (_lockObject)
            {
                return _stations;
            }
        }
    }
   
    private readonly List<IStation> _stations = new();

    public bool TryGetStation(string name, out IStation station)
    {
        lock (_lockObject)
        {
            return (station = _stations.FirstOrDefault(x => string.Equals(name, x.Name, StringComparison.OrdinalIgnoreCase)))
                   != null;
        }
    }

    public void AttachStation(IStation station)
    {
        lock (_lockObject)
        {
            _logger.Info($"Attaching station {station.Name}");
            _stations.Add(station);
        }
    }

    public void DetachStation(IStation station)
    {
        lock (_lockObject)
        {
            _logger.Info($"Detaching station {station.Name}");
            _stations.Remove(station);
        }
    }

    public void AttachPrimary(IPrimaryStation primaryStation)
    {
        lock (_lockObject)
        {
            _primaryStation = primaryStation;
            _stations.Add(primaryStation);
            _logger.Trace("Primary station set");
        }
    }
}
