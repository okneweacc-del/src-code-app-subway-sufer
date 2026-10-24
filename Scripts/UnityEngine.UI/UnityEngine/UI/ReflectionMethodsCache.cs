using System;
using System.Reflection;
using UnityEngineInternal;

namespace UnityEngine.UI;

internal class ReflectionMethodsCache
{
	public delegate bool Raycast3DCallback(Ray r, out RaycastHit hit, float f, int i);

	public delegate RaycastHit2D Raycast2DCallback(Vector2 p1, Vector2 p2, float f, int i);

	public delegate RaycastHit[] RaycastAllCallback(Ray r, float f, int i);

	public delegate RaycastHit2D[] GetRayIntersectionAllCallback(Ray r, float f, int i);

	public delegate int GetRayIntersectionAllNonAllocCallback(Ray r, RaycastHit2D[] results, float f, int i);

	public delegate int GetRaycastNonAllocCallback(Ray r, RaycastHit[] results, float f, int i);

	public Raycast3DCallback raycast3D = null;

	public RaycastAllCallback raycast3DAll = null;

	public Raycast2DCallback raycast2D = null;

	public GetRayIntersectionAllCallback getRayIntersectionAll = null;

	public GetRayIntersectionAllNonAllocCallback getRayIntersectionAllNonAlloc = null;

	public GetRaycastNonAllocCallback getRaycastNonAlloc = null;

	private static ReflectionMethodsCache s_ReflectionMethodsCache = null;

	public static ReflectionMethodsCache Singleton
	{
		get
		{
			if (s_ReflectionMethodsCache == null)
			{
				s_ReflectionMethodsCache = new ReflectionMethodsCache();
			}
			return s_ReflectionMethodsCache;
		}
	}

	public ReflectionMethodsCache()
	{
		MethodInfo method = typeof(Physics).GetMethod("Raycast", new Type[4]
		{
			typeof(Ray),
			typeof(RaycastHit).MakeByRefType(),
			typeof(float),
			typeof(int)
		});
		if (method != null)
		{
			raycast3D = (Raycast3DCallback)ScriptingUtils.CreateDelegate(typeof(Raycast3DCallback), method);
		}
		MethodInfo method2 = typeof(Physics2D).GetMethod("Raycast", new Type[4]
		{
			typeof(Vector2),
			typeof(Vector2),
			typeof(float),
			typeof(int)
		});
		if (method2 != null)
		{
			raycast2D = (Raycast2DCallback)ScriptingUtils.CreateDelegate(typeof(Raycast2DCallback), method2);
		}
		MethodInfo method3 = typeof(Physics).GetMethod("RaycastAll", new Type[3]
		{
			typeof(Ray),
			typeof(float),
			typeof(int)
		});
		if (method3 != null)
		{
			raycast3DAll = (RaycastAllCallback)ScriptingUtils.CreateDelegate(typeof(RaycastAllCallback), method3);
		}
		MethodInfo method4 = typeof(Physics2D).GetMethod("GetRayIntersectionAll", new Type[3]
		{
			typeof(Ray),
			typeof(float),
			typeof(int)
		});
		if (method4 != null)
		{
			getRayIntersectionAll = (GetRayIntersectionAllCallback)ScriptingUtils.CreateDelegate(typeof(GetRayIntersectionAllCallback), method4);
		}
		MethodInfo method5 = typeof(Physics2D).GetMethod("GetRayIntersectionNonAlloc", new Type[4]
		{
			typeof(Ray),
			typeof(RaycastHit2D[]),
			typeof(float),
			typeof(int)
		});
		if (method5 != null)
		{
			getRayIntersectionAllNonAlloc = (GetRayIntersectionAllNonAllocCallback)ScriptingUtils.CreateDelegate(typeof(GetRayIntersectionAllNonAllocCallback), method5);
		}
		MethodInfo method6 = typeof(Physics).GetMethod("RaycastNonAlloc", new Type[4]
		{
			typeof(Ray),
			typeof(RaycastHit[]),
			typeof(float),
			typeof(int)
		});
		if (method6 != null)
		{
			getRaycastNonAlloc = (GetRaycastNonAllocCallback)ScriptingUtils.CreateDelegate(typeof(GetRaycastNonAllocCallback), method6);
		}
	}
}
