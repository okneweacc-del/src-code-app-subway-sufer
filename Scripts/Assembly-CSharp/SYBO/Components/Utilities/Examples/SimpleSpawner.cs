using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Components.Utilities.Examples;

public class SimpleSpawner : MonoBehaviour
{
	public string poolName;

	public Transform testPrefab;

	public int spawnAmount = 50;

	public float spawnInterval = 0.25f;

	public string particlesPoolName;

	private void Start()
	{
		StartCoroutine(Spawner());
	}

	private IEnumerator Spawner()
	{
		int count = spawnAmount;
		SpawnPool shapesPool = PoolManager.Pools[poolName];
		while (count > 0)
		{
			Transform inst = shapesPool.Spawn(testPrefab);
			inst.localPosition = new Vector3(spawnAmount + 2 - count, 0f, 0f);
			count--;
			yield return new WaitForSeconds(spawnInterval);
		}
		StartCoroutine(Despawner());
		yield return null;
	}

	private IEnumerator Despawner()
	{
		SpawnPool shapesPool = PoolManager.Pools[poolName];
		List<Transform> spawnedCopy = new List<Transform>(shapesPool);
		foreach (Transform instance in spawnedCopy)
		{
			shapesPool.Despawn(instance);
			yield return new WaitForSeconds(spawnInterval);
		}
		StartCoroutine(Spawner());
		yield return null;
	}
}
