using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeType(Header = "Runtime/Math/Matrix4x4.h")]
[NativeHeader("Runtime/Math/MathScripting.h")]
[ThreadAndSerializationSafe]
public struct Matrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
{
	public float m00 = column0.x;

	public float m10 = column0.y;

	public float m20 = column0.z;

	public float m30 = column0.w;

	public float m01 = column1.x;

	public float m11 = column1.y;

	public float m21 = column1.z;

	public float m31 = column1.w;

	public float m02 = column2.x;

	public float m12 = column2.y;

	public float m22 = column2.z;

	public float m32 = column2.w;

	public float m03 = column3.x;

	public float m13 = column3.y;

	public float m23 = column3.z;

	public float m33 = column3.w;

	private static readonly Matrix4x4 zeroMatrix = new Matrix4x4(new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f));

	private static readonly Matrix4x4 identityMatrix = new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f));

	public Matrix4x4 inverse => Inverse(this);

	public float this[int row, int column]
	{
		set
		{
			this[row + column * 4] = value;
		}
	}

	public float this[int index]
	{
		get
		{
			return index switch
			{
				0 => m00, 
				1 => m10, 
				2 => m20, 
				3 => m30, 
				4 => m01, 
				5 => m11, 
				6 => m21, 
				7 => m31, 
				8 => m02, 
				9 => m12, 
				10 => m22, 
				11 => m32, 
				12 => m03, 
				13 => m13, 
				14 => m23, 
				15 => m33, 
				_ => throw new IndexOutOfRangeException("Invalid matrix index!"), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				m00 = value;
				break;
			case 1:
				m10 = value;
				break;
			case 2:
				m20 = value;
				break;
			case 3:
				m30 = value;
				break;
			case 4:
				m01 = value;
				break;
			case 5:
				m11 = value;
				break;
			case 6:
				m21 = value;
				break;
			case 7:
				m31 = value;
				break;
			case 8:
				m02 = value;
				break;
			case 9:
				m12 = value;
				break;
			case 10:
				m22 = value;
				break;
			case 11:
				m32 = value;
				break;
			case 12:
				m03 = value;
				break;
			case 13:
				m13 = value;
				break;
			case 14:
				m23 = value;
				break;
			case 15:
				m33 = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid matrix index!");
			}
		}
	}

	public static Matrix4x4 zero => zeroMatrix;

	public static Matrix4x4 identity => identityMatrix;

	[FreeFunction("MatrixScripting::TRS", IsThreadSafe = true)]
	public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
	{
		TRS_Injected(ref pos, ref q, ref s, out var ret);
		return ret;
	}

	public void SetTRS(Vector3 pos, Quaternion q, Vector3 s)
	{
		this = TRS(pos, q, s);
	}

	[FreeFunction("MatrixScripting::Inverse", IsThreadSafe = true)]
	public static Matrix4x4 Inverse(Matrix4x4 m)
	{
		Inverse_Injected(ref m, out var ret);
		return ret;
	}

	public override int GetHashCode()
	{
		return GetColumn(0).GetHashCode() ^ (GetColumn(1).GetHashCode() << 2) ^ (GetColumn(2).GetHashCode() >> 2) ^ (GetColumn(3).GetHashCode() >> 1);
	}

	public override bool Equals(object other)
	{
		if (!(other is Matrix4x4 matrix4x))
		{
			return false;
		}
		return GetColumn(0).Equals(matrix4x.GetColumn(0)) && GetColumn(1).Equals(matrix4x.GetColumn(1)) && GetColumn(2).Equals(matrix4x.GetColumn(2)) && GetColumn(3).Equals(matrix4x.GetColumn(3));
	}

	public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
	{
		Matrix4x4 result = default;
		result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
		result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
		result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
		result.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;
		result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
		result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
		result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
		result.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;
		result.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
		result.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
		result.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
		result.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;
		result.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
		result.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
		result.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
		result.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
		return result;
	}

	public static Vector4 operator *(Matrix4x4 lhs, Vector4 vector)
	{
		Vector4 result = default;
		result.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
		result.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
		result.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
		result.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;
		return result;
	}

	public Vector4 GetColumn(int index)
	{
		return index switch
		{
			0 => new Vector4(m00, m10, m20, m30), 
			1 => new Vector4(m01, m11, m21, m31), 
			2 => new Vector4(m02, m12, m22, m32), 
			3 => new Vector4(m03, m13, m23, m33), 
			_ => throw new IndexOutOfRangeException("Invalid column index!"), 
		};
	}

	public Vector3 MultiplyPoint(Vector3 point)
	{
		Vector3 result = default;
		result.x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
		result.y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
		result.z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
		float num = m30 * point.x + m31 * point.y + m32 * point.z + m33;
		num = 1f / num;
		result.x *= num;
		result.y *= num;
		result.z *= num;
		return result;
	}

	public Vector3 MultiplyPoint3x4(Vector3 point)
	{
		Vector3 result = default;
		result.x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
		result.y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
		result.z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
		return result;
	}

	public Vector3 MultiplyVector(Vector3 vector)
	{
		Vector3 result = default;
		result.x = m00 * vector.x + m01 * vector.y + m02 * vector.z;
		result.y = m10 * vector.x + m11 * vector.y + m12 * vector.z;
		result.z = m20 * vector.x + m21 * vector.y + m22 * vector.z;
		return result;
	}

	public static Matrix4x4 Scale(Vector3 vector)
	{
		Matrix4x4 result = default;
		result.m00 = vector.x;
		result.m01 = 0f;
		result.m02 = 0f;
		result.m03 = 0f;
		result.m10 = 0f;
		result.m11 = vector.y;
		result.m12 = 0f;
		result.m13 = 0f;
		result.m20 = 0f;
		result.m21 = 0f;
		result.m22 = vector.z;
		result.m23 = 0f;
		result.m30 = 0f;
		result.m31 = 0f;
		result.m32 = 0f;
		result.m33 = 1f;
		return result;
	}

	public override string ToString()
	{
		return UnityString.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n", m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TRS_Injected(ref Vector3 pos, ref Quaternion q, ref Vector3 s, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Inverse_Injected(ref Matrix4x4 m, out Matrix4x4 ret);
}
