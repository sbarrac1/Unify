using System.Runtime.InteropServices;
// ReSharper disable All

namespace Unify.Windows.Shared.Native;

public static partial class User32
{
    public delegate nint WndProc(nint hWnd, WindowMessage msg, nint wParam, nint lParam);

    public const int SM_XVIRTUALSCREEN = 76;
    public const int SM_YVIRTUALSCREEN = 77;
    public const int SM_CXVIRTUALSCREEN = 78;
    public const int SM_CYVIRTUALSCREEN = 79;
    
    public const int MOUSEEVENTF_MOVE = 0x0001;
    public const int MOUSEEVENTF_VIRTUALDESK = 0x4000;
    public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
    public const int MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
    public const int MOUSEEVENTF_LEFTDOWN = 0x02;
    public const int MOUSEEVENTF_LEFTUP = 0x04;
    public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    public const int MOUSEEVENTF_RIGHTUP = 0x10;
    public const int MOUSEEVENTF_XDOWN = 0x0080;
    public const int MOUSEEVENTF_XUP = 0x0100;
    public const int MOUSEEVENTF_WHEEL = 0x0800;
    public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    public const int MOUSEEVENTF_MIDDLEUP = 0x0040;

    [Flags]
    public enum KeyEventF
    {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008,
    }

    public struct WinInputStruct
    {
        public int type;
        public InputUnion u;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public MouseInput mi;
        [FieldOffset(0)] public KeyboardInput ki;
        [FieldOffset(0)] public readonly HardwareInput hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public int dx;
        public int dy;
        public int mouseData;
        public uint dwFlags;
        public uint time;
        public nint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public nint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        public readonly uint uMsg;
        public readonly ushort wParamL;
        public readonly ushort wParamH;
    }
    
    public struct MSG {
        public nint hwnd;
        public WindowMessage message;
        public nint wParam;
        public nint lParam;
        public int time;
        public int pt_x;
        public int pt_y;
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
    public class WNDCLASS {
        public int      style;
        public WndProc  lpfnWndProc;
        public int      cbClsExtra = 0;
        public int      cbWndExtra = 0;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string   lpszMenuName = null;
        
        [MarshalAs(UnmanagedType.LPStr)]
        public string   lpszClassName = null;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public class MSLLHOOKSTRUCT
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public UIntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public class KBDLLHOOKSTRUCT
    {
        public WinVirtualkey vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public uint dwExtraInfo;
    }
    
    public struct POINT
    {
        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;
    }
}