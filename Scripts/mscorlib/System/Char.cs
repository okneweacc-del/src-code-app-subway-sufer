using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

[Serializable]
[ComVisible(true)]
public struct Char : IConvertible, IComparable, IComparable<char>, IEquatable<char>
{
	public const char MaxValue = '\uffff';

	public const char MinValue = '\0';

	internal char m_value;

	private unsafe static readonly byte* category_data;

	private unsafe static readonly byte* numeric_data;

	private unsafe static readonly double* numeric_data_values;

	private unsafe static readonly ushort* to_lower_data_low;

	private unsafe static readonly ushort* to_lower_data_high;

	private unsafe static readonly ushort* to_upper_data_low;

	private unsafe static readonly ushort* to_upper_data_high;

	unsafe static Char()
	{
		GetDataTablePointers(out category_data, out numeric_data, out numeric_data_values, out to_lower_data_low, out to_lower_data_high, out to_upper_data_low, out to_upper_data_high);
	}

	object IConvertible.ToType(Type targetType, IFormatProvider provider)
	{
		if (targetType == null)
		{
			throw new ArgumentNullException("targetType");
		}
		return Convert.ToType(this, targetType, provider, try_target_to_type: false);
	}

	bool IConvertible.ToBoolean(IFormatProvider provider)
	{
		throw new InvalidCastException();
	}

	byte IConvertible.ToByte(IFormatProvider provider)
	{
		return Convert.ToByte(this);
	}

	char IConvertible.ToChar(IFormatProvider provider)
	{
		return this;
	}

	DateTime IConvertible.ToDateTime(IFormatProvider provider)
	{
		throw new InvalidCastException();
	}

	decimal IConvertible.ToDecimal(IFormatProvider provider)
	{
		throw new InvalidCastException();
	}

	double IConvertible.ToDouble(IFormatProvider provider)
	{
		throw new InvalidCastException();
	}

	short IConvertible.ToInt16(IFormatProvider provider)
	{
		return Convert.ToInt16(this);
	}

	int IConvertible.ToInt32(IFormatProvider provider)
	{
		return Convert.ToInt32(this);
	}

	long IConvertible.ToInt64(IFormatProvider provider)
	{
		return Convert.ToInt64(this);
	}

	sbyte IConvertible.ToSByte(IFormatProvider provider)
	{
		return Convert.ToSByte(this);
	}

	float IConvertible.ToSingle(IFormatProvider provider)
	{
		throw new InvalidCastException();
	}

	ushort IConvertible.ToUInt16(IFormatProvider provider)
	{
		return Convert.ToUInt16(this);
	}

	uint IConvertible.ToUInt32(IFormatProvider provider)
	{
		return Convert.ToUInt32(this);
	}

	ulong IConvertible.ToUInt64(IFormatProvider provider)
	{
		return Convert.ToUInt64(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void GetDataTablePointers(out byte* category_data, out byte* numeric_data, out double* numeric_data_values, out ushort* to_lower_data_low, out ushort* to_lower_data_high, out ushort* to_upper_data_low, out ushort* to_upper_data_high);

	public int CompareTo(object value)
	{
		if (value == null)
		{
			return 1;
		}
		if (!(value is char c))
		{
			throw new ArgumentException(Locale.GetText("Value is not a System.Char"));
		}
		if (this == c)
		{
			return 0;
		}
		if (this > c)
		{
			return 1;
		}
		return -1;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is char))
		{
			return false;
		}
		return (char)obj == this;
	}

	public int CompareTo(char value)
	{
		if (this == value)
		{
			return 0;
		}
		if (this > value)
		{
			return 1;
		}
		return -1;
	}

	public static string ConvertFromUtf32(int utf32)
	{
		if (utf32 < 0 || utf32 > 1114111)
		{
			throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
		}
		if (55296 <= utf32 && utf32 <= 57343)
		{
			throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
		}
		if (utf32 < 65536)
		{
			return new string((char)utf32, 1);
		}
		utf32 -= 65536;
		return new string(new char[2]
		{
			(char)((utf32 >> 10) + 55296),
			(char)(utf32 % 1024 + 56320)
		});
	}

