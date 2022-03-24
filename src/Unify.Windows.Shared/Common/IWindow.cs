namespace Unify.Windows.Shared.Common;

/// <summary>
/// A native win32 window
/// </summary>
public interface IWindow : IDisposable
{
    event Action<User32.MSG> MessageReceived;

    nint Handle { get; }

    IWindowDispatcher WindowDispatcher { get; }

    bool InvokeRequired();
}