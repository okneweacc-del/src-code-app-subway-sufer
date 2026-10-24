using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[UsedByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
public struct PlayableOutputHandle
{
	internal IntPtr m_Handle;

	internal int m_Version;

	public static PlayableOutputHandle Null => new PlayableOutputHandle
	{
		m_Version = int.MaxValue
	};

	public override int GetHashCode()
	{
		return m_Handle.GetHashCode() ^ m_Version.GetHashCode();
	}

	public static bool operator ==(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return CompareVersion(lhs, rhs);
	}

	public override bool Equals(object p)
	{
		return p is PlayableOutputHandle && CompareVersion(this, (PlayableOutputHandle)p);
	}

	internal static bool CompareVersion(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
	}
}
