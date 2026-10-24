namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
[ComVisible(true)]
public sealed class UnmanagedFunctionPointerAttribute : Attribute
{
	private CallingConvention call_conv;

	public UnmanagedFunctionPointerAttribute(CallingConvention callingConvention)
	{
		call_conv = callingConvention;
	}
}
