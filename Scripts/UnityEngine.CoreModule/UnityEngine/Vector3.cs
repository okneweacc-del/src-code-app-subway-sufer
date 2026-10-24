using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeType(Header = "Runtime/Math/Vector3.h")]
[UsedByNativeCode]
[ThreadAndSerializationSafe]
[NativeHeader("Runtime/Math/MathScripting.h")]
public struct Vector3
{
	public const float kEpsilon = 1E-05f;

	public float x;

	public float y;

	public float z;

	private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);

	private static readonly Vector3 oneVector = new Vector3(1f, 1f, 1f);

	private static readonly Vector3 upVector = new Vector3(0f, 1f, 0f);

	private static readonly Vector3 downVector = new Vector3(0f, -1f, 0f);

	private static readonly Vector3 leftVector = new Vector3(-1f, 0f, 0f);

	private static readonly Vector3 rightVector = new Vector3(1f, 0f, 0f);

	private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);

	private static readonly Vector3 backVector = new Vector3(0f, 0f, -1f);

	private static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

	private static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

	public float this[int index]
	{
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				_ => throw new IndexOutOfRangeException("Invalid Vector3 index!"), 
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
			default:
				throw new IndexOutOfRangeException("Invalid Vector3 index!");
			}
		}
	}

	public Vector3 normalized => Normalize(this);

	public float magnitude => Mathf.Sqrt(x * x + y * y + z * z);

	public float sqrMagnitude => x * x + y * y + z * z;

	public static Vector3 zero => zeroVector;

	public static Vector3 one => oneVector;

	public static Vector3 forward => forwardVector;

	public static Vector3 back => backVector;

	public static Vector3 up => upVector;

	public static Vector3 down => downVector;

	public static Vector3 left => leftVector;

	public static Vector3 right => rightVector;

	public Vector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3(float x, float y)
	{
		this.x = x;
		this.y = y;
		z = 0f;
	}

	public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
	{
		t = Mathf.Clamp01(t);
		return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
	}

	public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
	{
		return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
	}

	public static Vector3 Scale(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public void Scale(Vector3 scale)
	{
		x *= scale.x;
		y *= scale.y;
		z *= scale.z;
	}

	public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
	{
		return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector3 vector))
		{
			return false;
		}
		return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z);
	}

	public static Vector3 Normalize(Vector3 value)
	{
		float num = Magnitude(value);
		if (num > 1E-05f)
		{
			return value / num;
		}
		return zero;
	}

	public void Normalize()
	{
		float num = Magnitude(this);
		if (num > 1E-05f)
		{
			this /= num;
		}
		else
		{
			this = zero;
		}
	}

	public static float Dot(Vector3 lhs, Vector3 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
	}

	public static float Angle(Vector3 from, Vector3 to)
	{
		return Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
	}

	public static float Distance(Vector3 a, Vector3 b)
	{
		Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
	}

	public static float Magnitude(Vector3 vector)
	{
		return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
	}

	public static float SqrMagnitude(Vector3 vector)
	{
		return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
	}

	public static Vector3 Min(Vector3 lhs, Vector3 rhs)
	{
		return new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
	}

	public static Vector3 Max(Vector3 lhs, Vector3 rhs)
	{
		return new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
	}

	public static Vector3 operator +(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3 operator -(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector3 operator -(Vector3 a)
	{
		return new Vector3(0f - a.x, 0f - a.y, 0f - a.z);
	}

	public static Vector3 operator *(Vector3 a, float d)
	{
		return new Vector3(a.x * d, a.y * d, a.z * d);
	}

	public static Vector3 operator *(float d, Vector3 a)
	{
		return new Vector3(a.x * d, a.y * d, a.z * d);
	}

	public static Vector3 operator /(Vector3 a, float d)
	{
		return new Vector3(a.x / d, a.y / d, a.z / d);
	}

	public static bool operator ==(Vector3 lhs, Vector3 rhs)
	{
		return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
	}

	public static bool operator !=(Vector3 lhs, Vector3 rhs)
	{
		return !(lhs == rhs);
	}

	public override string ToString()
	{
		return UnityString.Format("({0:F1}, {1:F1}, {2:F1})", x, y, z);
	}
}
