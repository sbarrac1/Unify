using Unify.Core.Events;
using Unify.Core.Net.IO;

namespace Unify.Core.Net.Handshake.Server;

public sealed class UnknownClient : IUnknownClient
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    
    public ClientInfo Info { get; }

    private readonly IEventStream _eventStream;

    public UnknownClient(IEventStream eventStream, ClientInfo clientInfo)
    {
        this.Info = clientInfo;
        _eventStream = eventStream;
    }
    
    public IEventStream AcceptClient()
    {
        try
        {
            _eventStream.WriteEvent(new ServerAcceptedHandshakeEvent());

            return _eventStream;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to accept client {Info.StationName}");
            _eventStream.Dispose();
            throw;
        }
    }

    public void DeclineClient(string reason)
    {
        try
        {
            _eventStream.WriteEvent(new ServerDeclineHandshakeEvent()
            {
                Reason = reason
            });
        }
        catch (Exception)
        {
            //Ignore
        }
        finally
        {
            _eventStream.Dispose();
        }
    }
}