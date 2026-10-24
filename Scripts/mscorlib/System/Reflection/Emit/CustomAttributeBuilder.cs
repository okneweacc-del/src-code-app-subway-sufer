using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection.Emit;

[ComVisible(true)]
[ComDefaultInterface(typeof(_CustomAttributeBuilder))]
[ClassInterface(ClassInterfaceType.None)]
public class CustomAttributeBuilder : _CustomAttributeBuilder
{
	private ConstructorInfo ctor;

	private byte[] data;

	internal ConstructorInfo Ctor => ctor;

	internal byte[] Data => data;

	internal CustomAttributeBuilder(ConstructorInfo con, byte[] cdata)
	{
		ctor = con;
		data = (byte[])cdata.Clone();
	}

	internal static int decode_len(byte[] data, int pos, out int rpos)
	{
		int num = 0;
		if ((data[pos] & 0x80) == 0)
		{
			num = data[pos++] & 0x7F;
		}
		else if ((data[pos] & 0x40) == 0)
		{
			num = ((data[pos] & 0x3F) << 8) + data[pos + 1];
			pos += 2;
		}
		else
		{
			num = ((data[pos] & 0x1F) << 24) + (data[pos + 1] << 16) + (data[pos + 2] << 8) + data[pos + 3];
			pos += 4;
		}
		rpos = pos;
		return num;
	}

	internal static string string_from_bytes(byte[] data, int pos, int len)
	{
		return Encoding.UTF8.GetString(data, pos, len);
	}

	internal string string_arg()
	{
		int rpos = 2;
		int len = decode_len(data, rpos, out rpos);
		return string_from_bytes(data, rpos, len);
	}
}
