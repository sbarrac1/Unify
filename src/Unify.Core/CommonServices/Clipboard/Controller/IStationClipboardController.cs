namespace Unify.Core.CommonServices.Clipboard.Controller;

public interface IStationClipboardController : IClipboardController
{
    void TakeOwnership();
}