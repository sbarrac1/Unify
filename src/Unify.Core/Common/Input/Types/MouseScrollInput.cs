using ProtoBuf;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class MouseScrollInput : IMouseInput
{
    public ScrollDirection Direction { get; init; }
}