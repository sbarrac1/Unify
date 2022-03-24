using Unify.Core.CommonServices.DataMarshal.Data;
using Unify.Core.CommonServices.Streams.Common;
using Unify.Core.CommonServices.Streams.Service;

namespace Unify.Core.CommonServices.DataMarshal.Service;

public sealed class MarshalService : IMarshalService
{
    private readonly IStreamsService _streamsService;

    public MarshalService(IStreamsService streamsService)
    {
        _streamsService = streamsService;
    }

    public IDataContainer<T> GetContainer<T>(StreamHeader header)
    {
        var stream = _streamsService.GetStream(header);

        return new DataContainer<T>(stream);
    }
}
