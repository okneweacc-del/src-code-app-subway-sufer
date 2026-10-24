using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.U2D;

public sealed class SpriteAtlasManager
{
	public delegate void RequestAtlasCallback(string tag, Action<SpriteAtlas> action);

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static RequestAtlasCallback atlasRequested = null;

	[RequiredByNativeCode]
	private static bool RequestAtlas(string tag)
	{
		if (atlasRequested != null)
		{
			atlasRequested(tag, Register);
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Register(SpriteAtlas spriteAtlas);
}
