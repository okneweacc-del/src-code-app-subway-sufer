using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyTitleAttribute : Attribute
{
	private string name;

	public AssemblyTitleAttribute(string title)
	{
		name = title;
	}
}
