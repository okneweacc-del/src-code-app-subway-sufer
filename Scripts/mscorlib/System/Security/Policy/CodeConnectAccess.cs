using System.Runtime.InteropServices;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public class CodeConnectAccess
{
	public static readonly string AnyScheme = "*";

	public static readonly int DefaultPort = -3;

	public static readonly int OriginPort = -4;

	public static readonly string OriginScheme = "$origin";

	private string _scheme;

	private int _port;

	public override bool Equals(object o)
	{
		if (!(o is CodeConnectAccess codeConnectAccess))
		{
			return false;
		}
		return _scheme == codeConnectAccess._scheme && _port == codeConnectAccess._port;
	}

	public override int GetHashCode()
	{
		return _scheme.GetHashCode() ^ _port;
	}
}
