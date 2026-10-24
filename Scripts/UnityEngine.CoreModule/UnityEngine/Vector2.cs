using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[GenerateManagedProxy("Vector2f")]
public struct Vector2(float x, float y)
{
	public float x = x;

	public float y = y;

	private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

	private static readonly Vector2 oneVector = new Vector2(1f, 1f);

	private static readonly Vector2 upVector = new Vector2(0f, 1f);

	private static readonly Vector2 downVector = new Vector2(0f, -1f);

	private static readonly Vector2 leftVector = new Vector2(-1f, 0f);

	private static readonly Vector2 rightVector = new Vector2(1f, 0f);

	private static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

	private static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

	public const float kEpsilon = 1E-05f;

	public float this[int index]
	{
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				_ => throw new IndexOutOfRangeException("Invalid Vector2 index!"), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid Vector2 index!");
			}
		}
	}

	public Vector2 normalized
	{
		get
		{
			Vector2 result = new Vector2(x, y);
			result.Normalize();
			return result;
		}
	}

	public float magnitude => Mathf.Sqrt(x * x + y * y);

	public float sqrMagnitude => x * x + y * y;

	public static Vector2 zero => zeroVector;

	public static Vector2 one => oneVector;

	public static Vector2 up => upVector;

	public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
	{
		t = Mathf.Clamp01(t);
		return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
	}

	public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
	{
		return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
	}

	public static Vector2 Scale(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x * b.x, a.y * b.y);
	}

	public void Scale(Vector2 scale)
	{
		x *= scale.x;
		y *= scale.y;
	}

	public void Normalize()
	{
		float num = magnitude;
		if (num > 1E-05f)
		{
			this /= num;
		}
		else
		{
			this = zero;
		}
	}

	public override string ToString()
	{
		return UnityString.Format("({0:F1}, {1:F1})", x, y);
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2);
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector2 vector))
		{
			return false;
		}
		return x.Equals(vector.x) && y.Equals(vector.y);
	}

	public static float Dot(Vector2 lhs, Vector2 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y;
	}

	public float SqrMagnitude()
	{
		return x * x + y * y;
	}

	public static Vector2 operator +(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x + b.x, a.y + b.y);
	}

	public static Vector2 operator -(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x - b.x, a.y - b.y);
	}

	public static Vector2 operator -(Vector2 a)
	{
		return new Vector2(0f - a.x, 0f - a.y);
	}

	public static Vector2 operator *(Vector2 a, float d)
	{
		return new Vector2(a.x * d, a.y * d);
	}

	public static Vector2 operator *(float d, Vector2 a)
	{
		return new Vector2(a.x * d, a.y * d);
	}

	public static Vector2 operator /(Vector2 a, float d)
	{
		return new Vector2(a.x / d, a.y / d);
	}

	public static bool operator ==(Vector2 lhs, Vector2 rhs)
	{
		return (lhs - rhs).sqrMagnitude < 9.9999994E-11f;
	}

	public static bool operator !=(Vector2 lhs, Vector2 rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator Vector2(Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static implicit operator Vector3(Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}
}
