using ProtoBuf;
using Unify.Core.Common;
using Unify.Core.Common.Input;
using Unify.Core.Common.Input.Types;

namespace Unify.Core.Events;

/// <summary>
/// Sent by the server to tell a station host to perform a user input
/// </summary>
[ProtoContract]
public sealed class SendInputCommand : IEvent
{
    [ProtoMember(1)]
    public IInput Input { get; init; }
}

/// <summary>
/// Sent by the server to tell the station host
/// to either grab or ungrab the user input
/// </summary>
public sealed class SetGrabStateCommand : IEvent
{
    public bool State { get; init; }
}

/// <summary>
/// Sent by station hosts when a registered hotkey is pressed
/// </summary>
public sealed class HotkeyPressedEvent : IEvent
{
    public Hotkey Hotkey { get; init; }
}


/// <summary>
/// Sent by the server to tell the station host to
/// register a hotkey callback
/// </summary>
public sealed class RegisterHotkeyCommand : IEvent
{
    public Hotkey Hotkey { get; init; }
}

/// <summary>
/// Sent by the server to tell the station host to
/// unregister a hotkey callback
/// </summary>
public sealed class UnregisterHotkeyCommand : IEvent
{
    public Hotkey Hotkey { get; init; }
}


/// <summary>
/// Sent by station hosts when a user input is received
/// </summary>
public sealed class InputReceivedEvent : IEvent
{
    public IInput Input { get; init; }
}


/// <summary>
/// Sent by station hosts when the cursor hits a side of the screen
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class SideHitEvent : IEvent
{
    public Side Side { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
}