using System.ComponentModel;
using System.Runtime.InteropServices;
using Unify.Core.Common.Input;
using Unify.Core.Common.Input.Types;
using Unify.Windows.Shared.Input.Translation;

namespace Unify.Windows.Shared.Input.Driver;

public sealed class WinInputDriver : IWinInputDriver
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IWinKeyMap _keyMap = new WinKeyMap();

    public void SendInput(IInput input)
    {
        if (input is IKeyboardInput keyboardInput)
            SendKeyboardInput(keyboardInput);
        else if (input is IMouseInput mouseInput)
            SendMouseInput(mouseInput);
    }

    private void SendKeyboardInput(IKeyboardInput input)
    {
        if (input is KeyPressInput keyInput)
        {
            if (keyInput.Pressed)
                DoKeyboardInput(0, 0, (ushort)_keyMap.ToWin32(keyInput.Key));
            else
                DoKeyboardInput(User32.KeyEventF.KeyUp, 0, (ushort)keyInput.Key);
        }
    }

    private static void DoKeyboardInput(User32.KeyEventF flags, ushort scan, ushort vkey)
    {
        User32.WinInputStruct mi = new();

        mi.type = 1;

        mi.u = new User32.InputUnion
        {
            ki = new User32.KeyboardInput
            {
                dwFlags = (uint)flags,
                wScan = scan,
                wVk = vkey
            }
        };

        if (User32.SendInput(1, new User32.WinInputStruct[] { mi }, Marshal.SizeOf<User32.WinInputStruct>()) != 1)
        {
            _logger.Error(new Win32Exception(), "SendInput failed");
        }
    }

    private static void SendMouseInput(IMouseInput input)
    {
        if (input is MouseMoveInput relativeMouseEvent)
            DoMouseInput(relativeMouseEvent.X, relativeMouseEvent.Y, User32.MOUSEEVENTF_MOVE, 0);
        else if (input is MouseButtonInput buttonEvent)
        {
            if (buttonEvent.Button == MouseButton.Left)
            {
                if (buttonEvent.Pressed)
                    DoMouseInput(0, 0, User32.MOUSEEVENTF_LEFTDOWN, 0);
                else
                    DoMouseInput(0, 0, User32.MOUSEEVENTF_LEFTUP, 0);
            }
            else if (buttonEvent.Button == MouseButton.Right)
            {
                if (buttonEvent.Pressed)
                    DoMouseInput(0, 0, User32.MOUSEEVENTF_RIGHTDOWN, 0);
                else
                    DoMouseInput(0, 0, User32.MOUSEEVENTF_RIGHTUP, 0);
            }
            else if (buttonEvent.Button == MouseButton.Middle)
            {
                if (buttonEvent.Pressed)
                    DoMouseInput(0, 0, User32.MOUSEEVENTF_MIDDLEDOWN, 0);
                else
                    DoMouseInput(0, 0, User32.MOUSEEVENTF_MIDDLEUP, 0);
            }
        }else if (input is MouseScrollInput scrollInput)
        {
            DoMouseInput(0,0 , User32.MOUSEEVENTF_WHEEL, scrollInput.Direction == ScrollDirection.Down ? -120 : 120);
        }

    }

    private static void DoMouseInput(int x, int y, uint flags, int mouseData)
    {
        User32.WinInputStruct mi = new();

        mi.type = 0;

        mi.u = new User32.InputUnion()
        {
            mi = new User32.MouseInput()
            {
                dx = x,
                dy = y,
                dwFlags = flags,
                dwExtraInfo = 123,
                mouseData = mouseData
            }
        };

        if (User32.SendInput(1, new User32.WinInputStruct[] { mi }, Marshal.SizeOf<User32.WinInputStruct>()) != 1)
        {
            _logger.Error(new Win32Exception(), "SendInput failed");
        }
    }
}
