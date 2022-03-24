using Unify.Core.Common.Input;
using Unify.Core.StationHosts;

namespace Unify.Server.Config;
public class ServerConfig
{
    public StationHostConfig StationHostConfig { get; set; }

    public Hotkey StopHotkey { get; set; }

    public Dictionary<string, StationConfig> StationConfigs { get; } = new();
}
