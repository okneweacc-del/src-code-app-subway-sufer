using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioMixerPlayable.bindings.h")]
[RequiredByNativeCode]
[NativeHeader("Modules/Audio/Public/Director/AudioMixerPlayable.h")]
[StaticAccessor("AudioMixerPlayableBindings", StaticAccessorType.DoubleColon)]
public struct AudioMixerPlayable : IPlayable, IEquatable<AudioMixerPlayable>
{
	private PlayableHandle m_Handle;

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(AudioMixerPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
