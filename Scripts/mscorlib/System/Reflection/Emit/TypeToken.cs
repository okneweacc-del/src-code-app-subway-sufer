using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[Serializable]
[ComVisible(true)]
public struct TypeToken
{
	internal int tokValue;

	public static readonly TypeToken Empty;

	internal TypeToken(int val)
	{
		tokValue = val;
	}

	static TypeToken()
	{
		Empty = default;
	}

	public override bool Equals(object obj)
	{
		bool flag = obj is TypeToken;
		if (flag)
		{
			TypeToken typeToken = (TypeToken)obj;
			flag = tokValue == typeToken.tokValue;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return tokValue;
	}
}
