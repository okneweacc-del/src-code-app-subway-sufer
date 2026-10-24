using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[Serializable]
[ComVisible(true)]
public struct FieldToken
{
	internal int tokValue;

	public static readonly FieldToken Empty;

	internal FieldToken(int val)
	{
		tokValue = val;
	}

	static FieldToken()
	{
		Empty = default;
	}

	public override bool Equals(object obj)
	{
		bool flag = obj is FieldToken;
		if (flag)
		{
			FieldToken fieldToken = (FieldToken)obj;
			flag = tokValue == fieldToken.tokValue;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return tokValue;
	}
}
