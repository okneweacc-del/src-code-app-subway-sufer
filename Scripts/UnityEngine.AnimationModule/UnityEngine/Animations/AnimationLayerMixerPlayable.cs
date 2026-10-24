using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[RequiredByNativeCode]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationLayerMixerPlayable.bindings.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[NativeHeader("Runtime/Animation/Director/AnimationLayerMixerPlayable.h")]
[StaticAccessor("AnimationLayerMixerPlayableBindings", StaticAccessorType.DoubleColon)]
public struct AnimationLayerMixerPlayable : IPlayable, IEquatable<AnimationLayerMixerPlayable>
{
	private PlayableHandle m_Handle;

	private static readonly AnimationLayerMixerPlayable m_NullPlayable = new AnimationLayerMixerPlayable(PlayableHandle.Null);

	internal AnimationLayerMixerPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AnimationLayerMixerPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimationLayerMixerPlayable.");
		}
		m_Handle = handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(AnimationLayerMixerPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
