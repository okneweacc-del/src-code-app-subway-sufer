using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyConfigurationAttribute : Attribute
{
	private string name;

	public AssemblyConfigurationAttribute(string configuration)
	{
		name = configuration;
	}
}
