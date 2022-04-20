using ProtoBuf;

namespace Unify.Core.Net.Formatting.Formatters;

/// <summary>
/// Default formatter used when no custom formatter is defined for a type
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ProtoFormatter<T> : IFormatter<T>
{
    public T Read(Stream stream, ObjectPrefix prefix)
    {
        var obj =  Serializer.Deserialize<T>(stream, length: prefix.ObjectLength);

        if (obj == null)
            throw new IOException("End of stream");

        return obj;
    }

    public void Write(Stream stream, T value)
    {
        long originalPosition = stream.Position;
        stream.Position += ObjectPrefix.Size;

        Serializer.Serialize<T>(stream, value);
        long length = stream.Position - (originalPosition + ObjectPrefix.Size);

        stream.Position = originalPosition;

        ObjectManager.Instance.WritePrefixForType<T>(stream, (int)length);
        stream.Position += length;
    }
}
