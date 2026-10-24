using System.Reflection;
using UnityEngine.Scripting;

namespace UnityEngine;

[DefaultMember("Item")]
[UsedByNativeCode]
public struct Vector3Int(int x, int y, int z)
{
	private int m_X = x;

	private int m_Y = y;

	private int m_Z = z;

	private static readonly Vector3Int s_Zero = new Vector3Int(0, 0, 0);

	private static readonly Vector3Int s_One = new Vector3Int(1, 1, 1);

	private static readonly Vector3Int s_Up = new Vector3Int(0, 1, 0);

	private static readonly Vector3Int s_Down = new Vector3Int(0, -1, 0);

	private static readonly Vector3Int s_Left = new Vector3Int(-1, 0, 0);

	private static readonly Vector3Int s_Right = new Vector3Int(1, 0, 0);

	public int x => m_X;

	public int y => m_Y;

	public int z => m_Z;

	public static bool operator ==(Vector3Int lhs, Vector3Int rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector3Int vector3Int))
		{
			return false;
		}
		return this == vector3Int;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
	}

	public override string ToString()
	{
		return UnityString.Format("({0}, {1}, {2})", x, y, z);
	}
}
