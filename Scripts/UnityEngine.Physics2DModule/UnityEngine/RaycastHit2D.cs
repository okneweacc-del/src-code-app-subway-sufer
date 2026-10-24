using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct RaycastHit2D
{
	private Vector2 m_Centroid;

	private Vector2 m_Point;

	private Vector2 m_Normal;

	private float m_Distance;

	private float m_Fraction;

	private int m_Collider;

	public Vector2 point => m_Point;

	public Vector2 normal => m_Normal;

	public float distance => m_Distance;

	public Collider2D collider => Object.FindObjectFromInstanceID(m_Collider) as Collider2D;
}
