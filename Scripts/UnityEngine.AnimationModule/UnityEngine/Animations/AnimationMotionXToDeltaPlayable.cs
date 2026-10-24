using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[RequiredByNativeCode]
[NativeHeader("Runtime/Animation/Director/AnimationMotionXToDeltaPlayable.h")]
internal struct AnimationMotionXToDeltaPlayable : IPlayable, IEquatable<AnimationMotionXToDeltaPlayable>
{
	private PlayableHandle m_Handle;

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(AnimationMotionXToDeltaPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
