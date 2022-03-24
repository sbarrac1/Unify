using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.CommonServices.Streams.Controller;
public interface IStreamsController
{
    StreamHeader HostStream(Stream stream);
}
