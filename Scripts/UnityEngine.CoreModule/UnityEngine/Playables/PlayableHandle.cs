using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[UsedByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
public struct PlayableHandle
{
	internal IntPtr m_Handle;

	internal int m_Version;

	public static PlayableHandle Null => new PlayableHandle
	{
		m_Version = 10
	};

	[VisibleToOtherModules]
	internal bool IsPlayableOfType<T>()
	{
		return GetPlayableType() == typeof(T);
	}

	public static bool operator ==(PlayableHandle x, PlayableHandle y)
	{
		return CompareVersion(x, y);
	}

	public override bool Equals(object p)
	{
		if (!(p is PlayableHandle))
		{
			return false;
		}
		return CompareVersion(this, (PlayableHandle)p);
	}

	public override int GetHashCode()
	{
		return m_Handle.GetHashCode() ^ m_Version.GetHashCode();
	}

	internal static bool CompareVersion(PlayableHandle lhs, PlayableHandle rhs)
	{
		return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
	}

	[VisibleToOtherModules]
	internal bool IsValid()
	{
		return IsValid_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal Type GetPlayableType()
	{
		return GetPlayableType_Injected(ref this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValid_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Type GetPlayableType_Injected(ref PlayableHandle _unity_self);
}
