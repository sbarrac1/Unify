using ProtoBuf;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(702)]
public sealed class MouseScrollInput : IMouseInput
{
    public ScrollDirection Direction { get; init; }
}