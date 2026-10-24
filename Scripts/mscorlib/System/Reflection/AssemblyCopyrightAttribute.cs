using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyCopyrightAttribute : Attribute
{
	private string name;

	public AssemblyCopyrightAttribute(string copyright)
	{
		name = copyright;
	}
}
