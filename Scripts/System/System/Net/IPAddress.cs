using System.Globalization;
using System.Net.Sockets;

namespace System.Net;

[Serializable]
public class IPAddress
{
	private long m_Address;

	private AddressFamily m_Family;

	private ushort[] m_Numbers;

	private long m_ScopeId;

	public static readonly IPAddress Any = new IPAddress(0L);

	public static readonly IPAddress Broadcast = Parse("255.255.255.255");

	public static readonly IPAddress Loopback = Parse("127.0.0.1");

	public static readonly IPAddress None = Parse("255.255.255.255");

	public static readonly IPAddress IPv6Any = ParseIPV6("::");

	public static readonly IPAddress IPv6Loopback = ParseIPV6("::1");

	public static readonly IPAddress IPv6None = ParseIPV6("::");

	private int m_HashCode;

	internal long InternalIPv4Address => m_Address;

	public long ScopeId
	{
		get
		{
			if (m_Family != AddressFamily.InterNetworkV6)
			{
				throw new Exception("The attempted operation is not supported for the type of object referenced");
			}
			return m_ScopeId;
		}
	}

	public AddressFamily AddressFamily => m_Family;

	public IPAddress(long addr)
	{
		m_Address = addr;
		m_Family = AddressFamily.InterNetwork;
	}

	internal IPAddress(ushort[] address, long scopeId)
	{
		m_Numbers = address;
		for (int i = 0; i < 8; i++)
		{
			m_Numbers[i] = (ushort)HostToNetworkOrder((short)m_Numbers[i]);
		}
		m_Family = AddressFamily.InterNetworkV6;
		m_ScopeId = scopeId;
	}

	private static short SwapShort(short number)
	{
		return (short)(((number >> 8) & 0xFF) | ((number << 8) & 0xFF00));
	}

	public static short HostToNetworkOrder(short host)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return host;
		}
		return SwapShort(host);
	}

	public static short NetworkToHostOrder(short network)
	{
		if (!BitConverter.IsLittleEndian)
		{
			return network;
		}
		return SwapShort(network);
	}

	public static IPAddress Parse(string ipString)
	{
		if (TryParse(ipString, out var address))
		{
			return address;
		}
		throw new FormatException("An invalid IP address was specified.");
	}

	public static bool TryParse(string ipString, out IPAddress address)
	{
		if (ipString == null)
		{
			throw new ArgumentNullException("ipString");
		}
		if ((address = ParseIPV4(ipString)) == null && (address = ParseIPV6(ipString)) == null)
		{
			return false;
		}
		return true;
	}

	private static IPAddress ParseIPV4(string ip)
	{
		int num = ip.IndexOf(' ');
		if (num != -1)
		{
			string[] array = ip.Substring(num + 1).Split('.');
			if (array.Length > 0)
			{
				string text = array[array.Length - 1];
				if (text.Length == 0)
				{
					return null;
				}
				char[] array2 = text.ToCharArray();
				foreach (char digit in array2)
				{
					if (!Uri.IsHexDigit(digit))
					{
						return null;
					}
				}
			}
			ip = ip.Substring(0, num);
		}
		if (ip.Length == 0 || ip[ip.Length - 1] == '.')
		{
			return null;
		}
		string[] array3 = ip.Split('.');
		if (array3.Length > 4)
		{
			return null;
		}
		try
		{
			long num2 = 0L;
			long result = 0L;
			for (int j = 0; j < array3.Length; j++)
			{
				string text2 = array3[j];
				if (3 <= text2.Length && text2.Length <= 4 && text2[0] == '0' && (text2[1] == 'x' || text2[1] == 'X'))
				{
					result = ((text2.Length != 3) ? ((int)(byte)((Uri.FromHex(text2[2]) << 4) | Uri.FromHex(text2[3]))) : ((int)(byte)Uri.FromHex(text2[2])));
				}
				else
				{
					if (text2.Length == 0)
					{
						return null;
					}
					if (text2[0] == '0')
					{
						result = 0L;
						for (int k = 1; k < text2.Length; k++)
						{
							if ('0' <= text2[k] && text2[k] <= '7')
							{
								result = (result << 3) + (int)text2[k] - 48;
								continue;
							}
							return null;
						}
					}
					else if (!long.TryParse(text2, NumberStyles.None, null, out result))
					{
						return null;
					}
				}
				if (j == array3.Length - 1)
				{
					j = 3;
				}
				else if (result > 255)
				{
					return null;
				}
				int num3 = 0;
				while (result > 0)
				{
					num2 |= (result & 0xFF) << (j - num3 << 3);
					num3++;
					result /= 256;
				}
			}
			return new IPAddress(num2);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static IPAddress ParseIPV6(string ip)
	{
		if (System.Net.IPv6Address.TryParse(ip, out var result))
		{
			return new IPAddress(result.Address, result.ScopeId);
		}
		return null;
	}

	public byte[] GetAddressBytes()
	{
		if (m_Family == AddressFamily.InterNetworkV6)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(m_Numbers, 0, array, 0, 16);
			return array;
		}
		return new byte[4]
		{
			(byte)(m_Address & 0xFF),
			(byte)((m_Address >> 8) & 0xFF),
			(byte)((m_Address >> 16) & 0xFF),
			(byte)(m_Address >> 24)
		};
	}

	public static bool IsLoopback(IPAddress addr)
	{
		if (addr.m_Family == AddressFamily.InterNetwork)
		{
			return (addr.m_Address & 0xFF) == 127;
		}
		for (int i = 0; i < 6; i++)
		{
			if (addr.m_Numbers[i] != 0)
			{
				return false;
			}
		}
		return NetworkToHostOrder((short)addr.m_Numbers[7]) == 1;
	}

	public override string ToString()
	{
		if (m_Family == AddressFamily.InterNetwork)
		{
			return ToString(m_Address);
		}
		ushort[] array = m_Numbers.Clone() as ushort[];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (ushort)NetworkToHostOrder((short)array[i]);
		}
		System.Net.IPv6Address pv6Address = new System.Net.IPv6Address(array);
		pv6Address.ScopeId = ScopeId;
		return pv6Address.ToString();
	}

	private static string ToString(long addr)
	{
		return (addr & 0xFF) + "." + ((addr >> 8) & 0xFF) + "." + ((addr >> 16) & 0xFF) + "." + ((addr >> 24) & 0xFF);
	}

	public override bool Equals(object other)
	{
		if (other is IPAddress iPAddress)
		{
			if (AddressFamily != iPAddress.AddressFamily)
			{
				return false;
			}
			if (AddressFamily == AddressFamily.InterNetwork)
			{
				return m_Address == iPAddress.m_Address;
			}
			ushort[] numbers = iPAddress.m_Numbers;
			for (int i = 0; i < 8; i++)
			{
				if (m_Numbers[i] != numbers[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (m_Family == AddressFamily.InterNetwork)
		{
			return (int)m_Address;
		}
		return Hash((m_Numbers[0] << 16) + m_Numbers[1], (m_Numbers[2] << 16) + m_Numbers[3], (m_Numbers[4] << 16) + m_Numbers[5], (m_Numbers[6] << 16) + m_Numbers[7]);
	}

	private static int Hash(int i, int j, int k, int l)
	{
		return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25));
	}
}
