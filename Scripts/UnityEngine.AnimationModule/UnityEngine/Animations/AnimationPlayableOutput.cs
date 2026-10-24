using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[NativeHeader("Runtime/Animation/Animator.h")]
[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[StaticAccessor("AnimationPlayableOutputBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Animation/Director/AnimationPlayableOutput.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationPlayableOutput.bindings.h")]
public struct AnimationPlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;
}
