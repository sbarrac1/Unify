namespace Unify.Windows.Shared.Common;

/// <summary>
/// Invokes methods onto an <see cref="IWindow"/> thread
/// </summary>
public interface IWindowDispatcher : IDisposable
{
    void InvokePost(Action action);
    void InvokeWait(Action action, int timeoutMs = 3000);

    T InvokeReturn<T>(Func<T> func, int timeoutMs = 3000);
}