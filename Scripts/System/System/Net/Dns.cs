using System.Collections;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace System.Net;

public static class Dns
{
	static Dns()
	{
		Socket.CheckProtocolSupport();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetHostByName_internal(string host, out string h_name, out string[] h_aliases, out string[] h_addr_list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetHostByAddr_internal(string addr, out string h_name, out string[] h_aliases, out string[] h_addr_list);

	private static IPHostEntry hostent_to_IPHostEntry(string h_name, string[] h_aliases, string[] h_addrlist)
	{
		IPHostEntry iPHostEntry = new IPHostEntry();
		ArrayList arrayList = new ArrayList();
		iPHostEntry.HostName = h_name;
		iPHostEntry.Aliases = h_aliases;
		for (int i = 0; i < h_addrlist.Length; i++)
		{
			try
			{
				IPAddress iPAddress = IPAddress.Parse(h_addrlist[i]);
				if ((Socket.SupportsIPv6 && iPAddress.AddressFamily == AddressFamily.InterNetworkV6) || (Socket.SupportsIPv4 && iPAddress.AddressFamily == AddressFamily.InterNetwork))
				{
					arrayList.Add(iPAddress);
				}
			}
			catch (ArgumentNullException)
			{
			}
		}
		if (arrayList.Count == 0)
		{
			throw new SocketException(11001);
		}
		iPHostEntry.AddressList = arrayList.ToArray(typeof(IPAddress)) as IPAddress[];
		return iPHostEntry;
	}

	private static IPHostEntry GetHostByAddressFromString(string address, bool parse)
	{
		if (address.Equals("0.0.0.0"))
		{
			address = "127.0.0.1";
			parse = false;
		}
		if (parse)
		{
			IPAddress.Parse(address);
		}
		if (!GetHostByAddr_internal(address, out var h_name, out var h_aliases, out var h_addr_list))
		{
			throw new SocketException(11001);
		}
		return hostent_to_IPHostEntry(h_name, h_aliases, h_addr_list);
	}

	public static IPHostEntry GetHostEntry(string hostNameOrAddress)
	{
		if (hostNameOrAddress == null)
		{
			throw new ArgumentNullException("hostNameOrAddress");
		}
		if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
		{
			throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
		}
		if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out var address))
		{
			return GetHostEntry(address);
		}
		return GetHostByName(hostNameOrAddress);
	}

	public static IPHostEntry GetHostEntry(IPAddress address)
	{
		if (address == null)
		{
			throw new ArgumentNullException("address");
		}
		return GetHostByAddressFromString(address.ToString(), parse: false);
	}

	public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
	{
		if (hostNameOrAddress == null)
		{
			throw new ArgumentNullException("hostNameOrAddress");
		}
		if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
		{
			throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
		}
		if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out var address))
		{
			return new IPAddress[1] { address };
		}
		return GetHostEntry(hostNameOrAddress).AddressList;
	}

	[Obsolete("Use GetHostEntry instead")]
	public static IPHostEntry GetHostByName(string hostName)
	{
		if (hostName == null)
		{
			throw new ArgumentNullException("hostName");
		}
		if (!GetHostByName_internal(hostName, out var h_name, out var h_aliases, out var h_addr_list))
		{
			throw new SocketException(11001);
		}
		return hostent_to_IPHostEntry(h_name, h_aliases, h_addr_list);
	}
}
