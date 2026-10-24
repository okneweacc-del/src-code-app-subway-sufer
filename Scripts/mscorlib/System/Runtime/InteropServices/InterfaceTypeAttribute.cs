namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class InterfaceTypeAttribute : Attribute
{
	private ComInterfaceType intType;

	public InterfaceTypeAttribute(ComInterfaceType interfaceType)
	{
		intType = interfaceType;
	}
}
