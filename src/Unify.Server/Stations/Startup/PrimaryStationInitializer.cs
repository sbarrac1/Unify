using Autofac;
using Unify.Core.CommonServices.Clipboard.StationService;
using Unify.Core.CommonServices.InputDriver.Server;
using Unify.Core.CommonServices.InputHook.Service;
using Unify.Core.Net.IO;
using Unify.Core.Net.Processing;
using Unify.Core.StationHosts;
using Unify.Server.Common;
using Unify.Server.Config;
using Unify.Server.Input;
using Unify.Server.Input.Hotkeys;
using Unify.Server.Stations.Types;

namespace Unify.Server.Stations.Startup;

public sealed class PrimaryStationInitializer : IPrimaryStationInitializer
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    
    private readonly ILifetimeScope _serverScope;
    private readonly IStationHostFactory _stationHostFactory;
    private readonly ServerContext _serverContext;
    private readonly IStationManager _stationManager;
    private readonly IServerSettingsService _configService;

    private readonly object _lockObject = new();
    private bool _stopping;

    public PrimaryStationInitializer(ILifetimeScope serverScope,
        IStationHostFactory stationHostFactory,
        ServerContext serverContext,
        IStationManager stationManager,
        IServerSettingsService configService)
    {
        _serverScope = serverScope;
        _stationHostFactory = stationHostFactory;
        _serverContext = serverContext;
        _stationManager = stationManager;
        _configService = configService;
    }

    public void Initialize()
    {
        lock (_serverContext.SyncObject)
        {
            var (streamA, streamB) = LocalEventStream.CreatePair();
            var primaryScope = ConfigureScope(streamA, _configService.GetStationConfig(PrimaryStation.NAME));

            var primaryProcessor = primaryScope.Resolve<IEventProcessor>();
            SetupPrimaryEventProcessor(primaryScope, primaryProcessor);

            SetupStationHost(streamB);

            var primaryStation = primaryScope.Resolve<IPrimaryStation>();

            _stationManager.AttachPrimary(primaryStation);
        }
    }

    private void SetupPrimaryEventProcessor(ILifetimeScope lifetimeScope, IEventProcessor eventProcessor)
    {
        eventProcessor.BeginBackgroundWorker((ex) =>
        {
            lock (_lockObject)
            {
                if (_stopping)
                    return;

                _logger.Fatal(ex, "Primary station event processor exited");

                _serverContext.ServerShutdownCts.Cancel();
            }
        });

        _serverContext.ServerShutdownCts.Token.Register(() =>
        {
            lock (_lockObject)
            {
                _stopping = true;
                _logger.Info("Server shutdown -> Closing primary station context");

                eventProcessor.Dispose();
                lifetimeScope.Dispose();
            }
        });
    }

    private void SetupStationHost(IEventStream eventStream)
    {
        var stationHost = _stationHostFactory.Create(eventStream, _configService.StationHostConfig, (ex) =>
        {
            lock (_lockObject)
            {
                if (_stopping)
                    return;

                _logger.Error(ex, $"Primary station host exited");

                _serverContext.ServerShutdownCts.Cancel();
            }
        });

        _serverContext.ServerShutdownCts.Token.Register(() =>
        {
            lock (_lockObject)
            {
                _stopping = true;
                _logger.Info("Server shutdown -> stopping primary station host");

                stationHost.Dispose();
            }
        });
    }

    private ILifetimeScope ConfigureScope(IEventStream eventStream, StationConfig config)
    {
        return _serverScope.BeginLifetimeScope((builder) =>
        {
            builder.RegisterInstance(eventStream).AsImplementedInterfaces();

            builder.Register(x => new PrimaryStation(
                config,
                x.Resolve<IStationClipboardService>(),
                x.Resolve<IInputDriverService>(),
                x.Resolve<IInputHookService>())).AsImplementedInterfaces().SingleInstance();
        });
    }
}