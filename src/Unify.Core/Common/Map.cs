using System.Collections;

namespace Unify.Core.Common;
public sealed class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
{
    private readonly Dictionary<T1, T2> _t1ToT2 = new();
    private readonly Dictionary<T2, T1> _t2ToT1 = new();

    public void Add(T1 t1, T2 t2)
    {
        _t1ToT2.Add(t1, t2);
        _t2ToT1.Add(t2, t1);
    }

    public void Add(T2 t2, T1 t1) => Add(t1, t2);

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<T1, T2>>)_t1ToT2).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_t1ToT2).GetEnumerator();
    }

    public T1 this[T2 t2]
    {
        get
        {
            return _t2ToT1[t2];
        }
    }

    public T2 this[T1 t1]
    {
        get
        {
            return _t1ToT2[t1];
        }
    }
}
