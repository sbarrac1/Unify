using Unify.Core.CommonServices.DataMarshal.Data;
using Unify.Core.CommonServices.Streams.Common;

namespace Unify.Core.CommonServices.DataMarshal.Service;
public interface IMarshalService
{
    IDataContainer<T> GetContainer<T>(StreamHeader header);
}
