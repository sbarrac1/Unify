using Unify.Core.Common.Clipboard;
using Unify.Server.Common;
using Unify.Server.Config;
using Unify.Server.Stations;
using Unify.Server.Stations.Types;

namespace Unify.Server.Clipboard;
public sealed class ServerClipboardController : IServerClipboardController
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IStationRepository _stationRepository;
    private readonly ServerContext _context;
    private readonly IServerSettingsService _settingsService;

    public IStation Owner { get; private set; }

    public ServerClipboardController(IStationRepository stationRepository,
        ServerContext context,
        IServerSettingsService settingsService)
    {
        _stationRepository = stationRepository;
        _context = context;
        _settingsService = settingsService;
    }

    public IClipboard GetClipboard()
    {
        if (!_settingsService.EnableClipboard)
            throw new InvalidOperationException("Clipboard is disabled!");

        return this.Owner.ClipboardService.GetClipboard();
    }

    public void Initialize()
    {
        this.Owner = _stationRepository.Primary;
    }

    public void SetOwner(IStation station)
    {
        lock (_context.SyncObject)
        {
            _logger.Info($"Switching clipboard owner from {Owner.Name} -> {station.Name}");
            this.Owner = station;

            foreach (var target in _stationRepository.Stations)
            {
                if (target == station)
                    continue;

                target.ClipboardService.TakeOwnership();
            }
        }
    }
}
