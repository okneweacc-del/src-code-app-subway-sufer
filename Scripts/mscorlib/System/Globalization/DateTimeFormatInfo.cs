using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization;

[Serializable]
[ComVisible(true)]
public sealed class DateTimeFormatInfo : ICloneable, IFormatProvider
{
	private const string _RoundtripPattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

	private static readonly string MSG_READONLY = "This instance is read only";

	private static readonly string MSG_ARRAYSIZE_MONTH = "An array with exactly 13 elements is needed";

	private static readonly string MSG_ARRAYSIZE_DAY = "An array with exactly 7 elements is needed";

	private static readonly string[] INVARIANT_ABBREVIATED_DAY_NAMES = new string[7] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

	private static readonly string[] INVARIANT_DAY_NAMES = new string[7] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

	private static readonly string[] INVARIANT_ABBREVIATED_MONTH_NAMES = new string[13]
	{
		"Jan",
		"Feb",
		"Mar",
		"Apr",
		"May",
		"Jun",
		"Jul",
		"Aug",
		"Sep",
		"Oct",
		"Nov",
		"Dec",
		string.Empty
	};

	private static readonly string[] INVARIANT_MONTH_NAMES = new string[13]
	{
		"January",
		"February",
		"March",
		"April",
		"May",
		"June",
		"July",
		"August",
		"September",
		"October",
		"November",
		"December",
		string.Empty
	};

