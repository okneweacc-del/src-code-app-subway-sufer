using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[StaticAccessor("AnimationMixerPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[NativeHeader("Runtime/Animation/Director/AnimationMixerPlayable.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationMixerPlayable.bindings.h")]
public struct AnimationMixerPlayable : IPlayable, IEquatable<AnimationMixerPlayable>
{
	private PlayableHandle m_Handle;

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(AnimationMixerPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
