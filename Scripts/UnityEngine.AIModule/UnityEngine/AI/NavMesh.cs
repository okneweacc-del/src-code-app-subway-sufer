using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI;

[MovedFrom("UnityEngine")]
[NativeHeader("Modules/AI/NavMeshManager.h")]
public static class NavMesh
{
	public delegate void OnNavMeshPreUpdate();

	public static OnNavMeshPreUpdate onPreUpdate;

	[RequiredByNativeCode]
	private static void Internal_CallOnNavMeshPreUpdate()
	{
		if (onPreUpdate != null)
		{
			onPreUpdate();
		}
	}
}
