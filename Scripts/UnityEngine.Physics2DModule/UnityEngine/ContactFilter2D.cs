using System;
using UnityEngine.Bindings;

namespace UnityEngine;

[Serializable]
public struct ContactFilter2D
{
	[NativeName("m_UseTriggers")]
	public bool useTriggers;

	[NativeName("m_UseLayerMask")]
	public bool useLayerMask;

	[NativeName("m_UseDepth")]
	public bool useDepth;

	[NativeName("m_UseOutsideDepth")]
	public bool useOutsideDepth;

	[NativeName("m_UseNormalAngle")]
	public bool useNormalAngle;

	[NativeName("m_UseOutsideNormalAngle")]
	public bool useOutsideNormalAngle;

	[NativeName("m_LayerMask")]
	public LayerMask layerMask;

	[NativeName("m_MinDepth")]
	public float minDepth;

	[NativeName("m_MaxDepth")]
	public float maxDepth;

	[NativeName("m_MinNormalAngle")]
	public float minNormalAngle;

	[NativeName("m_MaxNormalAngle")]
	public float maxNormalAngle;

	private void CheckConsistency()
	{
		minDepth = ((minDepth != float.NegativeInfinity && minDepth != float.PositiveInfinity && !float.IsNaN(minDepth)) ? minDepth : float.MinValue);
		maxDepth = ((maxDepth != float.NegativeInfinity && maxDepth != float.PositiveInfinity && !float.IsNaN(maxDepth)) ? maxDepth : float.MaxValue);
		if (minDepth > maxDepth)
		{
			float num = minDepth;
			minDepth = maxDepth;
			maxDepth = num;
		}
		minNormalAngle = ((!float.IsNaN(minNormalAngle)) ? Mathf.Clamp(minNormalAngle, 0f, 359.9999f) : 0f);
		maxNormalAngle = ((!float.IsNaN(maxNormalAngle)) ? Mathf.Clamp(maxNormalAngle, 0f, 359.9999f) : 359.9999f);
		if (minNormalAngle > maxNormalAngle)
		{
			float num2 = minNormalAngle;
			minNormalAngle = maxNormalAngle;
			maxNormalAngle = num2;
		}
	}

	public void SetLayerMask(LayerMask layerMask)
	{
		this.layerMask = layerMask;
		useLayerMask = true;
	}

	public void SetDepth(float minDepth, float maxDepth)
	{
		this.minDepth = minDepth;
		this.maxDepth = maxDepth;
		useDepth = true;
		CheckConsistency();
	}

	internal static ContactFilter2D CreateLegacyFilter(int layerMask, float minDepth, float maxDepth)
	{
		ContactFilter2D result = default;
		result.useTriggers = Physics2D.queriesHitTriggers;
		result.SetLayerMask(layerMask);
		result.SetDepth(minDepth, maxDepth);
		return result;
	}
}
