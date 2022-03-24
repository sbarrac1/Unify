using Autofac;
using Unify.Core;
using Unify.Core.Common;
using Unify.Core.CommonServices.Clipboard.StationService;
using Unify.Core.CommonServices.DataMarshal.Controller;
using Unify.Core.CommonServices.DataMarshal.Service;
using Unify.Core.CommonServices.FileSys.Controller;
using Unify.Core.CommonServices.FileSys.Service;
using Unify.Core.CommonServices.InputDriver.Server;
using Unify.Core.CommonServices.InputHook.Service;
using Unify.Core.CommonServices.Streams.Controller;
using Unify.Core.CommonServices.Streams.Service;
using Unify.Core.Events;
using Unify.Core.Events.Dispatch;
using Unify.Core.Net.Processing;
using Unify.Server.Clipboard;
using Unify.Server.Common;
using Unify.Server.Common.Config;
using Unify.Server.Config;
using Unify.Server.Input;
using Unify.Server.Input.Hotkeys;
using Unify.Server.Stations;
using Unify.Server.Stations.Startup;
using Unify.Windows.Shared.StationHost;

namespace Unify.Server;
public static class ServerContainerConfig
{
    public static IContainer Configure()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<Server>().AsSelf().SingleInstance();
        builder.RegisterType<ServerContext>().AsSelf().SingleInstance();

        builder.RegisterType<AppSettingsService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<PrimaryStationInitializer>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<RemoteStationInitializer>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<WinStationHostFactory>().AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<StationManager>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<ServerClipboardController>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<WinStationHostFactory>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<ServerInputController>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<ServerHotkeyManager>().AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<EventProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterType<Dispatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterType<StreamsController>().AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterType<MarshalController>().AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterType<FileSysController>().AsImplementedInterfaces().InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(Repository<>)).AsImplementedInterfaces().InstancePerLifetimeScope();

        builder.RegisterType<StreamsService>().AsImplementedInterfaces().InstancePerDependency();
        builder.RegisterType<MarshalService>().AsImplementedInterfaces().InstancePerDependency();
        builder.RegisterType<FileSysService>().AsImplementedInterfaces().InstancePerDependency();
        builder.RegisterType<StationClipboardService>().AsImplementedInterfaces().InstancePerDependency();
        builder.RegisterType<InputDriverService>().AsImplementedInterfaces().InstancePerDependency();
        builder.RegisterType<InputHookService>().AsImplementedInterfaces().InstancePerDependency();

        builder.AddEventHandlers(typeof(IEvent).Assembly, typeof(Server).Assembly);

        return builder.Build();
    }
}
