using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class Gizmos
{
	public static Color color
	{
		set
		{
			INTERNAL_set_color(ref value);
		}
	}

	public static void DrawRay(Vector3 from, Vector3 direction)
	{
		DrawLine(from, from + direction);
	}

	public static void DrawLine(Vector3 from, Vector3 to)
	{
		INTERNAL_CALL_DrawLine(ref from, ref to);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DrawLine(ref Vector3 from, ref Vector3 to);

	public static void DrawWireSphere(Vector3 center, float radius)
	{
		INTERNAL_CALL_DrawWireSphere(ref center, radius);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DrawWireSphere(ref Vector3 center, float radius);

	public static void DrawSphere(Vector3 center, float radius)
	{
		INTERNAL_CALL_DrawSphere(ref center, radius);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DrawSphere(ref Vector3 center, float radius);

	public static void DrawWireCube(Vector3 center, Vector3 size)
	{
		INTERNAL_CALL_DrawWireCube(ref center, ref size);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DrawWireCube(ref Vector3 center, ref Vector3 size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_color(ref Color value);
}
