using Autofac;
using Unify.Core.CommonServices.Clipboard.StationService;
using Unify.Core.CommonServices.InputDriver.Server;
using Unify.Core.Events.Dispatch;
using Unify.Core.Net.Handshake.Server;
using Unify.Core.Net.IO;
using Unify.Core.Net.Processing;
using Unify.Server.Common;
using Unify.Server.Config;
using Unify.Server.Stations.Types;

namespace Unify.Server.Stations.Startup;
public sealed class RemoteStationInitializer : IRemoteStationInitializer
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IStationManager _stationManager;
    private readonly IServerSettingsService _configService;
    private readonly ILifetimeScope _serverScope;
    private readonly ServerContext _serverContext;

    public RemoteStationInitializer(IStationManager stationManager,
        IServerSettingsService configService,
        ILifetimeScope serverScope,
        ServerContext serverContext)
    {
        _stationManager = stationManager;
        _configService = configService;
        _serverScope = serverScope;
        _serverContext = serverContext;
    }

    public void Initialize(IUnknownClient unknownClient)
    {
        lock (_serverContext.SyncObject)
        {
            string stationName = unknownClient.Info.StationName;

            if (_serverContext.Stopping)
            {
                _logger.Info($"Declining client {stationName}: Server is stopping");
                unknownClient.DeclineClient("Server is stopping");
                return;
            }

            if (!_configService.IsStationEnabled(stationName))
            {
                _logger.Error($"Declining station {stationName}: No config defined");
                unknownClient.DeclineClient("No config defined");
                return;
            }

            if (_stationManager.TryGetStation(stationName, out _))
            {
                _logger.Error($"Declining station {stationName}: Duplicate station name");
                unknownClient.DeclineClient("Duplicate station name");
                return;
            }


            var config = _configService.GetStationConfig(stationName);

            InternalInitialize(unknownClient, config);
        }
    }

    private void InternalInitialize(IUnknownClient unknownClient, StationConfig config)
    {
        lock (_serverContext.SyncObject)
        {
            _logger.Info($"Initializing station {unknownClient.Info.StationName}");

            var eventStream = unknownClient.AcceptClient();
            var stationScope = ConfigureScope(_serverScope, eventStream, config, unknownClient.Info.StationName);
            var station = stationScope.Resolve<IStation>();
            var stationDispatcher = stationScope.Resolve<IDispatcher>();
            var stationProcessor = stationScope.Resolve<IEventProcessor>();

            bool disposed = false;

            void Cleanup()
            {
                _stationManager.DetachStation(station);
                stationDispatcher.DispatchEvent(new StationDetachedEvent());
                stationProcessor.Dispose();
                stationScope.Dispose();
            }

            var registration = _serverContext.ServerShutdownCts.Token.Register(() =>
            {
                _logger.Info("Server shutdown -> remove station " + station.Name);

                Cleanup();
            });

            stationProcessor.BeginBackgroundWorker((ex) =>
            {
                registration.Unregister();

                _logger.Error(ex, $"{station.Name} disconnected");

                Cleanup();
            });

            _stationManager.AttachStation(station);
            stationDispatcher.DispatchEvent(new StationAttachedEvent());
        }
    }

    private static ILifetimeScope ConfigureScope(ILifetimeScope serverScope, IEventStream eventStream, StationConfig config, string stationName)
    {
        return serverScope.BeginLifetimeScope((builder) =>
        {
            builder.RegisterInstance(eventStream).AsImplementedInterfaces();
            
            builder.Register(x => new Station(stationName, config,
                x.Resolve<IStationClipboardService>(),
                x.Resolve<IInputDriverService>())).AsImplementedInterfaces().SingleInstance();
        });
    }
}
