using Unify.Core.CommonServices.Clipboard.Common;

namespace Unify.Core.CommonServices.Clipboard.StationService;

public interface IStationClipboardService : IClipboardService
{
    void TakeOwnership();
}
