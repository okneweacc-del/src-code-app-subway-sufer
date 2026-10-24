using System.Runtime.InteropServices;

namespace System.Diagnostics;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class DebuggerTypeProxyAttribute : Attribute
{
	private string proxy_type_name;

	public DebuggerTypeProxyAttribute(Type type)
	{
		proxy_type_name = type.Name;
	}
}
