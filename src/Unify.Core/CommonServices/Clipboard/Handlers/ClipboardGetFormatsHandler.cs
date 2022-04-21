using Unify.Core.Events;

namespace Unify.Core.CommonServices.Clipboard.Controller.Handlers;
public sealed class ClipboardGetFormatsHandler : IRequestHandler<ClipboardGetFormatsRequest, ClipboardGetFormatsReply>
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IClipboardController _clipboardController;

    public ClipboardGetFormatsHandler(IClipboardController clipboardController)
    {
        _clipboardController = clipboardController;
    }

    public ClipboardGetFormatsReply Handle(ClipboardGetFormatsRequest request)
    {
        var formats = _clipboardController.GetClipboard().GetFormats();

        if (_logger.IsTraceEnabled)
            _logger.Trace($"Clipboard formats request -> returning formats {formats}");

        return new ClipboardGetFormatsReply
        {
            Formats = formats,
        };
    }
}
