using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore;

[ComVisible(true)]
public struct SymbolToken(int val)
{
	private int _val = val;

	public override bool Equals(object obj)
	{
		if (!(obj is SymbolToken symbolToken))
		{
			return false;
		}
		return symbolToken.GetToken() == _val;
	}

	public override int GetHashCode()
	{
		return _val.GetHashCode();
	}

	public int GetToken()
	{
		return _val;
	}
}
