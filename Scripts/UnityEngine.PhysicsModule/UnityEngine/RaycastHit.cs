using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct RaycastHit
{
	private Vector3 m_Point;

	private Vector3 m_Normal;

	private int m_FaceID;

	private float m_Distance;

	private Vector2 m_UV;

	private int m_Collider;

	public Vector3 point => m_Point;

	public Vector3 normal => m_Normal;

	public float distance => m_Distance;

	public Collider collider => Object.FindObjectFromInstanceID(m_Collider) as Collider;
}
