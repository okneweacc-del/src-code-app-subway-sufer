using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Animation/ScriptBindings/AvatarBuilder.bindings.h")]
[NativeType(CodegenOptions.Custom, "MonoHumanLimit")]
[NativeHeader("Runtime/Animation/HumanDescription.h")]
public struct HumanLimit
{
	private Vector3 m_Min;

	private Vector3 m_Max;

	private Vector3 m_Center;

	private float m_AxisLength;

	private int m_UseDefaultValues;
}
