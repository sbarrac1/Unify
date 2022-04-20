using System.Net;
using System.Net.Sockets;

namespace Unify.Core.Net.Listener;
public sealed class AsyncSocketListener : ISocketListener
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private readonly TcpListener _listener;
    private readonly Func<Socket, Task> _clientConnectCallback;
    private readonly CancellationTokenSource _stopCts = new();
    private bool _disposed;
    private readonly IPEndPoint _localEndpoint;

    public AsyncSocketListener(IPEndPoint bindAddress, Func<Socket, Task> clientConnectCallback)
    {
        _clientConnectCallback = clientConnectCallback;

        _listener = new TcpListener(bindAddress);
        _listener.Start(1);

        _localEndpoint = (_listener.LocalEndpoint as IPEndPoint)!;

        _logger.Info($"Listening for clients on TCP@{_localEndpoint}");

        _ = ListenLoopAsync();
    }

    private async Task ListenLoopAsync()
    {
        while (!_disposed)
        {
            Socket socket = null;

            try
            {
                socket = await _listener.AcceptSocketAsync(_stopCts.Token);
                _logger.Info($"Accepted tcp client {socket.RemoteEndPoint}");

                await _clientConnectCallback(socket);
            }
            catch (Exception ex)
            {
                socket?.Dispose();

                if (_disposed)
                    break;

                _logger.Error(ex, $"Listener @ {_localEndpoint} failed to process client");
            }
        }

        _logger.Info($"Socket listener @ {_localEndpoint} exited");
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _stopCts.Cancel();
        _listener.Stop();

        _stopCts.Dispose();
    }
}
