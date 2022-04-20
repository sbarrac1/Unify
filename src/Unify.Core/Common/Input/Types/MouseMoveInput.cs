using ProtoBuf;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(700)]
public sealed class MouseMoveInput : IMouseInput
{
    public int X { get; init; }
    public int Y { get; init; }

    public override string ToString()
    {
        return $"MOVE {X}:{Y}";
    }
}
