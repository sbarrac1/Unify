using Unify.Core.Common.Input.Types;

namespace Unify.Windows.Shared.Input.Driver;

/// <summary>
/// Sends input events to the windows input queue
/// </summary>
public interface IWinInputDriver
{
    void SendInput(IInput input);
}
