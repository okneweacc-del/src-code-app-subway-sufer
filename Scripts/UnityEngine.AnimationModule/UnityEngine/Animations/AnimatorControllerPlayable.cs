using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[NativeHeader("Runtime/Animation/RuntimeAnimatorController.h")]
[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
[NativeHeader("Runtime/Animation/Director/AnimatorControllerPlayable.h")]
[StaticAccessor("AnimatorControllerPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
[NativeHeader("Runtime/Animation/ScriptBindings/Animator.bindings.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorControllerPlayable.bindings.h")]
public struct AnimatorControllerPlayable : IPlayable, IEquatable<AnimatorControllerPlayable>
{
	private PlayableHandle m_Handle = PlayableHandle.Null;

	private static readonly AnimatorControllerPlayable m_NullPlayable = new AnimatorControllerPlayable(PlayableHandle.Null);

	internal AnimatorControllerPlayable(PlayableHandle handle)
	{
		SetHandle(handle);
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public void SetHandle(PlayableHandle handle)
	{
		if (m_Handle.IsValid())
		{
			throw new InvalidOperationException("Cannot call IPlayable.SetHandle on an instance that already contains a valid handle.");
		}
		if (handle.IsValid() && !handle.IsPlayableOfType<AnimatorControllerPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimatorControllerPlayable.");
		}
		m_Handle = handle;
	}

	public bool Equals(AnimatorControllerPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
