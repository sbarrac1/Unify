using ProtoBuf;
using Unify.Core.Common;
using Unify.Core.Common.Input;
using Unify.Core.Common.Input.Types;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Events;

/// <summary>
/// Sent by the server to tell a station host to perform a user input
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(400)]
public sealed class SendInputCommand : IEvent
{
    public Guid EventId { get; set; }
    public IInput Input { get; init; }
}

/// <summary>
/// Sent by the server to tell the station host
/// to either grab or ungrab the user input
/// </summary>
public sealed class SetGrabStateCommand : IEvent
{
    public Guid EventId { get; set; }
    public bool State { get; init; }
}

/// <summary>
/// Sent by station hosts when a registered hotkey is pressed
/// </summary>
public sealed class HotkeyPressedEvent : IEvent
{
    public Guid EventId { get; set; }
    public Hotkey Hotkey { get; init; }
}


/// <summary>
/// Sent by the server to tell the station host to
/// register a hotkey callback
/// </summary>
public sealed class RegisterHotkeyCommand : IEvent
{
    public Guid EventId { get; set; }
    public Hotkey Hotkey { get; init; }
}

/// <summary>
/// Sent by the server to tell the station host to
/// unregister a hotkey callback
/// </summary>
public sealed class UnregisterHotkeyCommand : IEvent
{
    public Guid EventId { get; set; }
    public Hotkey Hotkey { get; init; }
}


/// <summary>
/// Sent by station hosts when a user input is received
/// </summary>
public sealed class InputReceivedEvent : IEvent
{
    public Guid EventId { get; set; }
    public IInput Input { get; init; }
}


/// <summary>
/// Sent by station hosts when the cursor hits a side of the screen
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(401)]
public sealed class SideHitEvent : IEvent
{
    public Side Side { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public Guid EventId { get; set; }
}