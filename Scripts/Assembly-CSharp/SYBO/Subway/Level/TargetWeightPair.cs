using System;
using UnityEngine;

namespace SYBO.Subway.Level;

[Serializable]
public class TargetWeightPair
{
	[SerializeField]
	private GameObject _target;

	[SerializeField]
	private float _weight;

	public GameObject Target => _target;

	public float Weight => _weight;
}
