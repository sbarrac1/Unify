namespace Unify.Core.Net.Formatting;

/// <summary>
/// Contains utility for reading/writing objects to/from a stream
/// </summary>
public interface IObjectManager
{
    /// <summary>
    /// Sets up formatters and maps object types
    /// </summary>
    void Setup();

    ObjectReadDelegate GetReader(int objectId);
    ObjectWriteDelegate GetWriter(int objectId);

    ObjectReadDelegate GetReader(Type objectType);
    ObjectWriteDelegate GetWriter(Type objectType);

    void WritePrefixForType<T>(Stream stream, int length);
}
