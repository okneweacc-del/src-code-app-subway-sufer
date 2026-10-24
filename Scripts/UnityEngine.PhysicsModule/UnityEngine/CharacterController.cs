using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class CharacterController : Collider
{
	public extern bool isGrounded
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public Vector3 velocity
	{
		get
		{
			INTERNAL_get_velocity(out var value);
			return value;
		}
	}

	public extern float height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public Vector3 center
	{
		get
		{
			INTERNAL_get_center(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_center(ref value);
		}
	}

	public extern float skinWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool detectCollisions
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public CollisionFlags Move(Vector3 motion)
	{
		return INTERNAL_CALL_Move(this, ref motion);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern CollisionFlags INTERNAL_CALL_Move(CharacterController self, ref Vector3 motion);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_velocity(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_center(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_center(ref Vector3 value);
}
