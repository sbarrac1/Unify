using ProtoBuf;

namespace Unify.Core.Common.Input.Types;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class KeyPressInput : IKeyboardInput
{
    public bool Pressed { get; init; }
    public Key Key { get; init; }
}
