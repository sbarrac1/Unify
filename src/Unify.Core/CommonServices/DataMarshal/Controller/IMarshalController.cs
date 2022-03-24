using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.CommonServices.DataMarshal.Controller;
public interface IMarshalController
{
    StreamHeader HostObjectAsStream<T>(T obj);
}
