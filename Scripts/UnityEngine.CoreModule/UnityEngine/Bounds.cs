using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Math/MathScripting.h")]
[UsedByNativeCode]
[NativeType(Header = "Runtime/Geometry/AABB.h")]
[NativeHeader("Runtime/Geometry/Ray.h")]
[ThreadAndSerializationSafe]
[NativeHeader("Runtime/Geometry/Intersection.h")]
public struct Bounds(Vector3 center, Vector3 size)
{
	private Vector3 m_Center = center;

	private Vector3 m_Extents = size * 0.5f;

	public Vector3 center
	{
		get
		{
			return m_Center;
		}
		set
		{
			m_Center = value;
		}
	}

	public Vector3 size
	{
		get
		{
			return m_Extents * 2f;
		}
		set
		{
			m_Extents = value * 0.5f;
		}
	}

	public Vector3 extents
	{
		get
		{
			return m_Extents;
		}
		set
		{
			m_Extents = value;
		}
	}

	public Vector3 min
	{
		get
		{
			return center - extents;
		}
		set
		{
			SetMinMax(value, max);
		}
	}

	public Vector3 max
	{
		get
		{
			return center + extents;
		}
		set
		{
			SetMinMax(min, value);
		}
	}

	public override int GetHashCode()
	{
		return center.GetHashCode() ^ (extents.GetHashCode() << 2);
	}

	public override bool Equals(object other)
	{
		if (!(other is Bounds bounds))
		{
			return false;
		}
		return center.Equals(bounds.center) && extents.Equals(bounds.extents);
	}

	public static bool operator ==(Bounds lhs, Bounds rhs)
	{
		return lhs.center == rhs.center && lhs.extents == rhs.extents;
	}

	public static bool operator !=(Bounds lhs, Bounds rhs)
	{
		return !(lhs == rhs);
	}

	public void SetMinMax(Vector3 min, Vector3 max)
	{
		extents = (max - min) * 0.5f;
		center = min + extents;
	}

	public void Encapsulate(Vector3 point)
	{
		SetMinMax(Vector3.Min(min, point), Vector3.Max(max, point));
	}

	public void Encapsulate(Bounds bounds)
	{
		Encapsulate(bounds.center - bounds.extents);
		Encapsulate(bounds.center + bounds.extents);
	}

	public bool Intersects(Bounds bounds)
	{
		return min.x <= bounds.max.x && max.x >= bounds.min.x && min.y <= bounds.max.y && max.y >= bounds.min.y && min.z <= bounds.max.z && max.z >= bounds.min.z;
	}

	public override string ToString()
	{
		return UnityString.Format("Center: {0}, Extents: {1}", m_Center, m_Extents);
	}

	[NativeMethod("IsInside", IsThreadSafe = true)]
	public bool Contains(Vector3 point)
	{
		return Contains_Injected(ref this, ref point);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Contains_Injected(ref Bounds _unity_self, ref Vector3 point);
}
