namespace System.Globalization;

internal class CCGregorianCalendar
{
	public static bool is_leap_year(int year)
	{
		if (CCMath.mod(year, 4) != 0)
		{
			return false;
		}
		return CCMath.mod(year, 400) switch
		{
			100 => false, 
			200 => false, 
			300 => false, 
			_ => true, 
		};
	}

	public static int fixed_from_dmy(int day, int month, int year)
	{
		int num = 0;
		num += 365 * (year - 1);
		num += CCMath.div(year - 1, 4);
		num -= CCMath.div(year - 1, 100);
		num += CCMath.div(year - 1, 400);
		num += CCMath.div(367 * month - 362, 12);
		if (month > 2)
		{
			num += ((!is_leap_year(year)) ? (-2) : (-1));
		}
		return num + day;
	}

	public static int year_from_fixed(int date)
	{
		int remainder = date - 1;
		int num = CCMath.div_mod(out remainder, remainder, 146097);
		int num2 = CCMath.div_mod(out remainder, remainder, 36524);
		int num3 = CCMath.div_mod(out remainder, remainder, 1461);
		int num4 = CCMath.div(remainder, 365);
		int num5 = 400 * num + 100 * num2 + 4 * num3 + num4;
		return (num2 != 4 && num4 != 4) ? (num5 + 1) : num5;
	}

	public static void my_from_fixed(out int month, out int year, int date)
	{
		year = year_from_fixed(date);
		int num = date - fixed_from_dmy(1, 1, year);
		int num2 = ((date >= fixed_from_dmy(1, 3, year)) ? (is_leap_year(year) ? 1 : 2) : 0);
		month = CCMath.div(12 * (num + num2) + 373, 367);
	}

	public static void dmy_from_fixed(out int day, out int month, out int year, int date)
	{
		my_from_fixed(out month, out year, date);
		day = date - fixed_from_dmy(1, month, year) + 1;
	}

	public static int month_from_fixed(int date)
	{
		my_from_fixed(out var month, out var _, date);
		return month;
	}

	public static int day_from_fixed(int date)
	{
		dmy_from_fixed(out var day, out var _, out var _, date);
		return day;
	}

	public static int GetDayOfMonth(DateTime time)
	{
		return day_from_fixed(CCFixed.FromDateTime(time));
	}

	public static int GetMonth(DateTime time)
	{
		return month_from_fixed(CCFixed.FromDateTime(time));
	}

	public static int GetYear(DateTime time)
	{
		return year_from_fixed(CCFixed.FromDateTime(time));
	}
}
