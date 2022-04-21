using Unify.Core.CommonServices.Streams.Controller;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.Clipboard.Controller.Handlers;
public sealed class ClipboardGetTextHandler : IRequestHandler<ClipboardGetTextRequest, ClipboardGetTextReply>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IClipboardController _clipboardController;
    private readonly IStreamsController _streamsController;

    public ClipboardGetTextHandler(IClipboardController clipboardController,
        IStreamsController streamsController)
    {
        _clipboardController = clipboardController;
        _streamsController = streamsController;
    }

    public ClipboardGetTextReply Handle(ClipboardGetTextRequest request)
    {
        var textContainer = _clipboardController.GetClipboard().GetText();
        var textStreamHeader = _streamsController.HostStream(textContainer.GetStream());

        if (_logger.IsTraceEnabled)
            _logger.Trace($"Clipboard text request -> returned stream header {textStreamHeader.StreamId}");

        return new ClipboardGetTextReply
        {
            Header = textStreamHeader
        };
    }
}
