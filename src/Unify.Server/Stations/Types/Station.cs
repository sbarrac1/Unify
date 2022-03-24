using Unify.Core.CommonServices.Clipboard.StationService;
using Unify.Core.CommonServices.InputDriver.Server;
using Unify.Server.Config;

namespace Unify.Server.Stations.Types;
public class Station : IStation
{
    public string Name { get; }
    public StationConfig Config { get; }
    public IStationClipboardService ClipboardService { get; }
    public IInputDriverService InputDriverService { get; }

    public Station(string name,
        StationConfig config,
        IStationClipboardService clipboardService,
        IInputDriverService inputDriverService)
    {
        this.Name = name;
        this.ClipboardService = clipboardService;
        this.InputDriverService = inputDriverService;
        this.Config = config;
    }
}
