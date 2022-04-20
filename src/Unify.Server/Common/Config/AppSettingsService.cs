using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Unify.Core.Common;
using Unify.Core.Common.Input;
using Unify.Core.StationHosts;
using Unify.Server.Config;

namespace Unify.Server.Common.Config;
public sealed class AppSettingsService : IServerSettingsService
{
    public Hotkey StopHotkey
    {
        get
        {
            string value = ConfigurationManager.AppSettings["Server.StopHotkey"];

            if (Hotkey.TryParse(value, out var hotkey))
                return hotkey;

            return null;
        }
    }

    public bool EnableClipboard => ConfigurationManager.AppSettings["Server.EnableClipboard"] == "True";

    public StationHostConfig StationHostConfig
    {
        get
        {
            return new StationHostConfig
            {
                EnableClipboard = ConfigurationManager.AppSettings["Server.EnableClipboard"] == "True",
                EnableHooks = ConfigurationManager.AppSettings["Server.EnableHooks"] == "True"
            };
        }
    }

    public string GetStationAtSide(string stationName, Side side)
    {
        string key = $"Server.Station.{stationName}.{GetSideString(side)}";

        return ConfigurationManager.AppSettings[key];
    }

    public bool IsStationEnabled(string stationName)
    {
        string key = $"Server.Station.{stationName}.Enabled";

        return ConfigurationManager.AppSettings[key] == "True";
    }

    public bool TryGetStationAtSide(string stationName, Side side, [NotNullWhen(true)] out string targetStationName)
    {
        string key = $"Server.Station.{stationName}.{GetSideString(side)}";

        targetStationName = ConfigurationManager.AppSettings[key];

        return targetStationName != null;
    }

    public StationConfig GetStationConfig(string stationName)
    {
        var config = new StationConfig();

        config.LeftStation = ConfigurationManager.AppSettings.Get($"Server.Station.{stationName}.LeftStation");
        config.RightStation = ConfigurationManager.AppSettings.Get($"Server.Station.{stationName}.RightStation");
        config.TopStation = ConfigurationManager.AppSettings.Get($"Server.Station.{stationName}.TopStation");
        config.BottomStation = ConfigurationManager.AppSettings.Get($"Server.Station.{stationName}.BottomStation");

        if (Hotkey.TryParse(ConfigurationManager.AppSettings.Get($"Server.Station.{stationName}.Hotkey"), out var hotkey))
            config.Hotkey = hotkey;

        return config;
    }

    private string GetSideString(Side side)
    {
        return side switch
        {
            Side.Left => "LeftStation",
            Side.Right => "RightStation",
            Side.Top => "TopStation",
            Side.Bottom => "BottomStation"
        };
    }
}
