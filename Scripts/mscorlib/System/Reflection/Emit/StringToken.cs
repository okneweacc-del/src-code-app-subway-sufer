using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[Serializable]
[ComVisible(true)]
public struct StringToken
{
	internal int tokValue;

	internal StringToken(int val)
	{
		tokValue = val;
	}

	static StringToken()
	{
	}

	public override bool Equals(object obj)
	{
		bool flag = obj is StringToken;
		if (flag)
		{
			StringToken stringToken = (StringToken)obj;
			flag = tokValue == stringToken.tokValue;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return tokValue;
	}
}
