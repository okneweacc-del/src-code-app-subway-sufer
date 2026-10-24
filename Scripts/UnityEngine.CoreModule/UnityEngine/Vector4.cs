using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct Vector4(float x, float y, float z, float w)
{
	public const float kEpsilon = 1E-05f;

	public float x = x;

	public float y = y;

	public float z = z;

	public float w = w;

	private static readonly Vector4 zeroVector = new Vector4(0f, 0f, 0f, 0f);

	private static readonly Vector4 oneVector = new Vector4(1f, 1f, 1f, 1f);

	private static readonly Vector4 positiveInfinityVector = new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

	private static readonly Vector4 negativeInfinityVector = new Vector4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

	public float this[int index]
	{
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				3 => w, 
				_ => throw new IndexOutOfRangeException("Invalid Vector4 index!"), 
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
			case 2:
				z = value;
				break;
			case 3:
				w = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid Vector4 index!");
			}
		}
	}

	public float sqrMagnitude => Dot(this, this);

	public static Vector4 zero => zeroVector;

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector4 vector))
		{
			return false;
		}
		return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z) && w.Equals(vector.w);
	}

	public static float Dot(Vector4 a, Vector4 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
	}

	public static Vector4 operator -(Vector4 a, Vector4 b)
	{
		return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
	}

	public static Vector4 operator *(Vector4 a, float d)
	{
		return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
	}

	public static Vector4 operator /(Vector4 a, float d)
	{
		return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
	}

	public static bool operator ==(Vector4 lhs, Vector4 rhs)
	{
		return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
	}

	public static bool operator !=(Vector4 lhs, Vector4 rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator Vector3(Vector4 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	public static implicit operator Vector4(Vector2 v)
	{
		return new Vector4(v.x, v.y, 0f, 0f);
	}

	public override string ToString()
	{
		return UnityString.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
	}

	public static float SqrMagnitude(Vector4 a)
	{
		return Dot(a, a);
	}
}
