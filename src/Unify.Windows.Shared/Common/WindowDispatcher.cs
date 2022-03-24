using System.Collections.Concurrent;
using System.ComponentModel;

namespace Unify.Windows.Shared.Common;

public sealed class WindowDispatcher : IWindowDispatcher
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IWindow _context;
    private readonly object _lockObject = new();
    private bool _disposed;

    private readonly ConcurrentQueue<Action> _invokeQueue = new();

    public WindowDispatcher(IWindow context)
    {
        _context = context;
    }

    public void InvokePost(Action action)
    {
        lock (_lockObject)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowDispatcher));

            if (!_context.InvokeRequired())
            {
                action();
                return;
            }

            _invokeQueue.Enqueue(action);

            if (!User32.PostMessage(_context.Handle, WindowMessage.WM_NULL, 0, 0))
                throw new Win32Exception();
        }
    }

    public void InvokeWait(Action action, int timeoutMs = 15000)
    {
        if (!_context.InvokeRequired())
        {
            action();
            return;
        }

        ManualResetEventSlim waitEvent = new(false);
        Exception thrownException = null;

        InvokePost(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }
            finally
            {
                waitEvent.Set();
            }
        });

        if (!waitEvent.Wait(timeoutMs))
            throw new TimeoutException("Timed out waiting for invoked window delegate");

        if (thrownException != null)
            throw new AggregateException($@"Invoked action threw exception", thrownException);
    }

    public T InvokeReturn<T>(Func<T> func, int timeoutMs = 3000)
    {
        if (!_context.InvokeRequired())
            return func();

        T value = default;

        InvokeWait(() =>
        {
            value = func();
        });

        return value;
    }

    public void ProcessMessage(WindowMessage message)
    {
        while (_invokeQueue.TryDequeue(out var invoke))
        {
            try
            {
                invoke();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, @"Unhandled exception in invoked method");
            }
        }

    }

    public void Dispose()
    {
        lock (_lockObject)
        {
            _disposed = true;
            _invokeQueue.Clear();
        }
    }
}