using Unify.Core.Common.Clipboard;
using Unify.Core.CommonServices.Clipboard.Common;
using Unify.Core.CommonServices.Clipboard.Controller;
using Unify.Core.Events.Target;
using Unify.Core.StationHosts;
using Unify.Windows.Shared.Clipboard;

namespace Unify.Windows.Shared.StationHost;

public sealed class WinClipboardController : IStationClipboardController, IDisposable
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private readonly IClipboardService _clipboardService;
    private readonly StationHostConfig _config;
    private readonly IWinClipboard _winClipboard;
    
    public WinClipboardController(IEventTarget eventTarget, IClipboardService clipboardService, StationHostConfig config)
    {
        _clipboardService = clipboardService;
        _config = config;

        if (config.EnableClipboard)
            _winClipboard = new WinClipboard(Window.Create("ISCLIPBOARD"), eventTarget);
        else
            _logger.Warn("Clipboard is disabled");
    }

    public void Dispose()
    {
        _winClipboard?.Dispose();
    }

    public IClipboard GetClipboard()
    {
        if (!_config.EnableClipboard)
            throw new InvalidOperationException("Clipboard is disabled");

        return _winClipboard;
    }

    public void TakeOwnership()
    {
        if (!_config.EnableClipboard)
            throw new InvalidOperationException("Clipboard is disabled");

        _winClipboard.TakeOwnership(_clipboardService.GetClipboard());
    }
}