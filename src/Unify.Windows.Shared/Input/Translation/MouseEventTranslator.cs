using Unify.Core.Common.Input;
using Unify.Core.Common.Input.Types;

namespace Unify.Windows.Shared.Input.Translation;
public sealed class MouseEventTranslator : IMouseEventTranslator
{
    public bool TryTranslateInput(WindowMessage message, User32.MSLLHOOKSTRUCT mouseData, out IMouseInput mouseInput)
    {
        mouseInput = null;

        if (message is WindowMessage.WM_MOUSEMOVE)
        {
            mouseInput = CreateRelativeMove(mouseData);
        }
        else if (message is WindowMessage.WM_LBUTTONDOWN)
        {
            mouseInput = new MouseButtonInput { Pressed = true, Button = MouseButton.Left };
        }
        else if (message is WindowMessage.WM_LBUTTONUP)
        {
            mouseInput = new MouseButtonInput { Pressed = false, Button = MouseButton.Left };
        }
        else if (message is WindowMessage.WM_RBUTTONDOWN)
        {
            mouseInput = new MouseButtonInput { Pressed = true, Button = MouseButton.Right };
        }
        else if (message is WindowMessage.WM_RBUTTONUP)
        {
            mouseInput = new MouseButtonInput { Pressed = false, Button = MouseButton.Right };
        }
        else if (message is WindowMessage.WM_MBUTTONDOWN)
        {
            mouseInput = new MouseButtonInput { Pressed = true, Button = MouseButton.Middle };
        }
        else if (message is WindowMessage.WM_MBUTTONUP)
        {
            mouseInput = new MouseButtonInput { Pressed = false, Button = MouseButton.Middle };
        }else if (message is WindowMessage.WM_MOUSEWHEEL)
        {
            short dir = unchecked((short) ((long)mouseData.mouseData >> 16));

            mouseInput = new MouseScrollInput()
            {
                Direction = dir > 0  ? ScrollDirection.Up : ScrollDirection.Down
            };
        }

        return mouseInput != null;
    }

    private static MouseMoveInput CreateRelativeMove(User32.MSLLHOOKSTRUCT mouseData)
    {
        User32.GetCursorPos(out var oldPosition);

        return new MouseMoveInput()
        {
            X = mouseData.pt.X - oldPosition.X,
            Y = mouseData.pt.Y - oldPosition.Y
        };
    }
}
