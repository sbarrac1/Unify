using Unify.Core.CommonServices.Clipboard.Controller;
using Unify.Server.Stations.Types;

namespace Unify.Server.Clipboard;
public interface IServerClipboardController : IClipboardController
{
    IStation Owner { get; }

    void SetOwner(IStation station);

    void Initialize();
}
