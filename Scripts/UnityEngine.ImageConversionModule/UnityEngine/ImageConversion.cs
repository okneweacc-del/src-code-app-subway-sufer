using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

public static class ImageConversion
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern byte[] EncodeToPNG(this Texture2D tex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool LoadImage(this Texture2D tex, byte[] data, [DefaultValue("false")] bool markNonReadable);

	[ExcludeFromDocs]
	public static bool LoadImage(this Texture2D tex, byte[] data)
	{
		bool markNonReadable = false;
		return tex.LoadImage(data, markNonReadable);
	}
}
