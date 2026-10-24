namespace UnityEngine;

public struct Ray(Vector3 origin, Vector3 direction)
{
	private Vector3 m_Origin = origin;

	private Vector3 m_Direction = direction.normalized;

	public Vector3 origin => m_Origin;

	public Vector3 direction => m_Direction;

	public Vector3 GetPoint(float distance)
	{
		return m_Origin + m_Direction * distance;
	}

	public override string ToString()
	{
		return UnityString.Format("Origin: {0}, Dir: {1}", m_Origin, m_Direction);
	}
}
