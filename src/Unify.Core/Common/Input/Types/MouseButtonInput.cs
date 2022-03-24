using ProtoBuf;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class MouseButtonInput : IMouseInput
{
    public MouseButton Button { get; init; }
    public bool Pressed { get; init; }
}
