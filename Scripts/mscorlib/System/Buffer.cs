using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

[ComVisible(true)]
public static class Buffer
{
	public static int ByteLength(Array array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		int num = ByteLengthInternal(array);
		if (num < 0)
		{
			throw new ArgumentException(Locale.GetText("Object must be an array of primitives."));
		}
		return num;
	}

	public static void BlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dst == null)
		{
			throw new ArgumentNullException("dst");
		}
		if (srcOffset < 0)
		{
			throw new ArgumentOutOfRangeException("srcOffset", Locale.GetText("Non-negative number required."));
		}
		if (dstOffset < 0)
		{
			throw new ArgumentOutOfRangeException("dstOffset", Locale.GetText("Non-negative number required."));
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", Locale.GetText("Non-negative number required."));
		}
		if (!BlockCopyInternal(src, srcOffset, dst, dstOffset, count) && (srcOffset > ByteLength(src) - count || dstOffset > ByteLength(dst) - count))
		{
			throw new ArgumentException(Locale.GetText("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ByteLengthInternal(Array array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool BlockCopyInternal(Array src, int src_offset, Array dest, int dest_offset, int count);
}
