namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, Inherited = false)]
[ComVisible(true)]
public sealed class DispIdAttribute : Attribute
{
	private int id;

	public DispIdAttribute(int dispId)
	{
		id = dispId;
	}
}
