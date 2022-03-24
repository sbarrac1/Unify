using Autofac;
using System.Net;
using System.Net.Sockets;
using Unify.Core.Net.Handshake;
using Unify.Core.Net.IO;
using Unify.Core.Net.Listener;
using Unify.Server.Clipboard;
using Unify.Server.Common;
using Unify.Server.Config;
using Unify.Server.Input;
using Unify.Server.Input.Hotkeys;
using Unify.Server.Stations.Startup;

namespace Unify.Server;
public sealed class Server : IDisposable
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    
    private readonly IPrimaryStationInitializer _primaryStationInitializer;
    private readonly IServerSettingsService _configService;
    private readonly IServerClipboardController _clipboardController;
    private readonly IRemoteStationInitializer _remoteStationInitializer;
    private readonly IServerInputController _inputController;
    private readonly IServerHotkeyManager _hotkeyManager;
    private readonly ServerContext _context;
    
    private ISocketListener _listener;

    public static Server Create()
    {
        return ServerContainerConfig.Configure().Resolve<Server>();
    }

    public Server(
        IPrimaryStationInitializer primaryStationInitializer,
        IServerSettingsService configService,
        IServerClipboardController clipboardController,
        IRemoteStationInitializer remoteStationInitializer,
        IServerInputController inputController,
        IServerHotkeyManager hotkeyManager,
        ServerContext context)
    {
        _clipboardController = clipboardController;
        _remoteStationInitializer = remoteStationInitializer;
        _inputController = inputController;
        _hotkeyManager = hotkeyManager;
        _context = context;
        _primaryStationInitializer = primaryStationInitializer;
        _configService = configService;
    }

    public void Start(IPEndPoint bindAddress)
    {
        _logger.Info($"Starting server @ {bindAddress}");

        try
        {
            lock (_context.SyncObject)
            {
                _primaryStationInitializer.Initialize();
                _clipboardController.Initialize();
                _inputController.Initialize();
                
                _listener = new AsyncSocketListener(bindAddress, AcceptSocketAsync);

                _context.ServerShutdownCts.Token.Register(() =>
                {
                    _logger.Info("Server shutdown -> stopping clientlistener");
                    _listener.Dispose();
                });

                if(_configService.StopHotkey != null)
                {
                    _logger.Info($"Setting server stop hotkey to {_configService.StopHotkey}");
                    _hotkeyManager.RegisterHotkey(_configService.StopHotkey, () => Dispose());
                }
                
                _logger.Info("Server started!");
            }
        }
        catch (Exception ex)
        {
            Dispose();
            _logger.Error(ex, "Failed to start server!");
        }
    }

    private async Task AcceptSocketAsync(Socket socket)
    {
        var eventStream = new ProtoEventStream(new NetworkStream(socket, true));
        var cts = new CancellationTokenSource(3000);

        var unknownClient = await new ServerHandshakeRunner().DoHandshakeAsync(eventStream, cts.Token);

        lock (_context.SyncObject)
        {
            if (_context.ServerShutdownCts.IsCancellationRequested)
            {
                socket.Dispose();
                return;
            }
            
            socket.NoDelay = true;
            _remoteStationInitializer.Initialize(unknownClient);
        }
    }
    
    public void Dispose()
    {
        lock (_context.SyncObject)
        {
            _listener?.Dispose();

            _context.ServerShutdownCts.Cancel();
        }
    }
}
