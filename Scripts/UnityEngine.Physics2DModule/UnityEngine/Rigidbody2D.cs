using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeHeader("Modules/Physics2D/Public/Rigidbody2D.h")]
public sealed class Rigidbody2D : Component
{
	public extern RigidbodyType2D bodyType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("SetBodyType_Binding")]
		set;
	}

	public bool isKinematic
	{
		set
		{
			bodyType = (value ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic);
		}
	}
}
