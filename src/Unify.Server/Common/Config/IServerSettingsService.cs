using System.Diagnostics.CodeAnalysis;
using Unify.Core.Common;
using Unify.Core.Common.Input;
using Unify.Core.StationHosts;

namespace Unify.Server.Config;
public interface IServerSettingsService
{
    bool EnableClipboard { get; }

    Hotkey StopHotkey { get; }

    StationHostConfig StationHostConfig { get; }

    StationConfig GetStationConfig(string stationName);
    string GetStationAtSide(string stationName, Side side);
    bool IsStationEnabled(string stationName);
}
