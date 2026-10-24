namespace UnityEngine;

public struct RangeInt(int start, int length)
{
	public int start = start;

	public int length = length;

	public int end => start + length;
}
