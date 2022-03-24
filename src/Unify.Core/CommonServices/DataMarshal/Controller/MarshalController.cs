using ProtoBuf;
using Unify.Core.CommonServices.Streams.Common;
using Unify.Core.CommonServices.Streams.Controller;

namespace Unify.Core.CommonServices.DataMarshal.Controller;
public sealed class MarshalController : IMarshalController
{
    private readonly IStreamsController _streamsController;

    public MarshalController(IStreamsController streamsController)
    {
        _streamsController = streamsController;
    }

    public StreamHeader HostObjectAsStream<T>(T obj)
    {
        var ms = new MemoryStream();
        Serializer.Serialize(ms, obj);
        ms.Position = 0;

        return _streamsController.HostStream(ms);
    }
}
