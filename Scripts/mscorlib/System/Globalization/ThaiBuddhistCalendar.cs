using System.Runtime.InteropServices;

namespace System.Globalization;

[Serializable]
[MonoTODO("Serialization format not compatible with.NET")]
[ComVisible(true)]
public class ThaiBuddhistCalendar : Calendar
{
	internal static readonly CCGregorianEraHandler M_EraHandler;

	private static DateTime ThaiMin;

	private static DateTime ThaiMax;

	public override int[] Eras => (int[])M_EraHandler.Eras.Clone();

	public ThaiBuddhistCalendar()
	{
		M_AbbrEraNames = new string[1] { "T.B.C.E." };
		M_EraNames = new string[1] { "ThaiBuddhist current era" };
		if (twoDigitYearMax == 99)
		{
			twoDigitYearMax = 2572;
		}
	}

	static ThaiBuddhistCalendar()
	{
		ThaiMin = new DateTime(1, 1, 1, 0, 0, 0);
		ThaiMax = new DateTime(9999, 12, 31, 11, 59, 59);
		M_EraHandler = new CCGregorianEraHandler();
		M_EraHandler.appendEra(1, CCGregorianCalendar.fixed_from_dmy(1, 1, -542));
	}

	public override int GetDayOfMonth(DateTime time)
	{
		return CCGregorianCalendar.GetDayOfMonth(time);
	}

	public override DayOfWeek GetDayOfWeek(DateTime time)
	{
		int date = CCFixed.FromDateTime(time);
		return CCFixed.day_of_week(date);
	}

	public override int GetEra(DateTime time)
	{
		int date = CCFixed.FromDateTime(time);
		M_EraHandler.EraYear(out var era, date);
		return era;
	}

	public override int GetMonth(DateTime time)
	{
		return CCGregorianCalendar.GetMonth(time);
	}

	public override int GetYear(DateTime time)
	{
		int date = CCFixed.FromDateTime(time);
		int era;
		return M_EraHandler.EraYear(out era, date);
	}
}
