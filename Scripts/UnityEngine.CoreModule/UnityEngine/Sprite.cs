using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/SpriteUtility.h")]
[NativeType("Runtime/Graphics/SpriteFrame.h")]
[NativeHeader("Runtime/2D/Common/ScriptBindings/SpritesMarshalling.h")]
public sealed class Sprite : Object
{
	public Rect rect
	{
		get
		{
			INTERNAL_get_rect(out var value);
			return value;
		}
	}

	public extern Texture2D texture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern Texture2D associatedAlphaSplitTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public Rect textureRect
	{
		get
		{
			INTERNAL_get_textureRect(out var value);
			return value;
		}
	}

	public Vector2 textureRectOffset
	{
		get
		{
			Internal_GetTextureRectOffset(this, out var output);
			return output;
		}
	}

	public extern bool packed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public Vector4 border
	{
		get
		{
			INTERNAL_get_border(out var value);
			return value;
		}
	}

	public extern float pixelsPerUnit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetPixelsToUnits")]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_rect(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_textureRect(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_GetTextureRectOffset(Sprite sprite, out Vector2 output);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_border(out Vector4 value);
}
