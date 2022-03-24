using ProtoBuf;

namespace Unify.Core.Common.Input.Types;

[ProtoContract]
[ProtoInclude(1, typeof(MouseMoveInput))]
[ProtoInclude(2, typeof(MouseButtonInput))]
[ProtoInclude(3, typeof(MouseScrollInput))]
public interface IMouseInput : IInput { }