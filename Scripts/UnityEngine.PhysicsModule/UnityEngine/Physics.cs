using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

public class Physics
{
	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		return Raycast(origin, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		return Raycast(origin, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		float maxDistance = float.PositiveInfinity;
		return Raycast(origin, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool Raycast(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return Internal_RaycastTest(origin, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	[RequiredByNativeCode]
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		return Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		return Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		float maxDistance = float.PositiveInfinity;
		return Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return Internal_Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, float maxDistance, int layerMask)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		return Raycast(ray, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, float maxDistance)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		return Raycast(ray, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		float maxDistance = float.PositiveInfinity;
		return Raycast(ray, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool Raycast(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return Raycast(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		return Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		return Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, out RaycastHit hitInfo)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		float maxDistance = float.PositiveInfinity;
		return Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool Raycast(Ray ray, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Ray ray, float maxDistance, int layerMask)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		return RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Ray ray, float maxDistance)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		return RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Ray ray)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layerMask = -5;
		float maxDistance = float.PositiveInfinity;
		return RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] RaycastAll(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return RaycastAll(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, int layermask)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		return INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layermask = -5;
		return INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
	{
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
		int layermask = -5;
		float maxDistance = float.PositiveInfinity;
		return INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask, queryTriggerInteraction);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern RaycastHit[] INTERNAL_CALL_RaycastAll(ref Vector3 origin, ref Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);

	public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return RaycastNonAlloc(ray.origin, ray.direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, maxDistance, layermask, queryTriggerInteraction);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_RaycastNonAlloc(ref Vector3 origin, ref Vector3 direction, RaycastHit[] results, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);

	private static bool Internal_Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return INTERNAL_CALL_Internal_Raycast(ref origin, ref direction, out hitInfo, maxDistance, layermask, queryTriggerInteraction);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_Internal_Raycast(ref Vector3 origin, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);

	private static bool Internal_RaycastTest(Vector3 origin, Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return INTERNAL_CALL_Internal_RaycastTest(ref origin, ref direction, maxDistance, layermask, queryTriggerInteraction);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_Internal_RaycastTest(ref Vector3 origin, ref Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
}
