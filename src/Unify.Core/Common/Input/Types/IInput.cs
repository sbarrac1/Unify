using ProtoBuf;

namespace Unify.Core.Common.Input.Types;

[ProtoContract]
[ProtoInclude(1, typeof(IMouseInput))]
[ProtoInclude(2, typeof(IKeyboardInput))]
public interface IInput
{

}
