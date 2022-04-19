namespace Unify.Core.Net.Formatting.Formatters;
public interface IFormatter<T>
{
    T Read(Stream stream, ObjectPrefix prefix);

    void Write(Stream stream, T value);
}
