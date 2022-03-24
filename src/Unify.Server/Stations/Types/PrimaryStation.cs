using Unify.Core.CommonServices.Clipboard.StationService;
using Unify.Core.CommonServices.InputDriver.Server;
using Unify.Core.CommonServices.InputHook.Service;
using Unify.Server.Config;

namespace Unify.Server.Stations.Types;
public sealed class PrimaryStation : Station, IPrimaryStation
{
    public const string NAME = "Localhost";
    
    public PrimaryStation(
        StationConfig config,
        IStationClipboardService clipboardService,
        IInputDriverService inputDriverService,
        IInputHookService inputHookService) : base("Localhost", config, clipboardService, inputDriverService)
    {
        this.InputHookService = inputHookService;
    }

    public IInputHookService InputHookService { get; }
}
