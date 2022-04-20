namespace Unify.Core.Net.Formatting.Formatters;

/// <summary>
/// Serializes and deserializes objects of type <typeparamref name="T"/>
/// to/from a stream
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFormatter<T>
{
    /// <summary>
    /// Deserializes an object from the stream.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="prefix">The <seealso cref="ObjectPrefix>"/>That contains information
    /// about the serialized object</param>
    /// <returns></returns>
    T Read(Stream stream, ObjectPrefix prefix);

    /// <summary>
    /// Serializes the object to the stream
    /// </summary>
    /// <param name="stream">The stream to serialize the object to</param>
    /// <param name="value">The object to serialize</param>
    void Write(Stream stream, T value);
}
