using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[Serializable]
[ComVisible(true)]
public struct MethodToken
{
	internal int tokValue;

	public static readonly MethodToken Empty;

	public int Token => tokValue;

	internal MethodToken(int val)
	{
		tokValue = val;
	}

	static MethodToken()
	{
		Empty = default;
	}

	public override bool Equals(object obj)
	{
		bool flag = obj is MethodToken;
		if (flag)
		{
			MethodToken methodToken = (MethodToken)obj;
			flag = tokValue == methodToken.tokValue;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return tokValue;
	}
}
