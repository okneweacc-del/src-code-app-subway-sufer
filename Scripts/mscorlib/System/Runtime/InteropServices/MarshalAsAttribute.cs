namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
public sealed class MarshalAsAttribute : Attribute
{
	private UnmanagedType utype;

	public UnmanagedType ArraySubType;

	public string MarshalCookie;

	[ComVisible(true)]
	public string MarshalType;

	[ComVisible(true)]
	public Type MarshalTypeRef;

	public int SizeConst;

	public short SizeParamIndex;

	public MarshalAsAttribute(UnmanagedType unmanagedType)
	{
		utype = unmanagedType;
	}
}
