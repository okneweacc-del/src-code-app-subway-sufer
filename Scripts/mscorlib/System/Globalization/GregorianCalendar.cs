using System.Runtime.InteropServices;

namespace System.Globalization;

[Serializable]
[MonoTODO("Serialization format not compatible with .NET")]
[ComVisible(true)]
public class GregorianCalendar : Calendar
{
	[NonSerialized]
	internal GregorianCalendarTypes m_type;

	public override int[] Eras => new int[1] { 1 };

	public virtual GregorianCalendarTypes CalendarType
	{
		set
		{
			CheckReadOnly();
			m_type = value;
		}
	}

	public GregorianCalendar(GregorianCalendarTypes type)
	{
		CalendarType = type;
		M_AbbrEraNames = new string[1] { "AD" };
		M_EraNames = new string[1] { "A.D." };
		if (twoDigitYearMax == 99)
		{
			twoDigitYearMax = 2029;
		}
	}

	public GregorianCalendar()
		: this(GregorianCalendarTypes.Localized)
	{
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
		return 1;
	}

	public override int GetMonth(DateTime time)
	{
		return CCGregorianCalendar.GetMonth(time);
	}

	public override int GetYear(DateTime time)
	{
		return CCGregorianCalendar.GetYear(time);
	}
}
