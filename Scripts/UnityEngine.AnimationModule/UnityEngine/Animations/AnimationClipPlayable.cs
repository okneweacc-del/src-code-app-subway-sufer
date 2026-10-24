using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[NativeHeader("Runtime/Animation/Director/AnimationClipPlayable.h")]
[RequiredByNativeCode]
[StaticAccessor("AnimationClipPlayableBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationClipPlayable.bindings.h")]
public struct AnimationClipPlayable : IPlayable, IEquatable<AnimationClipPlayable>
{
	private PlayableHandle m_Handle;

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(AnimationClipPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
