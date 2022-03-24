using Unify.Core.Common.Input.Types;

namespace Unify.Windows.Shared.Input.Translation;

/// <summary>
/// Converts a native windows mouse event messages
/// into a generic <see cref="IInput"/> object
/// </summary>
public interface IMouseEventTranslator
{
    bool TryTranslateInput(WindowMessage message, User32.MSLLHOOKSTRUCT mouseData, out IMouseInput mouseInput);
}
