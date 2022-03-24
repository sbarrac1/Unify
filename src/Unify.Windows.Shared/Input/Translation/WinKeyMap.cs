using Unify.Core.Common.Input;

namespace Unify.Windows.Shared.Input.Translation;
public sealed class WinKeyMap : IWinKeyMap
{
    public Key ToGeneric(WinVirtualkey vKey)
    {
        return (Key)vKey;
    }

    public WinVirtualkey ToWin32(Key key)
    {
        return (WinVirtualkey)key;
    }
}
