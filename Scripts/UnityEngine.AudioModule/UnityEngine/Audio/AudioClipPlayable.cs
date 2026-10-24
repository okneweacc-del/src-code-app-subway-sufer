using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[StaticAccessor("AudioClipPlayableBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioClipPlayable.bindings.h")]
[NativeHeader("Modules/Audio/Public/Director/AudioClipPlayable.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
public struct AudioClipPlayable : IPlayable, IEquatable<AudioClipPlayable>
{
	private PlayableHandle m_Handle;

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(AudioClipPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
