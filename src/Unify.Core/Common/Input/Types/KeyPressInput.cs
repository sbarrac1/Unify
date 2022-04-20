using ProtoBuf;
using Unify.Core.Net.Formatting;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
[Formattable(720)]
public sealed class KeyPressInput : IKeyboardInput
{
    public bool Pressed { get; init; }
    public Key Key { get; init; }
}
