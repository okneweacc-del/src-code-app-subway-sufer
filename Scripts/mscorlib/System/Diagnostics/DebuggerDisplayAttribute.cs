using System.Runtime.InteropServices;

namespace System.Diagnostics;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Delegate, AllowMultiple = true)]
[ComVisible(true)]
public sealed class DebuggerDisplayAttribute : Attribute
{
	private string value;

	private string type;

	private string name;

	public string Name
	{
		set
		{
			name = value;
		}
	}

	public DebuggerDisplayAttribute(string value)
	{
		if (value == null)
		{
			value = string.Empty;
		}
		this.value = value;
		type = string.Empty;
		name = string.Empty;
	}
}
