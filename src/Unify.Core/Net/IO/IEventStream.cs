using Unify.Core.Events;

namespace Unify.Core.Net.IO;

/// <summary>
/// Reads and writes events to a stream
/// </summary>
public interface IEventStream : IDisposable
{
    /// <summary>
    /// Writes an event to the stream
    /// </summary>
    /// <param name="wrapper">The event to be written</param>
    /// <exception cref="IOException"></exception>
    void WriteEvent(EventWrapper wrapper);

    /// <summary>
    /// Reads an event from the stream. Blocks until an event is received
    /// </summary>
    /// <returns></returns>
    /// <exception cref="IOException"></exception>
    EventWrapper ReadEvent();

    /// <summary>
    /// Writes an event to the event stream, using a CancellationToken
    /// to allow cancellation
    /// </summary>
    /// <param name="wrapper">The event to write</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task WriteEventAsync(EventWrapper wrapper, CancellationToken ct = default);
    
    /// <summary>
    /// Reads an event to the stream, using a CancellationToken
    /// to allow cancellation
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<EventWrapper> ReadEventAsync(CancellationToken ct = default);
}
