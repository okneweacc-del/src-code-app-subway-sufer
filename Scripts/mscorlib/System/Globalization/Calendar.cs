using System.Runtime.InteropServices;

namespace System.Globalization;

[Serializable]
[ComVisible(true)]
public abstract class Calendar : ICloneable
{
	public const int CurrentEra = 0;

	[NonSerialized]
	private bool m_isReadOnly;

	[NonSerialized]
	internal int twoDigitYearMax;

	[NonSerialized]
	private int M_MaxYearValue;

	[NonSerialized]
	internal string[] M_AbbrEraNames;

	[NonSerialized]
	internal string[] M_EraNames;

	internal int m_currentEraValue;

	public abstract int[] Eras { get; }

	internal string[] EraNames
	{
		get
		{
			if (M_EraNames == null || M_EraNames.Length != Eras.Length)
			{
				throw new Exception("Internal: M_EraNames not initialized!");
			}
			return (string[])M_EraNames.Clone();
		}
	}

	protected Calendar()
	{
		twoDigitYearMax = 99;
	}

	[ComVisible(false)]
	public virtual object Clone()
	{
		Calendar calendar = (Calendar)MemberwiseClone();
		calendar.m_isReadOnly = false;
		return calendar;
	}

	internal void CheckReadOnly()
	{
		if (m_isReadOnly)
		{
			throw new InvalidOperationException("This Calendar is read-only.");
		}
	}

	public abstract int GetDayOfMonth(DateTime time);

	public abstract DayOfWeek GetDayOfWeek(DateTime time);

	public abstract int GetEra(DateTime time);

	public abstract int GetMonth(DateTime time);

	public abstract int GetYear(DateTime time);
}
