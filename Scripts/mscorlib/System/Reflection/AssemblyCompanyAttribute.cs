using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyCompanyAttribute : Attribute
{
	private string name;

	public AssemblyCompanyAttribute(string company)
	{
		name = company;
	}
}
