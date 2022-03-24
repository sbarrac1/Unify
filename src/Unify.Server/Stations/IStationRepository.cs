using Unify.Server.Stations.Types;

namespace Unify.Server.Stations;
public interface IStationRepository
{
    IPrimaryStation Primary { get; }
    IReadOnlyList<IStation> Stations { get; }

    bool TryGetStation(string name, out IStation station);
}