	public bool Equals(char obj)
	{
		return this == obj;
	}

	public override int GetHashCode()
	{
		return this;
	}

	public unsafe static double GetNumericValue(char c)
	{
		if (c > '㊉')
		{
			if (c >= '０' && c <= '９')
			{
				return c - 65296;
			}
			return -1.0;
		}
		return numeric_data_values[(int)numeric_data[(int)c]];
	}

	public unsafe static UnicodeCategory GetUnicodeCategory(char c)
	{
		return (UnicodeCategory)category_data[(int)c];
	}

	public unsafe static bool IsDigit(char c)
	{
		return category_data[(int)c] == 8;
	}

	public static bool IsDigit(string s, int index)
	{
		CheckParameter(s, index);
		return IsDigit(s[index]);
	}

	public unsafe static bool IsLetter(char c)
	{
		return category_data[(int)c] <= 4;
	}

	public unsafe static bool IsLetterOrDigit(char c)
	{
		int num = category_data[(int)c];
		return num <= 4 || num == 8;
	}

	public static bool IsLetterOrDigit(string s, int index)
	{
		CheckParameter(s, index);
		return IsLetterOrDigit(s[index]);
	}

	public unsafe static bool IsLower(char c)
	{
		return category_data[(int)c] == 1;
	}

	public static bool IsLowSurrogate(char c)
	{
		return c >= '\udc00' && c <= '\udfff';
	}

	public unsafe static bool IsNumber(char c)
	{
		int num = category_data[(int)c];
		return num >= 8 && num <= 10;
	}

	public unsafe static bool IsSurrogate(char c)
	{
		return category_data[(int)c] == 16;
	}

	public unsafe static bool IsUpper(char c)
	{
		return category_data[(int)c] == 0;
	}

	public unsafe static bool IsWhiteSpace(char c)
	{
		int num = category_data[(int)c];
		if (num <= 10)
		{
			return false;
		}
		if (num <= 13)
		{
			return true;
		}
		return (c >= '\t' && c <= '\r') || c == '\u0085' || c == '\u205f';
	}

	public static bool IsWhiteSpace(string s, int index)
	{
		CheckParameter(s, index);
		return IsWhiteSpace(s[index]);
	}

	private static void CheckParameter(string s, int index)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (index < 0 || index >= s.Length)
		{
			throw new ArgumentOutOfRangeException(Locale.GetText("The value of index is less than zero, or greater than or equal to the length of s."));
		}
	}

	public static char Parse(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (s.Length != 1)
		{
			throw new FormatException(Locale.GetText("s contains more than one character."));
		}
		return s[0];
	}

	public static char ToLower(char c)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToLower(c);
	}

	public unsafe static char ToLowerInvariant(char c)
	{
		if (c <= 'Ⓩ')
		{
			return *(char*)((byte*)to_lower_data_low + c * 2);
		}
		if (c >= 'Ａ')
		{
			return *(char*)((byte*)to_lower_data_high + (c - 65313) * 2);
		}
		return c;
	}

	public static char ToLower(char c, CultureInfo culture)
	{
		if (culture == null)
		{
			throw new ArgumentNullException("culture");
		}
		if (culture.LCID == 127)
		{
			return ToLowerInvariant(c);
		}
		return culture.TextInfo.ToLower(c);
	}

	public static char ToUpper(char c)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToUpper(c);
	}

	public unsafe static char ToUpperInvariant(char c)
	{
		if (c <= 'ⓩ')
		{
			return *(char*)((byte*)to_upper_data_low + c * 2);
		}
		if (c >= 'Ａ')
		{
			return *(char*)((byte*)to_upper_data_high + (c - 65313) * 2);
		}
		return c;
	}

	public override string ToString()
	{
		return new string(this, 1);
	}

	public string ToString(IFormatProvider provider)
	{
		return new string(this, 1);
	}

	public TypeCode GetTypeCode()
	{
		return TypeCode.Char;
	}
}
