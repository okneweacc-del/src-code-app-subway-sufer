using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyDescriptionAttribute : Attribute
{
	private string name;

	public AssemblyDescriptionAttribute(string description)
	{
		name = description;
	}
}
