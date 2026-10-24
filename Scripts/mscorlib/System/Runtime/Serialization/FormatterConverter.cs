using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[ComVisible(true)]
public class FormatterConverter : IFormatterConverter
{
	public object Convert(object value, Type type)
	{
		return System.Convert.ChangeType(value, type);
	}

	public bool ToBoolean(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToBoolean(value);
	}

	public short ToInt16(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToInt16(value);
	}

	public int ToInt32(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToInt32(value);
	}

	public long ToInt64(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToInt64(value);
	}

	public string ToString(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToString(value);
	}

	[CLSCompliant(false)]
	public uint ToUInt32(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToUInt32(value);
	}
}
