using Unify.Core.Common.Input.Types;
using Unify.Core.Events;

namespace Unify.Core.Net.Formatting.Formatters;
public sealed class SendInputFormatter : IFormatter<SendInputCommand>
{
    public SendInputCommand Read(Stream stream, ObjectPrefix prefix)
    {
        var inputPrefix = ObjectPrefix.Read(stream);
        var input = (IInput)ObjectManager.Instance.GetReader(inputPrefix.ObjectId)(stream, inputPrefix);

        return new SendInputCommand
        {
            Input = input
        };
    }

    public void Write(Stream stream, SendInputCommand value)
    {
        long streamStart = stream.Position;
        stream.Position += ObjectPrefix.Size;

        ObjectManager.Instance.GetWriter(value.Input.GetType())(stream, value.Input);
        long length = stream.Position - (streamStart + ObjectPrefix.Size);
        stream.Position = streamStart;

        ObjectManager.Instance.WritePrefixForType<SendInputCommand>(stream, (int)length);
        stream.Position += length;
    }
}
