using Unify.Core.StationHosts;

namespace Unify.Client.Config;
public interface IClientConfigService
{
    StationHostConfig StationHostConfig { get; }
}
