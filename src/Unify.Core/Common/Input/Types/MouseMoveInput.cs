using ProtoBuf;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class MouseMoveInput : IMouseInput
{
    public int X { get; init; }
    public int Y { get; init; }

    public override string ToString()
    {
        return $"MOVE {X}:{Y}";
    }
}
