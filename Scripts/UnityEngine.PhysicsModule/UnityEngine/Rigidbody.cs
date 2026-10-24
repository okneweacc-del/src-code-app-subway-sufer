using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
public sealed class Rigidbody : Component
{
	public extern bool useGravity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool isKinematic
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public Vector3 position
	{
		set
		{
			INTERNAL_set_position(ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_position(ref Vector3 value);
}
