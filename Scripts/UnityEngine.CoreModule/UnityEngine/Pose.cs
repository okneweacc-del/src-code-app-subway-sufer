using System;

namespace UnityEngine;

[Serializable]
public struct Pose(Vector3 position, Quaternion rotation)
{
	public Vector3 position = position;

	public Quaternion rotation = rotation;

	private static readonly Pose k_Identity = new Pose(Vector3.zero, Quaternion.identity);

	public override string ToString()
	{
		return $"({position.ToString()}, {rotation.ToString()})";
	}
}
