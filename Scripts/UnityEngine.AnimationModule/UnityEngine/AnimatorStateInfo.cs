using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode]
[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
public struct AnimatorStateInfo
{
	private int m_Name;

	private int m_Path;

	private int m_FullPath;

	private float m_NormalizedTime;

	private float m_Length;

	private float m_Speed;

	private float m_SpeedMultiplier;

	private int m_Tag;

	private int m_Loop;

	public int fullPathHash => m_FullPath;

	public float normalizedTime => m_NormalizedTime;
}
