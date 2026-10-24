namespace System.Globalization;

internal class CCFixed
{
	public static int FromDateTime(DateTime time)
	{
		return 1 + (int)(time.Ticks / 864000000000L);
	}

	public static DayOfWeek day_of_week(int date)
	{
		return (DayOfWeek)CCMath.mod(date, 7);
	}
}
