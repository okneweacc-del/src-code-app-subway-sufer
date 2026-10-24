using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[StructLayout((LayoutKind)3)]
public struct DateTimeOffset : IFormattable, IComparable, ISerializable, IComparable<DateTimeOffset>, IEquatable<DateTimeOffset>, IDeserializationCallback
{
	public static readonly DateTimeOffset MaxValue;

	public static readonly DateTimeOffset MinValue;

	private DateTime dt;

	private TimeSpan utc_offset;

	public DateTime DateTime => DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);

	public static DateTimeOffset Now => new DateTimeOffset(DateTime.Now);

	public TimeSpan Offset => utc_offset;

	public long Ticks => dt.Ticks;

	public DateTime UtcDateTime => DateTime.SpecifyKind(dt - utc_offset, DateTimeKind.Utc);

	public static DateTimeOffset UtcNow => new DateTimeOffset(DateTime.UtcNow);

	public DateTimeOffset(DateTime dateTime)
	{
		dt = dateTime;
		if (dateTime.Kind == DateTimeKind.Utc)
		{
			utc_offset = TimeSpan.Zero;
		}
		else
		{
			utc_offset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
		}
		if (UtcDateTime < DateTime.MinValue || UtcDateTime > DateTime.MaxValue)
		{
			throw new ArgumentOutOfRangeException("The UTC date and time that results from applying the offset is earlier than MinValue or later than MaxValue.");
		}
	}

	public DateTimeOffset(DateTime dateTime, TimeSpan offset)
	{
		if (dateTime.Kind == DateTimeKind.Utc && offset != TimeSpan.Zero)
		{
			throw new ArgumentException("dateTime.Kind equals Utc and offset does not equal zero.");
		}
		if (dateTime.Kind == DateTimeKind.Local && offset != TimeZone.CurrentTimeZone.GetUtcOffset(dateTime))
		{
			throw new ArgumentException("dateTime.Kind equals Local and offset does not equal the offset of the system's local time zone.");
		}
		if (offset.Ticks % 600000000 != 0L)
		{
			throw new ArgumentException("offset is not specified in whole minutes.");
		}
		if (offset < new TimeSpan(-14, 0, 0) || offset > new TimeSpan(14, 0, 0))
		{
			throw new ArgumentOutOfRangeException("offset is less than -14 hours or greater than 14 hours.");
		}
		dt = dateTime;
		utc_offset = offset;
		if (UtcDateTime < DateTime.MinValue || UtcDateTime > DateTime.MaxValue)
		{
			throw new ArgumentOutOfRangeException("The UtcDateTime property is earlier than MinValue or later than MaxValue.");
		}
	}

	public DateTimeOffset(long ticks, TimeSpan offset)
		: this(new DateTime(ticks), offset)
	{
	}

	private DateTimeOffset(SerializationInfo info, StreamingContext context)
	{
		DateTime dateTime = (DateTime)info.GetValue("DateTime", typeof(DateTime));
		short @int = info.GetInt16("OffsetMinutes");
		utc_offset = TimeSpan.FromMinutes(@int);
		dt = dateTime.Add(utc_offset);
	}

	static DateTimeOffset()
	{
		MaxValue = new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero);
		MinValue = new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);
		if (MonoTouchAOTHelper.FalseFlag)
		{
			GenericComparer<DateTimeOffset> genericComparer = new GenericComparer<DateTimeOffset>();
			GenericEqualityComparer<DateTimeOffset> genericEqualityComparer = new GenericEqualityComparer<DateTimeOffset>();
		}
	}

	int IComparable.CompareTo(object obj)
	{
		return CompareTo((DateTimeOffset)obj);
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		DateTime value = new DateTime(dt.Ticks).Subtract(utc_offset);
		info.AddValue("DateTime", value);
		info.AddValue("OffsetMinutes", (short)utc_offset.TotalMinutes);
	}

	[MonoTODO]
	void IDeserializationCallback.OnDeserialization(object sender)
	{
	}

	public DateTimeOffset AddTicks(long ticks)
	{
		return new DateTimeOffset(dt.AddTicks(ticks), utc_offset);
	}

	public int CompareTo(DateTimeOffset other)
	{
		return UtcDateTime.CompareTo(other.UtcDateTime);
	}

	public bool Equals(DateTimeOffset other)
	{
		return UtcDateTime == other.UtcDateTime;
	}

	public override bool Equals(object obj)
	{
		if (obj is DateTimeOffset)
		{
			return UtcDateTime == ((DateTimeOffset)obj).UtcDateTime;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return dt.GetHashCode() ^ utc_offset.GetHashCode();
	}

	public override string ToString()
	{
		return ToString(null, null);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(formatProvider);
		if (format == null || format == string.Empty)
		{
			format = instance.ShortDatePattern + " " + instance.LongTimePattern + " zzz";
		}
		bool useutc = false;
		bool use_invariant = false;
		if (format.Length == 1)
		{
			char format2 = format[0];
			try
			{
				format = DateTimeUtils.GetStandardPattern(format2, instance, out useutc, out use_invariant, date_time_offset: true);
			}
			catch
			{
				format = null;
			}
			if (format == null)
			{
				throw new FormatException("format is not one of the format specifier characters defined for DateTimeFormatInfo");
			}
		}
		return (!useutc) ? DateTimeUtils.ToString(DateTime, Offset, format, instance) : DateTimeUtils.ToString(UtcDateTime, TimeSpan.Zero, format, instance);
	}

	public static implicit operator DateTimeOffset(DateTime dateTime)
	{
		return new DateTimeOffset(dateTime);
	}

	public static bool operator <(DateTimeOffset left, DateTimeOffset right)
	{
		return left.UtcDateTime < right.UtcDateTime;
	}
}
