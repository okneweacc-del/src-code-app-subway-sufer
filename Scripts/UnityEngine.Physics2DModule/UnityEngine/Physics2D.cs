using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Physics2DScriptingClasses.h")]
[NativeHeader("Physics2DScriptingClasses.h")]
[NativeHeader("Modules/Physics2D/PhysicsManager2D.h")]
[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
public class Physics2D
{
	private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool queriesHitTriggers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	[RequiredByNativeCode]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return Raycast_Internal(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return RaycastNonAlloc_Internal(origin, direction, float.PositiveInfinity, contactFilter, results);
	}

	public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
	{
		return RaycastNonAlloc_Internal(origin, direction, distance, contactFilter, results);
	}

	[NativeMethod("Raycast_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static RaycastHit2D Raycast_Internal(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		Raycast_Internal_Injected(ref origin, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("RaycastNonAlloc_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static int RaycastNonAlloc_Internal(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
	{
		return RaycastNonAlloc_Internal_Injected(ref origin, ref direction, distance, ref contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray)
	{
		return GetRayIntersectionAll_Internal(ray.origin, ray.direction, float.PositiveInfinity, -5);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance)
	{
		return GetRayIntersectionAll_Internal(ray.origin, ray.direction, distance, -5);
	}

	[RequiredByNativeCode]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
	{
		return GetRayIntersectionAll_Internal(ray.origin, ray.direction, distance, layerMask);
	}

	[ExcludeFromDocs]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
	{
		return GetRayIntersectionNonAlloc_Internal(ray.origin, ray.direction, float.PositiveInfinity, -5, results);
	}

	[ExcludeFromDocs]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
	{
		return GetRayIntersectionNonAlloc_Internal(ray.origin, ray.direction, distance, -5, results);
	}

	[RequiredByNativeCode]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
	{
		return GetRayIntersectionNonAlloc_Internal(ray.origin, ray.direction, distance, layerMask, results);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("GetRayIntersectionAll_Binding")]
	private static RaycastHit2D[] GetRayIntersectionAll_Internal(Vector3 origin, Vector3 direction, float distance, int layerMask)
	{
		return GetRayIntersectionAll_Internal_Injected(ref origin, ref direction, distance, layerMask);
	}

	[NativeMethod("GetRayIntersectionNonAlloc_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static int GetRayIntersectionNonAlloc_Internal(Vector3 origin, Vector3 direction, float distance, int layerMask, [Out] RaycastHit2D[] results)
	{
		return GetRayIntersectionNonAlloc_Internal_Injected(ref origin, ref direction, distance, layerMask, results);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapPoint(Vector2 point, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPoint_Internal(point, contactFilter);
	}

	[ExcludeFromDocs]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointNonAlloc_Internal(point, contactFilter, results);
	}

	[NativeMethod("OverlapPoint_Binding")]
	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	private static Collider2D OverlapPoint_Internal(Vector2 point, ContactFilter2D contactFilter)
	{
		return OverlapPoint_Internal_Injected(ref point, ref contactFilter);
	}

	[StaticAccessor("GetPhysicsQuery2D()", StaticAccessorType.Arrow)]
	[NativeMethod("OverlapPointNonAlloc_Binding")]
	private static int OverlapPointNonAlloc_Internal(Vector2 point, ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return OverlapPointNonAlloc_Internal_Injected(ref point, ref contactFilter, results);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Raycast_Internal_Injected(ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RaycastNonAlloc_Internal_Injected(ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RaycastHit2D[] GetRayIntersectionAll_Internal_Injected(ref Vector3 origin, ref Vector3 direction, float distance, int layerMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRayIntersectionNonAlloc_Internal_Injected(ref Vector3 origin, ref Vector3 direction, float distance, int layerMask, [Out] RaycastHit2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D OverlapPoint_Internal_Injected(ref Vector2 point, ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapPointNonAlloc_Internal_Injected(ref Vector2 point, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);
}
