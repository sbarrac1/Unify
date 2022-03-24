using System.ComponentModel;

namespace Unify.Windows.Shared.Common;

public sealed class Window : IWindow
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public event Action<User32.MSG> MessageReceived
    {
        add
        {
            lock (_messageReceivedLock)
                _messageReceived += value;
        }

        remove
        {
            lock (_messageReceivedLock)
                if(_messageReceived != null) 
                    _messageReceived -= value;
        }
    }

    private Action<User32.MSG> _messageReceived = null;

    public nint Handle { get; private set; }
    public IWindowDispatcher WindowDispatcher
    {
        get
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Window));

            return _dispatcher;
        }
    }

    public static Window Create(string windowName)
    {
        var instance = new Window(windowName);

        if (!instance._creationWaitHandle.Wait(3000))
            throw new TimeoutException();

        if (instance._creationException != null)
            throw new AggregateException($@"Failed to create window", instance._creationException);

        return instance;
    }

    private readonly ManualResetEventSlim _creationWaitHandle = new();
    private readonly ManualResetEventSlim _threadExitedWaitHandle = new();

    private Exception _creationException;

    private readonly string _windowClassName;
    private readonly string _windowName;
    private readonly User32.WndProc _wndProc;
    private readonly object _messageReceivedLock = new();
    private WindowDispatcher _dispatcher;
    private uint _windowThreadId;
    private bool _disposed;

    private Window(string windowName)
    {
        _windowName = windowName;
        _windowClassName = $"{_windowName}_cls_{new Random().Next()}";
        _wndProc = OnMessage;

        var wndThread = new Thread(WindowThreadInit);

#pragma warning disable CA1416
        wndThread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416

        wndThread.Start();
    }

    private void WindowThreadInit()
    {
        _logger.Trace($"Created window thread for window {_windowName}");

        try
        {
            CreateClass();
            Handle = CreateWindow();

            _dispatcher = new WindowDispatcher(this);
            Ole32.OleInitialize(0);
        }
        catch (Exception ex)
        {
            _creationException = ex;
        }
        finally
        {
            _creationWaitHandle.Set();
        }

        User32.MSG msg = default;

        while (User32.GetMessage(ref msg, 0, 0, 0) > 0)
        {
            User32.DispatchMessage(ref msg);
        }

        _dispatcher.Dispose();
        _logger.Trace($"Exited window thread for window {_windowName}");
        _threadExitedWaitHandle.Set();
    }

    private void CreateClass()
    {
        User32.WNDCLASS cls = new()
        {
            hInstance = Win32Helpers.GetHInstance(),
            lpfnWndProc = _wndProc,
            lpszClassName = _windowClassName
        };

        if (User32.RegisterClass(cls) == 0)
        {
            User32.UnregisterClass(_windowClassName, Win32Helpers.GetHInstance());
            throw new Win32Exception();
        }
    }

    private nint CreateWindow()
    {
        nint handle = User32.CreateWindowEx(0, _windowClassName, _windowName,
            0, 0, 0, 1, 1, 0, 0, Win32Helpers.GetHInstance(), null);

        return handle == 0  ? throw new Win32Exception() : handle;
    }

    private nint OnMessage(nint hWnd, WindowMessage message, nint wParam, nint lParam)
    {
        _windowThreadId = Kernel32.GetCurrentThreadId();
        

        lock (_messageReceivedLock)
        {
            _messageReceived?.Invoke(new User32.MSG()
            {
                hwnd = hWnd,
                message = message,
                wParam = wParam,
                lParam = lParam
            });
        }

        _dispatcher?.ProcessMessage(message);

        return User32.DefWindowProc(hWnd, message, wParam, lParam);
    }

    public bool InvokeRequired()
    {
        return Kernel32.GetCurrentThreadId() != _windowThreadId;
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        
        _disposed = true;
        
        User32.PostMessage(Handle, WindowMessage.WM_QUIT, 0, 0);
        //Todo
        if(!_threadExitedWaitHandle.Wait(7500))
            _logger.Warn($"Timed out waiting for window {_windowName} to close");
    }
}