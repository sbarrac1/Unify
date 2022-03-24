using Unify.Core.Common.Input.Types;

namespace Unify.Windows.Shared.Input.Translation;

public class KeyboardEventTranslator : IKeyboardEventTranslator
{
    private readonly IWinKeyMap _keyMap;

    public KeyboardEventTranslator(IWinKeyMap keyMap)
    {
        _keyMap = keyMap;
    }

    public bool TryTranslateInput(WindowMessage message, User32.KBDLLHOOKSTRUCT keyboardData, out IKeyboardInput keyboardInput)
    {

        keyboardInput = new KeyPressInput()
        {
            Key = _keyMap.ToGeneric((WinVirtualkey)keyboardData.vkCode),
            Pressed = message is WindowMessage.WM_KEYDOWN or WindowMessage.WM_SYSKEYDOWN
        };

        return true;
    }
}
