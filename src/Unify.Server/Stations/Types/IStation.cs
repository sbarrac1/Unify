using Unify.Core.CommonServices.Clipboard.StationService;
using Unify.Core.CommonServices.InputDriver.Server;
using Unify.Server.Config;

namespace Unify.Server.Stations.Types;

/// <summary>
/// Represents a station that is connected to the server
/// </summary>
public interface IStation
{
    bool IsPrimary { get => this is IPrimaryStation; }
    string Name { get; }
    
    StationConfig Config { get; }

    IStationClipboardService ClipboardService { get; }
    IInputDriverService InputDriverService { get; }
}
