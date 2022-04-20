using ProtoBuf;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(701)]
public sealed class MouseButtonInput : IMouseInput
{
    public MouseButton Button { get; init; }
    public bool Pressed { get; init; }
}
