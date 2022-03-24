using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.CommonServices.Streams.Service;

/// <summary>
/// Constructs Streams from a <see cref="StreamHeader"/>
/// </summary>
public interface IStreamsService
{
    Stream GetStream(StreamHeader header);
}
