using System.Runtime.InteropServices;

namespace System.Diagnostics;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
[ComVisible(true)]
public sealed class DebuggerBrowsableAttribute : Attribute
{
	private DebuggerBrowsableState state;

	public DebuggerBrowsableAttribute(DebuggerBrowsableState state)
	{
		this.state = state;
	}
}
