using Unify.Core.Net.Handshake.Server;

namespace Unify.Server.Stations.Startup;
public interface IRemoteStationInitializer
{
    void Initialize(IUnknownClient unknownClient);
}
