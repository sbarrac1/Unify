using Autofac;
using Unify.Core;
using Unify.Core.Common;
using Unify.Core.CommonServices.Clipboard.Controller;
using Unify.Core.CommonServices.Clipboard.StationService;
using Unify.Core.CommonServices.DataMarshal.Controller;
using Unify.Core.CommonServices.DataMarshal.Service;
using Unify.Core.CommonServices.FileSys.Controller;
using Unify.Core.CommonServices.FileSys.Service;
using Unify.Core.CommonServices.InputHook.Controller;
using Unify.Core.CommonServices.Streams.Controller;
using Unify.Core.CommonServices.Streams.Service;
using Unify.Core.Events;
using Unify.Core.Events.Dispatch;
using Unify.Core.Net.IO;
using Unify.Core.Net.Processing;
using Unify.Core.StationHosts;
using Unify.Windows.Shared.Input.Driver;

namespace Unify.Windows.Shared.StationHost;

public sealed class WinStationHostFactory : IStationHostFactory
{
    public IDisposable Create(IEventStream eventStream, StationHostConfig config, Action<Exception> onFault)
    {
        var scope = Configure(eventStream, config);
        var processor = scope.Resolve<IEventProcessor>();

        try
        {
            processor.BeginBackgroundWorker((ex) =>
            {
                processor.Dispose();
                scope.Dispose();
                onFault(ex);
            });

            var hook = scope.Resolve<IInputHookController>();
            scope.Resolve<IStationClipboardController>();
        }
        catch(Exception)
        {
            processor.Dispose();
            scope.Dispose();
            throw;
        }

        return new CallbackDisposable(() =>
        {
            processor.Dispose();
            scope.Dispose();
        });
    }

    class CallbackDisposable : IDisposable
    {
        private readonly Action _callback;

        public CallbackDisposable(Action callback)
        {
            _callback = callback;
        }

        public void Dispose()
        {
            _callback();
        }
    }

    private static IContainer Configure(IEventStream eventStream, StationHostConfig config)
    {
        var builder = new ContainerBuilder();

        builder.RegisterInstance(config).AsSelf();
        builder.RegisterInstance(eventStream).AsImplementedInterfaces();
        builder.AddEventHandlers(typeof(IEvent).Assembly);

        builder.RegisterType<EventProcessor>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<Dispatcher>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<StreamsController>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<MarshalController>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<FileSysController>().AsImplementedInterfaces().SingleInstance();

        builder.RegisterGeneric(typeof(Repository<>)).AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<StreamsService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<MarshalService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<StationClipboardService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<FileSysService>().AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<WinClipboardController>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<WinInputDriverController>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<WinInputDriver>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<WinInputHookController>().AsImplementedInterfaces().SingleInstance();

        return builder.Build();
    }
}