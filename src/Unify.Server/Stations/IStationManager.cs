using Unify.Server.Stations.Types;

namespace Unify.Server.Stations;
public interface IStationManager : IStationRepository
{
    void AttachStation(IStation station);
    void DetachStation(IStation station);

    void AttachPrimary(IPrimaryStation primaryStation);
}
