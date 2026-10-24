using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
[NativeHeader("Modules/Director/PlayableDirector.h")]
[RequiredByNativeCode]
public class PlayableDirector : Behaviour, IExposedPropertyTable
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<PlayableDirector> played;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<PlayableDirector> paused;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<PlayableDirector> stopped;

	[RequiredByNativeCode]
	private void SendOnPlayableDirectorPlay()
	{
		if (played != null)
		{
			played(this);
		}
	}

	[RequiredByNativeCode]
	private void SendOnPlayableDirectorPause()
	{
		if (paused != null)
		{
			paused(this);
		}
	}

	[RequiredByNativeCode]
	private void SendOnPlayableDirectorStop()
	{
		if (stopped != null)
		{
			stopped(this);
		}
	}
}
