using ProtoBuf;

namespace Unify.Core.Common.Input.Types;

[ProtoContract]
[ProtoInclude(1, typeof(KeyPressInput))]
public interface IKeyboardInput : IInput { }
