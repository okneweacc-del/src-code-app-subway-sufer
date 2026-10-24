using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyDelaySignAttribute : Attribute
{
	private bool delay;

	public AssemblyDelaySignAttribute(bool delaySign)
	{
		delay = delaySign;
	}
}
