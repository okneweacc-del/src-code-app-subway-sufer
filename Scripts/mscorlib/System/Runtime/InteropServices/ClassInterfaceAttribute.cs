namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, Inherited = false)]
public sealed class ClassInterfaceAttribute : Attribute
{
	private ClassInterfaceType ciType;

	public ClassInterfaceAttribute(ClassInterfaceType classInterfaceType)
	{
		ciType = classInterfaceType;
	}
}
