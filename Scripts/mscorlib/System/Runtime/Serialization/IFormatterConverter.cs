using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[CLSCompliant(false)]
[ComVisible(true)]
public interface IFormatterConverter
{
	object Convert(object value, Type type);

	bool ToBoolean(object value);

	short ToInt16(object value);

	int ToInt32(object value);

	long ToInt64(object value);

	string ToString(object value);

	uint ToUInt32(object value);
}
