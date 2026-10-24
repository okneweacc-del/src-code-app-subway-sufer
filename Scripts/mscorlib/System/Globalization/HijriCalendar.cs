using System.IO;
using System.Runtime.InteropServices;

namespace System.Globalization;

[Serializable]
[MonoTODO("Serialization format not compatible with .NET")]
[ComVisible(true)]
public class HijriCalendar : Calendar
{
	public static readonly int HijriEra = 1;

	internal static readonly int M_MinFixed = CCHijriCalendar.fixed_from_dmy(1, 1, 1);

	internal static readonly int M_MaxFixed = CCGregorianCalendar.fixed_from_dmy(31, 12, 9999);

	internal int M_AddHijriDate;

	private static DateTime Min = new DateTime(622, 7, 18, 0, 0, 0);

	private static DateTime Max = new DateTime(9999, 12, 31, 11, 59, 59);

	public override int[] Eras => new int[1] { HijriEra };

	internal virtual int AddHijriDate => M_AddHijriDate;

	public HijriCalendar()
	{
		M_AbbrEraNames = new string[1] { "A.H." };
		M_EraNames = new string[1] { "Anno Hegirae" };
		if (twoDigitYearMax == 99)
		{
			twoDigitYearMax = 1451;
		}
	}

	internal void M_CheckFixedHijri(string param, int rdHijri)
	{
		if (rdHijri < M_MinFixed || rdHijri > M_MaxFixed - AddHijriDate)
		{
			StringWriter stringWriter = new StringWriter();
			CCHijriCalendar.dmy_from_fixed(out var day, out var month, out var year, M_MaxFixed - AddHijriDate);
			if (AddHijriDate != 0)
			{
				stringWriter.Write("This HijriCalendar (AddHijriDate {0}) allows dates from 1. 1. 1 to {1}. {2}. {3}.", AddHijriDate, day, month, year);
			}
			else
			{
				stringWriter.Write("HijriCalendar allows dates from 1.1.1 to {0}.{1}.{2}.", day, month, year);
			}
			throw new ArgumentOutOfRangeException(param, stringWriter.ToString());
		}
	}

	internal void M_CheckDateTime(DateTime time)
	{
		int rdHijri = CCFixed.FromDateTime(time) - AddHijriDate;
		M_CheckFixedHijri("time", rdHijri);
	}

	internal int M_FromDateTime(DateTime time)
	{
		return CCFixed.FromDateTime(time) - AddHijriDate;
	}

	public override int GetDayOfMonth(DateTime time)
	{
		int num = M_FromDateTime(time);
		M_CheckFixedHijri("time", num);
		return CCHijriCalendar.day_from_fixed(num);
	}

	public override DayOfWeek GetDayOfWeek(DateTime time)
	{
		int num = M_FromDateTime(time);
		M_CheckFixedHijri("time", num);
		return CCFixed.day_of_week(num);
	}

	public override int GetEra(DateTime time)
	{
		M_CheckDateTime(time);
		return HijriEra;
	}

	public override int GetMonth(DateTime time)
	{
		int num = M_FromDateTime(time);
		M_CheckFixedHijri("time", num);
		return CCHijriCalendar.month_from_fixed(num);
	}

	public override int GetYear(DateTime time)
	{
		int num = M_FromDateTime(time);
		M_CheckFixedHijri("time", num);
		return CCHijriCalendar.year_from_fixed(num);
	}
}
