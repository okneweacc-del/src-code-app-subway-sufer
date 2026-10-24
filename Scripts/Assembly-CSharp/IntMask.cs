public struct IntMask(int i)
{
	private int mask = i;

	public bool this[int bit]
	{
		get
		{
			return (mask & (1 << bit)) != 0;
		}
		set
		{
			if (value)
			{
				mask |= 1 << bit;
			}
			else
			{
				mask &= ~(1 << bit);
			}
		}
	}

	public static implicit operator int(IntMask i)
	{
		return i.mask;
	}

	public static implicit operator IntMask(int i)
	{
		return new IntMask(i);
	}
}
