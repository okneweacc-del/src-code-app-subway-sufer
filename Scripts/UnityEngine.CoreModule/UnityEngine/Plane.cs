using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct Plane
{
	private Vector3 m_Normal;

	private float m_Distance;

	public Plane(Vector3 inNormal, Vector3 inPoint)
	{
		m_Normal = Vector3.Normalize(inNormal);
		m_Distance = 0f - Vector3.Dot(m_Normal, inPoint);
	}

	public Plane(Vector3 inNormal, float d)
	{
		m_Normal = Vector3.Normalize(inNormal);
		m_Distance = d;
	}

	public Plane(Vector3 a, Vector3 b, Vector3 c)
	{
		m_Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
		m_Distance = 0f - Vector3.Dot(m_Normal, a);
	}

	public bool Raycast(Ray ray, out float enter)
	{
		float num = Vector3.Dot(ray.direction, m_Normal);
		float num2 = 0f - Vector3.Dot(ray.origin, m_Normal) - m_Distance;
		if (Mathf.Approximately(num, 0f))
		{
			enter = 0f;
			return false;
		}
		enter = num2 / num;
		return enter > 0f;
	}

	public override string ToString()
	{
		return UnityString.Format("(normal:({0:F1}, {1:F1}, {2:F1}), distance:{3:F1})", m_Normal.x, m_Normal.y, m_Normal.z, m_Distance);
	}
}
