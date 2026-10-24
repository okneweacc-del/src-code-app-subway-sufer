using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Camera/Camera.h")]
[NativeHeader("Runtime/Graphics/RenderTexture.h")]
public class RenderTexture : Texture
{
	public extern int depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern RenderTexture active
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public override extern int width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public override extern int height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public override extern TextureDimension dimension
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("ColorFormat")]
	public extern RenderTextureFormat format
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool autoGenerateMips
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int volumeDepth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int antiAliasing
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public bool isPowerOfTwo
	{
		set
		{
		}
	}

	protected internal RenderTexture()
	{
	}

	public RenderTexture(int width, int height, int depth, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
	{
		Internal_Create(this);
		this.width = width;
		this.height = height;
		this.depth = depth;
		this.format = format;
		bool flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
		SetSRGBReadWrite((readWrite != RenderTextureReadWrite.Default) ? (readWrite == RenderTextureReadWrite.sRGB) : flag);
	}

	public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
		: this(width, height, depth, format, RenderTextureReadWrite.Default)
	{
	}

	public RenderTexture(int width, int height, int depth)
		: this(width, height, depth, RenderTextureFormat.Default, RenderTextureReadWrite.Default)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SetSRGBReadWrite(bool srgb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RenderTextureScripting::Create")]
	private static extern void Internal_Create([Writable] RenderTexture rt);
}
