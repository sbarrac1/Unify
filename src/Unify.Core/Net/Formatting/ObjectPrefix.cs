using System.Buffers.Binary;
using System.Diagnostics;

namespace Unify.Core.Net.Formatting;

/// <summary>
/// Contains the type and length of the next object in the stream
/// </summary>
public readonly struct ObjectPrefix
{
    /// <summary>
    /// The constant length of a <seealso cref="ObjectPrefix"/>
    /// </summary>
    public const int Size = 6;

    public ObjectPrefix(short objectId, int objectLength)
    {
        ObjectId = objectId;
        ObjectLength = objectLength;
    }

    public void Write(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[Size];
        BinaryPrimitives.WriteInt16LittleEndian(buffer, ObjectId);
        BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(2), ObjectLength);

        stream.Write(buffer);
    }

    public static ObjectPrefix Read(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[Size];
        stream.ReadExact(buffer, Size);

        short objectId = BinaryPrimitives.ReadInt16LittleEndian(buffer);

        if (objectId <= 0)
            throw new IOException("Invalid object ID");

        int objectLength = BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(2));

        if (objectLength < 0 || objectLength > 300*1024)
            throw new IOException($"Invalid object length ({objectLength})");

        return new ObjectPrefix(objectId, objectLength);
    }

    public short ObjectId { get; }
    public int ObjectLength { get; }
}
