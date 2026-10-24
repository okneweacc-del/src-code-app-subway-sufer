using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace System.Net;

[Serializable]
public class WebProxy : ISerializable, IWebProxy
{
	private Uri address;

	private bool bypassOnLocal;

	private ArrayList bypassList;

	private ICredentials credentials;

	private bool useDefaultCredentials;

	[System.MonoTODO("Does not affect Credentials, since CredentialCache.DefaultCredentials is not implemented.")]
	public bool UseDefaultCredentials => useDefaultCredentials;

	public WebProxy()
		: this(null, bypassOnLocal: false, null, null)
	{
	}

	public WebProxy(Uri address, bool bypassOnLocal, string[] bypassList, ICredentials credentials)
	{
		this.address = address;
		this.bypassOnLocal = bypassOnLocal;
		if (bypassList != null)
		{
			this.bypassList = new ArrayList(bypassList);
		}
		this.credentials = credentials;
		CheckBypassList();
	}

	protected WebProxy(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		address = (Uri)serializationInfo.GetValue("_ProxyAddress", typeof(Uri));
		bypassOnLocal = serializationInfo.GetBoolean("_BypassOnLocal");
		bypassList = (ArrayList)serializationInfo.GetValue("_BypassList", typeof(ArrayList));
		useDefaultCredentials = serializationInfo.GetBoolean("_UseDefaultCredentials");
		credentials = null;
		CheckBypassList();
	}

	void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		GetObjectData(serializationInfo, streamingContext);
	}

	public Uri GetProxy(Uri destination)
	{
		if (IsBypassed(destination))
		{
			return destination;
		}
		return address;
	}

	public bool IsBypassed(Uri host)
	{
		if (host == null)
		{
			throw new ArgumentNullException("host");
		}
		if (host.IsLoopback && bypassOnLocal)
		{
			return true;
		}
		if (address == null)
		{
			return true;
		}
		string host2 = host.Host;
		if (bypassOnLocal && host2.IndexOf('.') == -1)
		{
			return true;
		}
		if (!bypassOnLocal)
		{
			if (string.Compare(host2, "localhost", ignoreCase: true, CultureInfo.InvariantCulture) == 0)
			{
				return true;
			}
			if (string.Compare(host2, "loopback", ignoreCase: true, CultureInfo.InvariantCulture) == 0)
			{
				return true;
			}
			IPAddress addr = null;
			if (IPAddress.TryParse(host2, out addr) && IPAddress.IsLoopback(addr))
			{
				return true;
			}
		}
		if (bypassList == null || bypassList.Count == 0)
		{
			return false;
		}
		try
		{
			string input = host.Scheme + "://" + host.Authority;
			int i;
			for (i = 0; i < bypassList.Count; i++)
			{
				Regex regex = new Regex((string)bypassList[i], RegexOptions.IgnoreCase | RegexOptions.Singleline);
				if (regex.IsMatch(input))
				{
					break;
				}
			}
			if (i == bypassList.Count)
			{
				return false;
			}
			for (; i < bypassList.Count; i++)
			{
				new Regex((string)bypassList[i]);
			}
			return true;
		}
		catch (ArgumentException)
		{
			return false;
		}
	}

	protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		serializationInfo.AddValue("_BypassOnLocal", bypassOnLocal);
		serializationInfo.AddValue("_ProxyAddress", address);
		serializationInfo.AddValue("_BypassList", bypassList);
		serializationInfo.AddValue("_UseDefaultCredentials", UseDefaultCredentials);
	}

	private void CheckBypassList()
	{
		if (bypassList != null)
		{
			for (int i = 0; i < bypassList.Count; i++)
			{
				new Regex((string)bypassList[i]);
			}
		}
	}
}
