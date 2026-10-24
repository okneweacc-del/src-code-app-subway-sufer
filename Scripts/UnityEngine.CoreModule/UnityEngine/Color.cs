using System.Reflection;
using UnityEngine.Scripting;

namespace UnityEngine;

[DefaultMember("Item")]
[UsedByNativeCode]
public struct Color
{
	public float r;

	public float g;

	public float b;

	public float a;

	public static Color red => new Color(1f, 0f, 0f, 1f);

	public static Color green => new Color(0f, 1f, 0f, 1f);

	public static Color blue => new Color(0f, 0f, 1f, 1f);

	public static Color white => new Color(1f, 1f, 1f, 1f);

	public static Color black => new Color(0f, 0f, 0f, 1f);

	public static Color yellow => new Color(1f, 235f / 255f, 4f / 255f, 1f);

	public static Color magenta => new Color(1f, 0f, 1f, 1f);

	public static Color gray => new Color(0.5f, 0.5f, 0.5f, 1f);

	public static Color grey => new Color(0.5f, 0.5f, 0.5f, 1f);

	public static Color clear => new Color(0f, 0f, 0f, 0f);

	public float maxColorComponent => Mathf.Max(Mathf.Max(r, g), b);

	public Color(float r, float g, float b, float a)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}

	public Color(float r, float g, float b)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		a = 1f;
	}

	public override string ToString()
	{
		return UnityString.Format("RGBA({0:F3}, {1:F3}, {2:F3}, {3:F3})", r, g, b, a);
	}

	public override int GetHashCode()
	{
		return ((Vector4)this/*cast due to constrained. prefix*/).GetHashCode();
	}

	public override bool Equals(object other)
	{
		if (!(other is Color color))
		{
			return false;
		}
		return r.Equals(color.r) && g.Equals(color.g) && b.Equals(color.b) && a.Equals(color.a);
	}

	public static Color operator *(Color a, Color b)
	{
		return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
	}

	public static Color operator *(Color a, float b)
	{
		return new Color(a.r * b, a.g * b, a.b * b, a.a * b);
	}

	public static Color operator /(Color a, float b)
	{
		return new Color(a.r / b, a.g / b, a.b / b, a.a / b);
	}

	public static bool operator ==(Color lhs, Color rhs)
	{
		return (Vector4)lhs == (Vector4)rhs;
	}

	public static bool operator !=(Color lhs, Color rhs)
	{
		return !(lhs == rhs);
	}

	public static Color Lerp(Color a, Color b, float t)
	{
		t = Mathf.Clamp01(t);
		return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
	}

	public static implicit operator Vector4(Color c)
	{
		return new Vector4(c.r, c.g, c.b, c.a);
	}

	public static implicit operator Color(Vector4 v)
	{
		return new Color(v.x, v.y, v.z, v.w);
	}
}
