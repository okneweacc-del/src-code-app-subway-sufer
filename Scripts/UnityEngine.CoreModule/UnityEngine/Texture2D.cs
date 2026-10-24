using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Texture2D.h")]
[NativeHeader("Runtime/Graphics/GeneratedTextures.h")]
[UsedByNativeCode]
public sealed class Texture2D : Texture
{
	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static extern Texture2D whiteTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	internal Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
	{
		Internal_Create(this, width, height, format, mipmap, linear, nativeTex);
	}

	public Texture2D(int width, int height, TextureFormat format, bool mipmap)
		: this(width, height, format, mipmap, linear: false, IntPtr.Zero)
	{
	}

	public Texture2D(int width, int height)
		: this(width, height, TextureFormat.RGBA32, mipmap: true, linear: false, IntPtr.Zero)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetAllPixels32(Color32[] colors, int miplevel);

	[ExcludeFromDocs]
	public void SetPixels32(Color32[] colors)
	{
		int miplevel = 0;
		SetPixels32(colors, miplevel);
	}

	public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
	{
		SetAllPixels32(colors, miplevel);
	}

	[ExcludeFromDocs]
	public void ReadPixels(Rect source, int destX, int destY)
	{
		bool recalculateMipMaps = true;
		INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadPixels(Texture2D self, ref Rect source, int destX, int destY, bool recalculateMipMaps);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Texture2DScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] Texture2D mono, int w, int h, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex);

	private static void Internal_Create([Writable] Texture2D mono, int w, int h, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
	{
		if (!Internal_CreateImpl(mono, w, h, format, mipmap, linear, nativeTex))
		{
			throw new UnityException("Failed to create texture because of invalid parameters.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetIsReadable")]
	private extern bool IsReadable();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("Apply")]
	private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

	[NativeName("SetPixel")]
	private void SetPixelImpl(int image, int x, int y, Color color)
	{
		SetPixelImpl_Injected(image, x, y, ref color);
	}

	[NativeName("GetPixelBilinear")]
	private Color GetPixelBilinearImpl(int image, float x, float y)
	{
		GetPixelBilinearImpl_Injected(image, x, y, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "Texture2DScripting::SetPixels", HasExplicitThis = true)]
	private extern void SetPixelsImpl(int x, int y, int w, int h, Color[] pixel, int miplevel, int frame);

	public void SetPixel(int x, int y, Color color)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		SetPixelImpl(0, x, y, color);
	}

	public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
	}

	public void SetPixels(Color[] colors)
	{
		SetPixels(0, 0, width, height, colors, 0);
	}

	public Color GetPixelBilinear(float x, float y)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelBilinearImpl(0, x, y);
	}

	public void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		ApplyImpl(updateMipmaps, makeNoLongerReadable);
	}

	public void Apply()
	{
		Apply(updateMipmaps: true, makeNoLongerReadable: false);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetPixelImpl_Injected(int image, int x, int y, ref Color color);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetPixelBilinearImpl_Injected(int image, float x, float y, out Color ret);
}
