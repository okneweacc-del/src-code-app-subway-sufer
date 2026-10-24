using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct ContactPoint
{
	internal Vector3 m_Point;

	internal Vector3 m_Normal;

	internal int m_ThisColliderInstanceID;

	internal int m_OtherColliderInstanceID;

	internal float m_Separation;
}
