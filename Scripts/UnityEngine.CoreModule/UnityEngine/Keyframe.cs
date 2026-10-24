using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode]
public struct Keyframe
{
	private float m_Time;

	private float m_Value;

	private float m_InTangent;

	private float m_OutTangent;

	private int m_WeightedMode;

	private float m_InWeight;

	private float m_OutWeight;

	public float time => m_Time;

	public float value => m_Value;

	public float inTangent => m_InTangent;

	public float outTangent => m_OutTangent;

	public Keyframe(float time, float value)
	{
		m_Time = time;
		m_Value = value;
		m_InTangent = 0f;
		m_OutTangent = 0f;
		m_WeightedMode = 0;
		m_InWeight = 0f;
		m_OutWeight = 0f;
	}

	public Keyframe(float time, float value, float inTangent, float outTangent)
	{
		m_Time = time;
		m_Value = value;
		m_InTangent = inTangent;
		m_OutTangent = outTangent;
		m_WeightedMode = 0;
		m_InWeight = 0f;
		m_OutWeight = 0f;
	}
}
