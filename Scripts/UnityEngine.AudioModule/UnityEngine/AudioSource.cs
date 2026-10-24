using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
public sealed class AudioSource : AudioBehaviour
{
	internal AudioSourceExtension spatializerExtension;

	internal AudioSourceExtension ambisonicExtension;

	public extern float volume
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern float pitch
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern float time
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[ThreadAndSerializationSafe]
	public extern int timeSamples
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[ThreadAndSerializationSafe]
	public extern AudioClip clip
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool isPlaying
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool loop
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool playOnAwake
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern float spatialBlend
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	internal extern bool spatializeInternal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public bool spatialize => spatializeInternal;

	public extern bool bypassEffects
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int priority
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern float minDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern float maxDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern AudioRolloffMode rolloffMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Play([DefaultValue("0")] ulong delay);

	[ExcludeFromDocs]
	public void Play()
	{
		ulong delay = 0uL;
		Play(delay);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Stop();

	public void Pause()
	{
		INTERNAL_CALL_Pause(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Pause(AudioSource self);

	public void UnPause()
	{
		INTERNAL_CALL_UnPause(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UnPause(AudioSource self);

	public void PlayOneShot(AudioClip clip, [DefaultValue("1.0F")] float volumeScale)
	{
		if (clip != null && clip.ambisonic)
		{
			AudioSourceExtension audioSourceExtension = AudioExtensionManager.AddAmbisonicDecoderExtension(this);
			if (audioSourceExtension != null)
			{
				AudioExtensionManager.GetReadyToPlay(audioSourceExtension);
			}
		}
		PlayOneShotHelper(clip, volumeScale);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void PlayOneShotHelper(AudioClip clip, [DefaultValue("1.0F")] float volumeScale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern int GetNumExtensionProperties();

	internal PropertyName ReadExtensionName(int sourceIndex)
	{
		INTERNAL_CALL_ReadExtensionName(this, sourceIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionName(AudioSource self, int sourceIndex, out PropertyName value);

	internal PropertyName ReadExtensionPropertyName(int sourceIndex)
	{
		INTERNAL_CALL_ReadExtensionPropertyName(this, sourceIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionPropertyName(AudioSource self, int sourceIndex, out PropertyName value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern float ReadExtensionPropertyValue(int sourceIndex);

	internal void ClearExtensionProperties(PropertyName extensionName)
	{
		INTERNAL_CALL_ClearExtensionProperties(this, ref extensionName);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ClearExtensionProperties(AudioSource self, ref PropertyName extensionName);

	internal AudioSourceExtension AddSpatializerExtension(Type extensionType)
	{
		if (spatializerExtension == null)
		{
			spatializerExtension = ScriptableObject.CreateInstance(extensionType) as AudioSourceExtension;
		}
		return spatializerExtension;
	}

	internal AudioSourceExtension AddAmbisonicExtension(Type extensionType)
	{
		if (ambisonicExtension == null)
		{
			ambisonicExtension = ScriptableObject.CreateInstance(extensionType) as AudioSourceExtension;
		}
		return ambisonicExtension;
	}
}
