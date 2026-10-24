using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeHeader("Modules/Physics2D/Public/Collider2D.h")]
public class Collider2D : Behaviour
{
	public extern bool isTrigger
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Vector2 offset
	{
		set
		{
			set_offset_Injected(ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_offset_Injected(ref Vector2 value);
}
