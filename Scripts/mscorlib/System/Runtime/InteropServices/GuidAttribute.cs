namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
[ComVisible(true)]
public sealed class GuidAttribute : Attribute
{
	private string guidValue;

	public string Value => guidValue;

	public GuidAttribute(string guid)
	{
		guidValue = guid;
	}
}
