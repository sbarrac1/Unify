namespace Unify.Core.Net.Formatting;
public interface IObjectManager
{
    void Setup();

    ObjectReadDelegate GetReader(int objectId);
    ObjectWriteDelegate GetWriter(int objectId);

    ObjectReadDelegate GetReader(Type objectType);
    ObjectWriteDelegate GetWriter(Type objectType);

    void WritePrefixForType<T>(Stream stream, int length);
}
