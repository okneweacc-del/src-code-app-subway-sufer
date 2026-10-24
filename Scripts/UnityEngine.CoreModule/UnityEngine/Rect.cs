using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct Rect
{
	private float m_XMin;

	private float m_YMin;

	private float m_Width;

	private float m_Height;

	public static Rect zero
	{
		[CompilerGenerated]
		get
		{
			return new Rect(0f, 0f, 0f, 0f);
		}
	}

	public float x
	{
		get
		{
			return m_XMin;
		}
		set
		{
			m_XMin = value;
		}
	}

	public float y
	{
		get
		{
			return m_YMin;
		}
		set
		{
			m_YMin = value;
		}
	}

	public Vector2 position => new Vector2(m_XMin, m_YMin);

	public Vector2 center => new Vector2(x + m_Width / 2f, y + m_Height / 2f);

	public Vector2 min => new Vector2(xMin, yMin);

	public Vector2 max => new Vector2(xMax, yMax);

	public float width
	{
		get
		{
			return m_Width;
		}
		set
		{
			m_Width = value;
		}
	}

	public float height
	{
		get
		{
			return m_Height;
		}
		set
		{
			m_Height = value;
		}
	}

	public Vector2 size => new Vector2(m_Width, m_Height);

	public float xMin
	{
		get
		{
			return m_XMin;
		}
		set
		{
			float num = xMax;
			m_XMin = value;
			m_Width = num - m_XMin;
		}
	}

	public float yMin
	{
		get
		{
			return m_YMin;
		}
		set
		{
			float num = yMax;
			m_YMin = value;
			m_Height = num - m_YMin;
		}
	}

	public float xMax
	{
		get
		{
			return m_Width + m_XMin;
		}
		set
		{
			m_Width = value - m_XMin;
		}
	}

	public float yMax
	{
		get
		{
			return m_Height + m_YMin;
		}
		set
		{
			m_Height = value - m_YMin;
		}
	}

	public Rect(float x, float y, float width, float height)
	{
		m_XMin = x;
		m_YMin = y;
		m_Width = width;
		m_Height = height;
	}

	public Rect(Rect source)
	{
		m_XMin = source.m_XMin;
		m_YMin = source.m_YMin;
		m_Width = source.m_Width;
		m_Height = source.m_Height;
	}

	public static Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax)
	{
		return new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
	}

	public bool Contains(Vector2 point)
	{
		return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
	}

	public bool Contains(Vector3 point)
	{
		return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
	}

	private static Rect OrderMinMax(Rect rect)
	{
		if (rect.xMin > rect.xMax)
		{
			float num = rect.xMin;
			rect.xMin = rect.xMax;
			rect.xMax = num;
		}
		if (rect.yMin > rect.yMax)
		{
			float num2 = rect.yMin;
			rect.yMin = rect.yMax;
			rect.yMax = num2;
		}
		return rect;
	}

	public bool Overlaps(Rect other)
	{
		return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
	}

	public bool Overlaps(Rect other, bool allowInverse)
	{
		Rect rect = this;
		if (allowInverse)
		{
			rect = OrderMinMax(rect);
			other = OrderMinMax(other);
		}
		return rect.Overlaps(other);
	}

	public static bool operator !=(Rect lhs, Rect rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator ==(Rect lhs, Rect rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (width.GetHashCode() << 2) ^ (y.GetHashCode() >> 2) ^ (height.GetHashCode() >> 1);
	}

	public override bool Equals(object other)
	{
		if (!(other is Rect rect))
		{
			return false;
		}
		return x.Equals(rect.x) && y.Equals(rect.y) && width.Equals(rect.width) && height.Equals(rect.height);
	}

	public override string ToString()
	{
		return UnityString.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", x, y, width, height);
	}
}
