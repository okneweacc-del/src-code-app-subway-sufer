using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization;

[Serializable]
[ComVisible(true)]
public sealed class NumberFormatInfo : ICloneable, IFormatProvider
{
	private bool isReadOnly;

	private string decimalFormats;

	private string currencyFormats;

	private string percentFormats;

	private string digitPattern = "#";

	private string zeroPattern = "0";

	private int currencyDecimalDigits;

	private string currencyDecimalSeparator;

	private string currencyGroupSeparator;

	private int[] currencyGroupSizes;

	private int currencyNegativePattern;

	private int currencyPositivePattern;

	private string currencySymbol;

	private string nanSymbol;

	private string negativeInfinitySymbol;

	private string negativeSign;

	private int numberDecimalDigits;

	private string numberDecimalSeparator;

	private string numberGroupSeparator;

	private int[] numberGroupSizes;

	private int numberNegativePattern;

	private int percentDecimalDigits;

	private string percentDecimalSeparator;

	private string percentGroupSeparator;

	private int[] percentGroupSizes;

	private int percentNegativePattern;

	private int percentPositivePattern;

	private string percentSymbol;

	private string perMilleSymbol;

	private string positiveInfinitySymbol;

	private string positiveSign;

	private string ansiCurrencySymbol;

	private int m_dataItem;

	private bool m_useUserOverride;

	private bool validForParseAsNumber;

	private bool validForParseAsCurrency;

	private string[] nativeDigits = invariantNativeDigits;

	private int digitSubstitution = 1;

	private static readonly string[] invariantNativeDigits = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

	public int CurrencyDecimalDigits => currencyDecimalDigits;

	public string CurrencyDecimalSeparator => currencyDecimalSeparator;

	public string CurrencyGroupSeparator => currencyGroupSeparator;

	internal int[] RawCurrencyGroupSizes => currencyGroupSizes;

	public int CurrencyNegativePattern => currencyNegativePattern;

	public int CurrencyPositivePattern => currencyPositivePattern;

	public string CurrencySymbol => currencySymbol;

	public static NumberFormatInfo CurrentInfo
	{
		get
		{
			NumberFormatInfo numberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;
			numberFormat.isReadOnly = true;
			return numberFormat;
		}
	}

	public static NumberFormatInfo InvariantInfo
	{
		get
		{
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			numberFormatInfo.NumberNegativePattern = 1;
			numberFormatInfo.isReadOnly = true;
			return numberFormatInfo;
		}
	}

	public string NaNSymbol => nanSymbol;

	public string NegativeInfinitySymbol => negativeInfinitySymbol;

	public string NegativeSign => negativeSign;

	public int NumberDecimalDigits => numberDecimalDigits;

	public string NumberDecimalSeparator => numberDecimalSeparator;

	public string NumberGroupSeparator => numberGroupSeparator;

	internal int[] RawNumberGroupSizes => numberGroupSizes;

	public int NumberNegativePattern
	{
		get
		{
			return numberNegativePattern;
		}
		set
		{
			if (value < 0 || value > 4)
			{
				throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 15");
			}
			if (isReadOnly)
			{
				throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
			}
			numberNegativePattern = value;
		}
	}

	public int PercentDecimalDigits => percentDecimalDigits;

	public string PercentDecimalSeparator => percentDecimalSeparator;

	public string PercentGroupSeparator => percentGroupSeparator;

	internal int[] RawPercentGroupSizes => percentGroupSizes;

	public int PercentNegativePattern => percentNegativePattern;

	public int PercentPositivePattern => percentPositivePattern;

	public string PercentSymbol => percentSymbol;

	public string PerMilleSymbol => perMilleSymbol;

	public string PositiveInfinitySymbol => positiveInfinitySymbol;

	public string PositiveSign => positiveSign;

	internal NumberFormatInfo(int lcid, bool read_only)
	{
		isReadOnly = read_only;
		if (lcid != 127)
		{
			lcid = 127;
		}
		int num = lcid;
		if (num == 127)
		{
			isReadOnly = false;
			currencyDecimalDigits = 2;
			currencyDecimalSeparator = ".";
			currencyGroupSeparator = ",";
			currencyGroupSizes = new int[1] { 3 };
			currencyNegativePattern = 0;
			currencyPositivePattern = 0;
			currencySymbol = "$";
			nanSymbol = "NaN";
			negativeInfinitySymbol = "-Infinity";
			negativeSign = "-";
			numberDecimalDigits = 2;
			numberDecimalSeparator = ".";
			numberGroupSeparator = ",";
			numberGroupSizes = new int[1] { 3 };
			numberNegativePattern = 1;
			percentDecimalDigits = 2;
			percentDecimalSeparator = ".";
			percentGroupSeparator = ",";
			percentGroupSizes = new int[1] { 3 };
			percentNegativePattern = 0;
			percentPositivePattern = 0;
			percentSymbol = "%";
			perMilleSymbol = "‰";
			positiveInfinitySymbol = "Infinity";
			positiveSign = "+";
		}
	}

	internal NumberFormatInfo(bool read_only)
		: this(127, read_only)
	{
	}

	public NumberFormatInfo()
		: this(read_only: false)
	{
	}

	public object GetFormat(Type formatType)
	{
		return (formatType != typeof(NumberFormatInfo)) ? null : this;
	}

	public object Clone()
	{
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)MemberwiseClone();
		numberFormatInfo.isReadOnly = false;
		return numberFormatInfo;
	}

	public static NumberFormatInfo ReadOnly(NumberFormatInfo nfi)
	{
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)nfi.Clone();
		numberFormatInfo.isReadOnly = true;
		return numberFormatInfo;
	}

	public static NumberFormatInfo GetInstance(IFormatProvider formatProvider)
	{
		if (formatProvider != null)
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo));
			if (numberFormatInfo != null)
			{
				return numberFormatInfo;
			}
		}
		return CurrentInfo;
	}
}