	private static readonly string[] INVARIANT_SHORT_DAY_NAMES = new string[7] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };

	private static DateTimeFormatInfo theInvariantDateTimeFormatInfo;

	private bool m_isReadOnly;

	private string amDesignator;

	private string pmDesignator;

	private string dateSeparator;

	private string timeSeparator;

	private string shortDatePattern;

	private string longDatePattern;

	private string shortTimePattern;

	private string longTimePattern;

	private string monthDayPattern;

	private string yearMonthPattern;

	private string fullDateTimePattern;

	private string _RFC1123Pattern;

	private string _SortableDateTimePattern;

	private string _UniversalSortableDateTimePattern;

	private int firstDayOfWeek;

	private Calendar calendar;

	private int calendarWeekRule;

	private string[] abbreviatedDayNames;

	private string[] dayNames;

	private string[] monthNames;

	private string[] abbreviatedMonthNames;

	private string[] allShortDatePatterns;

	private string[] allLongDatePatterns;

	private string[] allShortTimePatterns;

	private string[] allLongTimePatterns;

	private string[] monthDayPatterns;

	private string[] yearMonthPatterns;

	private string[] shortDayNames;

	private int nDataItem;

	private bool m_useUserOverride;

	private bool m_isDefaultCalendar;

	private int CultureID;

	private bool bUseCalendarInfo;

	private string generalShortTimePattern;

	private string generalLongTimePattern;

	private string[] m_eraNames;

	private string[] m_abbrevEraNames;

	private string[] m_abbrevEnglishEraNames;

	private string[] m_dateWords;

	private int[] optionalCalendars;

	private string[] m_superShortDayNames;

	private string[] genitiveMonthNames;

	private string[] m_genitiveAbbreviatedMonthNames;

	private string[] leapYearMonthNames;

	private DateTimeFormatFlags formatFlags;

	private string m_name;

	private volatile string[] all_date_time_patterns;

	public bool IsReadOnly => m_isReadOnly;

	internal string[] RawAbbreviatedDayNames => abbreviatedDayNames;

	public string[] AbbreviatedMonthNames => (string[])RawAbbreviatedMonthNames.Clone();

	internal string[] RawAbbreviatedMonthNames => abbreviatedMonthNames;

	internal string[] RawDayNames => dayNames;

	internal string[] RawMonthNames => monthNames;

	public string AMDesignator => amDesignator;

	public string PMDesignator => pmDesignator;

	public string DateSeparator => dateSeparator;

	public string TimeSeparator => timeSeparator;

	public string LongDatePattern => longDatePattern;

	public string ShortDatePattern => shortDatePattern;

	public string ShortTimePattern => shortTimePattern;

	public string LongTimePattern => longTimePattern;

	public string MonthDayPattern => monthDayPattern;

	public string YearMonthPattern => yearMonthPattern;

	public string FullDateTimePattern
	{
		get
		{
			if (fullDateTimePattern != null)
			{
				return fullDateTimePattern;
			}
			return longDatePattern + " " + longTimePattern;
		}
	}

	public static DateTimeFormatInfo CurrentInfo => Thread.CurrentThread.CurrentCulture.DateTimeFormat;

	public static DateTimeFormatInfo InvariantInfo
	{
		get
		{
			if (theInvariantDateTimeFormatInfo == null)
			{
				theInvariantDateTimeFormatInfo = ReadOnly(new DateTimeFormatInfo());
				theInvariantDateTimeFormatInfo.FillInvariantPatterns();
			}
			return theInvariantDateTimeFormatInfo;
		}
	}

	public Calendar Calendar
	{
		get
		{
			return calendar;
		}
		set
		{
			if (IsReadOnly)
			{
				throw new InvalidOperationException(MSG_READONLY);
			}
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			calendar = value;
		}
	}

	public string RFC1123Pattern => _RFC1123Pattern;

	internal string RoundtripPattern => "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

	public string SortableDateTimePattern => _SortableDateTimePattern;

	public string UniversalSortableDateTimePattern => _UniversalSortableDateTimePattern;

	internal DateTimeFormatInfo(bool read_only)
	{
		m_isReadOnly = read_only;
		amDesignator = "AM";
		pmDesignator = "PM";
		dateSeparator = "/";
		timeSeparator = ":";
		shortDatePattern = "MM/dd/yyyy";
		longDatePattern = "dddd, dd MMMM yyyy";
		shortTimePattern = "HH:mm";
		longTimePattern = "HH:mm:ss";
		monthDayPattern = "MMMM dd";
		yearMonthPattern = "yyyy MMMM";
		fullDateTimePattern = "dddd, dd MMMM yyyy HH:mm:ss";
		_RFC1123Pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
		_SortableDateTimePattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
		_UniversalSortableDateTimePattern = "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";
		firstDayOfWeek = 0;
		calendar = new GregorianCalendar();
		calendarWeekRule = 0;
		abbreviatedDayNames = INVARIANT_ABBREVIATED_DAY_NAMES;
		dayNames = INVARIANT_DAY_NAMES;
		abbreviatedMonthNames = INVARIANT_ABBREVIATED_MONTH_NAMES;
		monthNames = INVARIANT_MONTH_NAMES;
		m_genitiveAbbreviatedMonthNames = INVARIANT_ABBREVIATED_MONTH_NAMES;
		genitiveMonthNames = INVARIANT_MONTH_NAMES;
		shortDayNames = INVARIANT_SHORT_DAY_NAMES;
	}

	public DateTimeFormatInfo()
		: this(read_only: false)
	{
	}

	public static DateTimeFormatInfo GetInstance(IFormatProvider provider)
	{
		if (provider != null)
		{
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)provider.GetFormat(typeof(DateTimeFormatInfo));
			if (dateTimeFormatInfo != null)
			{
				return dateTimeFormatInfo;
			}
		}
		return CurrentInfo;
	}

	public static DateTimeFormatInfo ReadOnly(DateTimeFormatInfo dtfi)
	{
		DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)dtfi.Clone();
		dateTimeFormatInfo.m_isReadOnly = true;
		return dateTimeFormatInfo;
	}

	public object Clone()
	{
		DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)MemberwiseClone();
		dateTimeFormatInfo.m_isReadOnly = false;
		return dateTimeFormatInfo;
	}

	public object GetFormat(Type formatType)
	{
		return (formatType != GetType()) ? null : this;
	}

	public string GetAbbreviatedMonthName(int month)
	{
		if (month < 1 || month > 13)
		{
			throw new ArgumentOutOfRangeException();
		}
		return abbreviatedMonthNames[month - 1];
	}

	public string GetEraName(int era)
	{
		if (era < 0 || era > calendar.EraNames.Length)
		{
			throw new ArgumentOutOfRangeException("era", era.ToString());
		}
		return calendar.EraNames[era - 1];
	}

	public string GetMonthName(int month)
	{
		if (month < 1 || month > 13)
		{
			throw new ArgumentOutOfRangeException();
		}
		return monthNames[month - 1];
	}

	internal string[] GetAllDateTimePatternsInternal()
	{
		FillAllDateTimePatterns();
		return all_date_time_patterns;
	}

	private void FillAllDateTimePatterns()
	{
		if (all_date_time_patterns == null)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(GetAllRawDateTimePatterns('d'));
			arrayList.AddRange(GetAllRawDateTimePatterns('D'));
			arrayList.AddRange(GetAllRawDateTimePatterns('g'));
			arrayList.AddRange(GetAllRawDateTimePatterns('G'));
			arrayList.AddRange(GetAllRawDateTimePatterns('f'));
			arrayList.AddRange(GetAllRawDateTimePatterns('F'));
			arrayList.AddRange(GetAllRawDateTimePatterns('m'));
			arrayList.AddRange(GetAllRawDateTimePatterns('M'));
			arrayList.AddRange(GetAllRawDateTimePatterns('r'));
			arrayList.AddRange(GetAllRawDateTimePatterns('R'));
			arrayList.AddRange(GetAllRawDateTimePatterns('s'));
			arrayList.AddRange(GetAllRawDateTimePatterns('t'));
			arrayList.AddRange(GetAllRawDateTimePatterns('T'));
			arrayList.AddRange(GetAllRawDateTimePatterns('u'));
			arrayList.AddRange(GetAllRawDateTimePatterns('U'));
			arrayList.AddRange(GetAllRawDateTimePatterns('y'));
			arrayList.AddRange(GetAllRawDateTimePatterns('Y'));
			all_date_time_patterns = (string[])arrayList.ToArray(typeof(string));
		}
	}

	internal string[] GetAllRawDateTimePatterns(char format)
	{
		switch (format)
		{
		case 'D':
			if (allLongDatePatterns == null || allLongDatePatterns.Length <= 0)
			{
				return new string[1] { LongDatePattern };
			}
			return allLongDatePatterns;
		case 'd':
			if (allShortDatePatterns == null || allShortDatePatterns.Length <= 0)
			{
				return new string[1] { ShortDatePattern };
			}
			return allShortDatePatterns;
		case 'T':
			if (allLongTimePatterns == null || allLongTimePatterns.Length <= 0)
			{
				return new string[1] { LongTimePattern };
			}
			return allLongTimePatterns;
		case 't':
			if (allShortTimePatterns == null || allShortTimePatterns.Length <= 0)
			{
				return new string[1] { ShortTimePattern };
			}
			return allShortTimePatterns;
		case 'G':
		{
			string[] array = PopulateCombinedList(allShortDatePatterns, allLongTimePatterns);
			if (array == null || array.Length <= 0)
			{
				return new string[1] { ShortDatePattern + " " + LongTimePattern };
			}
			return array;
		}
		case 'g':
		{
			string[] array = PopulateCombinedList(allShortDatePatterns, allShortTimePatterns);
			if (array == null || array.Length <= 0)
			{
				return new string[1] { ShortDatePattern + " " + ShortTimePattern };
			}
			return array;
		}
		case 'F':
		case 'U':
		{
			string[] array = PopulateCombinedList(allLongDatePatterns, allLongTimePatterns);
			if (array == null || array.Length <= 0)
			{
				return new string[1] { LongDatePattern + " " + LongTimePattern };
			}
			return array;
		}
		case 'f':
		{
			string[] array = PopulateCombinedList(allLongDatePatterns, allShortTimePatterns);
			if (array == null || array.Length <= 0)
			{
				return new string[1] { LongDatePattern + " " + ShortTimePattern };
			}
			return array;
		}
		case 'M':
		case 'm':
			if (monthDayPatterns == null || monthDayPatterns.Length <= 0)
			{
				return new string[1] { MonthDayPattern };
			}
			return monthDayPatterns;
		case 'Y':
		case 'y':
			if (yearMonthPatterns == null || yearMonthPatterns.Length <= 0)
			{
				return new string[1] { YearMonthPattern };
			}
			return yearMonthPatterns;
		case 'R':
		case 'r':
			return new string[1] { RFC1123Pattern };
		case 's':
			return new string[1] { SortableDateTimePattern };
		case 'u':
			return new string[1] { UniversalSortableDateTimePattern };
		default:
			throw new ArgumentException("Format specifier was invalid.");
		}
	}

	public string GetDayName(DayOfWeek dayofweek)
	{
		if (dayofweek < DayOfWeek.Sunday || dayofweek > DayOfWeek.Saturday)
		{
			throw new ArgumentOutOfRangeException();
		}
		return dayNames[(int)dayofweek];
	}

	public string GetAbbreviatedDayName(DayOfWeek dayofweek)
	{
		if (dayofweek < DayOfWeek.Sunday || dayofweek > DayOfWeek.Saturday)
		{
			throw new ArgumentOutOfRangeException();
		}
		return abbreviatedDayNames[(int)dayofweek];
	}

	private void FillInvariantPatterns()
	{
		allShortDatePatterns = new string[1] { "MM/dd/yyyy" };
		allLongDatePatterns = new string[1] { "dddd, dd MMMM yyyy" };
		allLongTimePatterns = new string[1] { "HH:mm:ss" };
		allShortTimePatterns = new string[4] { "HH:mm", "hh:mm tt", "H:mm", "h:mm tt" };
		monthDayPatterns = new string[1] { "MMMM dd" };
		yearMonthPatterns = new string[1] { "yyyy MMMM" };
	}

	private string[] PopulateCombinedList(string[] dates, string[] times)
	{
		if (dates != null && times != null)
		{
			string[] array = new string[dates.Length * times.Length];
			int num = 0;
			foreach (string text in dates)
			{
				foreach (string text2 in times)
				{
					array[num++] = text + " " + text2;
				}
			}
			return array;
		}
		return null;
	}
}
