using Unify.Server.Stations.Types;

namespace Unify.Server.Stations;

public interface IStationContext<TStation> : IDisposable
    where TStation : IStation
{
    TStation Station { get; }
}