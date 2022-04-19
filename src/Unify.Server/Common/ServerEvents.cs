using Unify.Core.Events;

namespace Unify.Server.Common;

public sealed class StationAttachedEvent : IEvent
{
    public Guid EventId { get; set; }
}
public sealed class StationDetachedEvent : IEvent
{
    public Guid EventId { get; set; }
}