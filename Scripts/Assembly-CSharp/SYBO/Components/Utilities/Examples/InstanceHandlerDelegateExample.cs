using System;
using UnityEngine;

namespace SYBO.Components.Utilities.Examples;

public class InstanceHandlerDelegateExample : MonoBehaviour
{
	private void Awake()
	{
		InstanceHandler.InstantiateDelegates = (InstanceHandler.InstantiateDelegate)Delegate.Combine(InstanceHandler.InstantiateDelegates, new InstanceHandler.InstantiateDelegate(InstantiateDelegate));
		InstanceHandler.DestroyDelegates = (InstanceHandler.DestroyDelegate)Delegate.Combine(InstanceHandler.DestroyDelegates, new InstanceHandler.DestroyDelegate(DestroyDelegate));
	}

	private void Start()
	{
		SpawnPool spawnPool = PoolManager.Pools["Shapes"];
		spawnPool.instantiateDelegates = (SpawnPool.InstantiateDelegate)Delegate.Combine(spawnPool.instantiateDelegates, new SpawnPool.InstantiateDelegate(InstantiateDelegateForShapesPool));
	}

	public GameObject InstantiateDelegate(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		return UnityEngine.Object.Instantiate(prefab, pos, rot);
	}

	public void DestroyDelegate(GameObject instance)
	{
		UnityEngine.Object.Destroy(instance);
	}

	public GameObject InstantiateDelegateForShapesPool(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		return UnityEngine.Object.Instantiate(prefab, pos, rot);
	}
}
