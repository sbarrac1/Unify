using System.Drawing;
using Unify.Core.Common;
using Unify.Core.Common.Input;
using Unify.Core.CommonServices.InputHook.Controller;
using Unify.Core.Events;
using Unify.Core.Events.Target;
using Unify.Core.StationHosts;
using Unify.Windows.Shared.Input;
using Unify.Windows.Shared.Input.Hooks;
using Unify.Windows.Shared.Input.Translation;

namespace Unify.Windows.Shared.StationHost;

public sealed class WinInputHookController : IInputHookController, IDisposable
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private readonly IEventTarget _eventTarget;
    private readonly StationHostConfig _config;
    private readonly IWindow _window;
    private readonly LLhookBase _mouseHook;
    private readonly LLhookBase _keyboardHook;
    private readonly IWinKeyMap _keyMap = new WinKeyMap();
    private readonly KeyboardState _keyboardState = new();

    private readonly IMouseEventTranslator _mouseEventTranslator = new MouseEventTranslator();
    private readonly IKeyboardEventTranslator _keyboardEventTranslator = new KeyboardEventTranslator(new WinKeyMap());

    private Rectangle _bounds;
    private bool _grabbed;

    private readonly List<Hotkey> _hotkeys = new();
    
    public WinInputHookController(IEventTarget eventTarget, StationHostConfig config)
    {
        _eventTarget = eventTarget;
        _config = config;
        _window = Window.Create("IS_HOOK_WND");

        if (config.EnableHooks)
        {
            _mouseHook = new LLMouseHook(_window, OnMouseEvent);
            _keyboardHook = new LLKeyboardHook(_window, OnKeyboardEvent);
        }
        else
        {
            _logger.Warn("Hooks are disabled");
        }

        _bounds = GetVirtualBounds();
        _window.MessageReceived += WindowOnMessageReceived;
    }

    private void WindowOnMessageReceived(User32.MSG obj)
    {
        if(obj.message == WindowMessage.WM_DISPLAYCHANGE)
        {
            _logger.Info("Display bounds changed!");

            _bounds = GetVirtualBounds();
        }
    }

    private bool OnMouseEvent(WindowMessage message, User32.MSLLHOOKSTRUCT mouseData)
    {
        if (_grabbed)
        {
            if (!_mouseEventTranslator.TryTranslateInput(message, mouseData, out var input))
            {
                _logger.Error($"Could not translate mouse event type {message}");
                return false;
            }
            _eventTarget.PostEvent(new InputReceivedEvent { Input = input });

            return false;
        }
        else
        {
            CheckForSideHit(mouseData);
            return true;
        }
    }

    private bool OnKeyboardEvent(WindowMessage message, User32.KBDLLHOOKSTRUCT keyboardData)
    {
        _keyboardState.ProcessMessage(message, keyboardData);
        
        lock (_hotkeys)
                    {
                        foreach (var hotkey in _hotkeys)
                        {
                            var vKey = _keyMap.ToWin32(hotkey.Key);

                            if (vKey == keyboardData.vkCode)
                            { 
                                if (_keyboardState.CheckModifiers(hotkey.Modifiers))
                                {
                                    _logger.Trace($"Win -> pressed hotkey {hotkey}");
                                    
                                    _eventTarget.PostEvent(new HotkeyPressedEvent()
                                    {
                                        Hotkey = hotkey
                                    });
                                }
                            }
                        }
                    }
        
        if (_grabbed)
        {
            if(!_keyboardEventTranslator.TryTranslateInput(message, keyboardData, out var input))
            {
                _logger.Error($"Could not translate keyboard event type {message}");
                return false;
            }

            _eventTarget.PostEvent(new InputReceivedEvent { Input = input });
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetGrabState(bool grabState)
    {
        _grabbed = grabState;
    }

    public void RegisterHotkey(Hotkey hk)
    {
        lock (_hotkeys)
        {
            _hotkeys.Add(hk);
            
            _logger.Info($"Registered hotkey {hk}");
        }
    }

    public void UnregisterHotkey(Hotkey hk)
    {
        lock (_hotkeys)
        {
            if (_hotkeys.Remove(hk))
            {
                _logger.Info($"Unregistered hotkey {hk}");
            }
            else
            {
                _logger.Warn($"Hotkey {hk} not found");
            }
        }
    }
    
    private void CheckForSideHit(User32.MSLLHOOKSTRUCT mouseData)
    {
        Side side = Side.None;

        if (_bounds.Left >= mouseData.pt.X)
            side = Side.Left;
        else if (_bounds.Right - 2 < mouseData.pt.X)
            side = Side.Right;
        else if (_bounds.Top >= mouseData.pt.Y)
            side = Side.Top;
        else if (_bounds.Bottom - 2 < mouseData.pt.Y)
            side = Side.Bottom;

        if (side != Side.None)
        {
            _eventTarget.PostEvent(new SideHitEvent()
            {
                Side = side,
                X = mouseData.pt.X,
                Y = mouseData.pt.Y
            });
        }
    }
    
    private Rectangle GetVirtualBounds()
    {
        return _window.WindowDispatcher.InvokeReturn(() =>
        {
            return new Rectangle(User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN),
                User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN),
                User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN),
                User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN));
        });
    }

    public void Dispose()
    {
        _mouseHook?.Dispose();
        _keyboardHook?.Dispose();
        _window.Dispose();
    }
}