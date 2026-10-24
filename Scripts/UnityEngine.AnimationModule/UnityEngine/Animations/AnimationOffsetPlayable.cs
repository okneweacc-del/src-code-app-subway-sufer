using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[StaticAccessor("AnimationOffsetPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationOffsetPlayable.bindings.h")]
[NativeHeader("Runtime/Animation/Director/AnimationOffsetPlayable.h")]
internal struct AnimationOffsetPlayable : IPlayable, IEquatable<AnimationOffsetPlayable>
{
	private PlayableHandle m_Handle;

	private static readonly AnimationOffsetPlayable m_NullPlayable = new AnimationOffsetPlayable(PlayableHandle.Null);

	internal AnimationOffsetPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AnimationOffsetPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimationOffsetPlayable.");
		}
		m_Handle = handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(AnimationOffsetPlayable other)
	{
		return Equals(other.GetHandle());
	}
}
