using NLog;
using System.Net;
using System.Net.Sockets;
using Unify.Client.Config;
using Unify.Core.Net.Handshake;
using Unify.Core.Net.IO;
using Unify.Windows.Shared.StationHost;

namespace Unify.Client;
public sealed class Client : IClient
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly Action _onDispose;

    public static async Task <IClient> CreateAsync(IPEndPoint serverAddress, string stationName)
    {
        _logger.Info("Creating client");

        var socket = new TcpClient();

        _logger.Info("Connecting to " + serverAddress.ToString());

        socket.Connect(serverAddress);

        _logger.Info("Connected to " + serverAddress.ToString());

        var protoStream = new ProtoEventStream(socket.GetStream());

        _logger.Info($"Doing handshake...");
        await new ClientHandshakeRunner().DoHandshakeAsync(protoStream, new ClientInfo
        {
            StationName = stationName
        }, default);

        _logger.Info("Creating station host services");

        var stationHostConfig = new AppSettingsService().StationHostConfig;

        var stationHost = new WinStationHostFactory().Create(protoStream, stationHostConfig, (ex) =>
        {
            _logger.Error(ex, "Station host stopped due to exception");
        });

        return new Client(() =>
        {
            _logger.Info("Stopping client");
            stationHost.Dispose();
            protoStream.Dispose();

            _logger.Info("Client stopped");
        });
    }

    private Client(Action onDispose)
    {
        _onDispose = onDispose;
    }

    bool _disposed;
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _onDispose();
    }
}
