namespace Unify.Core.Events;

//Event object IDs - 
//Netevents - 100-199
//ClipboardEvents - 200-299
//FileSysEvents - 300-399
//InputEvents - 400-499
//NetEvents - 500-599
//StreamEvents - 600-699
//IInput objects - 700-799

/// <summary>
/// Marker class for events.
/// </summary>
public interface IEvent
{

    Guid EventId { get; set; }
}

/// <summary>
/// Marker class for request events. Inherits from IEvent
/// </summary>
public interface IRequest : IEvent
{

}

/// <summary>
/// Generic marker class for requests that require
/// a specific return type
/// </summary>
/// <typeparam name="TReply">The return type of the request</typeparam>
public interface IRequest<TReply> : IRequest
{

}