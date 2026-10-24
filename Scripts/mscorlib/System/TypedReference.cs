using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

[ComVisible(true)]
[CLSCompliant(false)]
public struct TypedReference
{
	private RuntimeTypeHandle type;

	private IntPtr value;

	private IntPtr klass;

	public override bool Equals(object o)
	{
		throw new NotSupportedException(Locale.GetText("This operation is not supported for this type."));
	}

	public override int GetHashCode()
	{
		if (type.Value == IntPtr.Zero)
		{
			return 0;
		}
		return Type.GetTypeFromHandle(type).GetHashCode();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern object ToObject(TypedReference value);
}
