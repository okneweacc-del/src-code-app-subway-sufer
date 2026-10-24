using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Texture.h")]
[UsedByNativeCode]
public class Texture : Object
{
	public virtual int width
	{
		get
		{
			return GetDataWidth();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public virtual int height
	{
		get
		{
			return GetDataHeight();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public virtual TextureDimension dimension
	{
		get
		{
			return GetDimension();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public extern TextureWrapMode wrapMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetWrapModeU")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern FilterMode filterMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int anisoLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float mipMapBias
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Vector2 texelSize
	{
		[NativeName("GetNpotTexelSize")]
		get
		{
			get_texelSize_Injected(out var ret);
			return ret;
		}
	}

	protected Texture()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetDataWidth();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetDataHeight();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern TextureDimension GetDimension();

	internal UnityException CreateNonReadableException(Texture t)
	{
		return new UnityException($"Texture '{t.name}' is not readable, the texture memory can not be accessed from scripts. You can make the texture readable in the Texture Import Settings.");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_texelSize_Injected(out Vector2 ret);
}
