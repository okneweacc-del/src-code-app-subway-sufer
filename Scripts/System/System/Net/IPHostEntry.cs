namespace System.Net;

public class IPHostEntry
{
	private IPAddress[] addressList;

	private string[] aliases;

	private string hostName;

	public IPAddress[] AddressList
	{
		get
		{
			return addressList;
		}
		set
		{
			addressList = value;
		}
	}

	public string[] Aliases
	{
		set
		{
			aliases = value;
		}
	}

	public string HostName
	{
		set
		{
			hostName = value;
		}
	}
}
