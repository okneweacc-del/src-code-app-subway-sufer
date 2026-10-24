namespace System.Text.RegularExpressions;

internal struct Interval : IComparable
{
	public int low;

	public int high;

	public bool contiguous;

	public static System.Text.RegularExpressions.Interval Empty
	{
		get
		{
			System.Text.RegularExpressions.Interval result = default;
			result.low = 0;
			result.high = result.low - 1;
			result.contiguous = true;
			return result;
		}
	}

	public bool IsDiscontiguous => !contiguous;

	public bool IsSingleton => contiguous && low == high;

	public bool IsEmpty => low > high;

	public int Size
	{
		get
		{
			if (IsEmpty)
			{
				return 0;
			}
			return high - low + 1;
		}
	}

	public Interval(int low, int high)
	{
		if (low > high)
		{
			int num = low;
			low = high;
			high = num;
		}
		this.low = low;
		this.high = high;
		contiguous = true;
	}

	public bool IsDisjoint(System.Text.RegularExpressions.Interval i)
	{
		if (IsEmpty || i.IsEmpty)
		{
			return true;
		}
		return low > i.high || i.low > high;
	}

	public bool IsAdjacent(System.Text.RegularExpressions.Interval i)
	{
		if (IsEmpty || i.IsEmpty)
		{
			return false;
		}
		return low == i.high + 1 || high == i.low - 1;
	}

	public bool Contains(System.Text.RegularExpressions.Interval i)
	{
		if (!IsEmpty && i.IsEmpty)
		{
			return true;
		}
		if (IsEmpty)
		{
			return false;
		}
		return low <= i.low && i.high <= high;
	}

	public bool Contains(int i)
	{
		return low <= i && i <= high;
	}

	public bool Intersects(System.Text.RegularExpressions.Interval i)
	{
		if (IsEmpty || i.IsEmpty)
		{
			return false;
		}
		return (Contains(i.low) && !Contains(i.high)) || (Contains(i.high) && !Contains(i.low));
	}

	public void Merge(System.Text.RegularExpressions.Interval i)
	{
		if (!i.IsEmpty)
		{
			if (IsEmpty)
			{
				low = i.low;
				high = i.high;
			}
			if (i.low < low)
			{
				low = i.low;
			}
			if (i.high > high)
			{
				high = i.high;
			}
		}
	}

	public int CompareTo(object o)
	{
		return low - ((System.Text.RegularExpressions.Interval)o).low;
	}
}
