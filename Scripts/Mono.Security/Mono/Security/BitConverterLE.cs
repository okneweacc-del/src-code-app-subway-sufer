using System;

namespace Mono.Security;

internal sealed class BitConverterLE
{
	private unsafe static byte[] GetUIntBytes(byte* bytes)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return new byte[4]
			{
				bytes[3],
				bytes[2],
				bytes[1],
				*bytes
			};
		}
		return new byte[4]
		{
			*bytes,
			bytes[1],
			bytes[2],
			bytes[3]
		};
	}

	internal unsafe static byte[] GetBytes(int value)
	{
		return GetUIntBytes((byte*)(&value));
	}
}
