using ProtoBuf;

namespace Unify.Core.CommonServices.DataMarshal.Data;
public sealed class DataContainer<TObject> : IDataContainer<TObject>
{
    private Stream _stream;
    private TObject _object;

    private readonly object _lockObject = new();
    private bool _disposed;

    public DataContainer(Stream stream)
    {
        _stream = stream;
    }

    public DataContainer(TObject obj)
    {
        _object = obj;
    }

    public TObject GetObject()
    {
        lock (_lockObject)
        {
            if (_object == null)
                _object = InternalGetObject();

            return _object;
        }
    }

    public Stream GetStream()
    {
        lock (_lockObject)
        {
            if (_stream == null)
                _stream = InternalGetStream();

            return _stream;
        }
    }

    private TObject InternalGetObject()
    {
        if (_stream == null)
            throw new NullReferenceException(nameof(_stream));

        lock (_stream)
        {
            _stream.Position = 0;
            return Serializer.Deserialize<TObject>(_stream);
        }
    }

    private Stream InternalGetStream()
    {
        if(_object == null)
            throw new NullReferenceException(nameof(_object));

        var ms = new MemoryStream();
        Serializer.Serialize(ms, _object);
        ms.Position = 0;
        return ms;
    }

    public void Dispose()
    {
        lock (_lockObject)
        {
            if (_disposed)
                return;

            _disposed = true;

            _stream.Dispose();

            if(_object is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
