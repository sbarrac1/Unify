namespace Unify.Core.Common;

/// <summary>
/// A repository of GUID identifiable objects of type <see cref="TObject"/> that
/// can be accessed from within the current context
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IRepository<TObject>
{
    void Add(Guid key, TObject value);
    bool Remove(Guid key);
    bool TryGet(Guid key, [MaybeNullWhen(false)] out TObject value);

    IEnumerable<TObject> GetAll();
}
