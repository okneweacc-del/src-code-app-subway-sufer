namespace System.Runtime.CompilerServices;

public static class RuntimeHelpers
{
	public static extern int OffsetToStringData
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InitializeArray(Array array, IntPtr fldHandle);

	public static void InitializeArray(Array array, RuntimeFieldHandle fldHandle)
	{
		if (array == null || fldHandle.Value == IntPtr.Zero)
		{
			throw new ArgumentNullException();
		}
		InitializeArray(array, fldHandle.Value);
	}
}
