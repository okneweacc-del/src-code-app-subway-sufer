using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
public sealed class SpriteRenderer : Renderer
{
	public Sprite sprite
	{
		set
		{
			SetSprite_INTERNAL(value);
		}
	}

	public Color color
	{
		get
		{
			INTERNAL_get_color(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_color(ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetSprite_INTERNAL(Sprite sprite);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_color(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_color(ref Color value);
}
