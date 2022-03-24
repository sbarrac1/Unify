namespace Unify.Core.Common;
public sealed class Repository<TObject> : IRepository<TObject>
{
    private readonly Dictionary<Guid, TObject> _dictionary = new();

    private bool _disposed;

    public void Add(Guid key, TObject value)
    {
        lock (_dictionary)
        {
            ThrowIfDisposed();

            _dictionary.Add(key, value);
        }
    }

    public bool Remove(Guid key)
    {
        lock (_dictionary)
        {
            ThrowIfDisposed();

            return _dictionary.Remove(key);
        }
    }

    public bool TryGet(Guid key, [MaybeNullWhen(false)] out TObject value)
    {
        lock (_dictionary)
        {
            ThrowIfDisposed();

            return _dictionary.TryGetValue(key, out value);
        }
    }

    public IEnumerable<TObject> GetAll()
    {
        lock (_dictionary)
        {
            ThrowIfDisposed();

            return _dictionary.Values.ToArray();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Repository<TObject>));
    }

    public void Dispose()
    {
        lock (_dictionary)
        {
            if (_disposed)
                return;

            _disposed = true;
            _dictionary.Clear();
        }
    }
}
