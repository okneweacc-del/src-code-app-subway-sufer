using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ComVisible(true)]
public struct SignatureToken
{
	internal int tokValue;

	public static readonly SignatureToken Empty;

	internal SignatureToken(int val)
	{
		tokValue = val;
	}

	static SignatureToken()
	{
		Empty = default;
	}

	public override bool Equals(object obj)
	{
		bool flag = obj is SignatureToken;
		if (flag)
		{
			SignatureToken signatureToken = (SignatureToken)obj;
			flag = tokValue == signatureToken.tokValue;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return tokValue;
	}
}
