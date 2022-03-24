using Unify.Core.Common.Input;
using Unify.Core.Events;

namespace Unify.Core.CommonServices.InputHook.Service;

/// <summary>
/// Hooks user mouse and keyboard input. Allows discarding and forwarding of user input events.
/// </summary>
public interface IInputHookService
{
    /// <summary>
    /// If the value is true, user events will be discarded, and an <see cref="InputReceivedEvent"/>
    /// will be sent to the server on each user input. If false, user input will not be discarded and 
    /// input events will not be sent to the server. Use <see cref="SetGrabState(bool)"/> to set this value
    /// </summary>
    bool InputGrabbed { get; }
    
    /// <summary>
    /// Sets the grab state of the input hook. If <paramref name="state"/> is true,
    /// user events will be discarded, and an <see cref="InputReceivedEvent"/>
    /// will be sent to the server on each user input. If <paramref name="state"/> is false, user input will not be discarded and 
    /// input events will not be sent to the server.
    /// </summary>
    /// <param name="state"></param>
    void SetGrabState(bool state);

    /// <summary>
    /// Registers a keyboard callback. Any time the given hotkey is pressed,
    /// a <see cref="HotkeyPressedEvent"/> will be received by the server
    /// </summary>
    /// <param name="hk"></param>
    void RegisterHotkey(Hotkey hk);
    void UnregisterHotkey(Hotkey hk);
}
