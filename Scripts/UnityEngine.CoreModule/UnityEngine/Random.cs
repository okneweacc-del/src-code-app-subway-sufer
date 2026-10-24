using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Random.bindings.h")]
public sealed class Random
{
	public static extern float value
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		get;
	}

	public static Vector3 insideUnitSphere
	{
		[FreeFunction]
		get
		{
			get_insideUnitSphere_Injected(out var ret);
			return ret;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float Range(float min, float max);

	public static int Range(int min, int max)
	{
		return RandomRangeInt(min, max);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern int RandomRangeInt(int min, int max);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_insideUnitSphere_Injected(out Vector3 ret);
}
