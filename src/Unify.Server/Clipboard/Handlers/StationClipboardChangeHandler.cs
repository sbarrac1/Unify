using Unify.Core.Events;
using Unify.Core.Events.Handlers;
using Unify.Server.Stations.Types;

namespace Unify.Server.Clipboard.Handlers;
public sealed class StationClipboardChangeHandler : IEventHandler<ClipboardChangedEvent>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IStation _sender;
    private readonly IServerClipboardController _clipboardController;

    public StationClipboardChangeHandler(IStation sender, IServerClipboardController clipboardController)
    {
        _sender = sender;
        _clipboardController = clipboardController;
    }

    public void Handle(ClipboardChangedEvent evt)
    {
        _logger.Info($"{_sender.Name} -> Clipboard changed");

        _clipboardController.SetOwner(_sender);
    }
}
