using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyInformationalVersionAttribute : Attribute
{
	private string name;

	public AssemblyInformationalVersionAttribute(string informationalVersion)
	{
		name = informationalVersion;
	}
}
