using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class AudioSettings
{
	public delegate void AudioConfigurationChangeHandler(bool deviceWasChanged);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static AudioConfigurationChangeHandler OnAudioConfigurationChanged;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern string GetSpatializerPluginName();

	[RequiredByNativeCode]
	internal static void InvokeOnAudioConfigurationChanged(bool deviceWasChanged)
	{
		if (OnAudioConfigurationChanged != null)
		{
			OnAudioConfigurationChanged(deviceWasChanged);
		}
	}

	[RequiredByNativeCode]
	internal static void InvokeOnAudioManagerUpdate()
	{
		AudioExtensionManager.Update();
	}

	[RequiredByNativeCode]
	internal static void InvokeOnAudioSourcePlay(AudioSource source)
	{
		AudioSourceExtension audioSourceExtension = AudioExtensionManager.AddSpatializerExtension(source);
		if (audioSourceExtension != null)
		{
			AudioExtensionManager.GetReadyToPlay(audioSourceExtension);
		}
		if (source.clip != null && source.clip.ambisonic)
		{
			AudioSourceExtension audioSourceExtension2 = AudioExtensionManager.AddAmbisonicDecoderExtension(source);
			if (audioSourceExtension2 != null)
			{
				AudioExtensionManager.GetReadyToPlay(audioSourceExtension2);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern string GetAmbisonicDecoderPluginName();
}
