using Unify.Core.Common.Input;

namespace Unify.Windows.Shared.Input.Translation;

/// <summary>
/// Converts between <see cref="WinVirtualkey"/> and <see cref="Key"/>
/// </summary>
public interface IWinKeyMap
{
    Key ToGeneric(WinVirtualkey vKey);
    WinVirtualkey ToWin32(Key key);
}
