using Unify.Core.Common.Input.Types;

namespace Unify.Windows.Shared.Input.Translation;

/// <summary>
/// Converts a native windows keyboard event messages
/// into a generic <see cref="IInput"/> object
/// </summary>
public interface IKeyboardEventTranslator
{
    bool TryTranslateInput(WindowMessage message, User32.KBDLLHOOKSTRUCT keyboardData, out IKeyboardInput keyboardInput);
}
