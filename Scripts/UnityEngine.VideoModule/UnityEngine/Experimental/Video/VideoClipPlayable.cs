using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Video;

[NativeHeader("Modules/Video/Public/Director/VideoClipPlayable.h")]
[StaticAccessor("VideoClipPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
[NativeHeader("Modules/Video/Public/ScriptBindings/VideoClipPlayable.bindings.h")]
[NativeHeader("Modules/Video/Public/VideoClip.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
public struct VideoClipPlayable : IPlayable, IEquatable<VideoClipPlayable>
{
	private PlayableHandle m_Handle;

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(VideoClipPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
