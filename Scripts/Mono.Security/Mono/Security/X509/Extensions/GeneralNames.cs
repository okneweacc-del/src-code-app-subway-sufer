using System;
using System.Collections;
using System.Text;

namespace Mono.Security.X509.Extensions;

internal class GeneralNames
{
	private ArrayList rfc822Name;

	private ArrayList dnsName;

	private ArrayList directoryNames;

	private ArrayList uris;

	private ArrayList ipAddr;

	public string[] DNSNames
	{
		get
		{
			if (dnsName == null)
			{
				return new string[0];
			}
			return (string[])dnsName.ToArray(typeof(string));
		}
	}

	public string[] IPAddresses
	{
		get
		{
			if (ipAddr == null)
			{
				return new string[0];
			}
			return (string[])ipAddr.ToArray(typeof(string));
		}
	}

	public GeneralNames(ASN1 sequence)
	{
		for (int i = 0; i < sequence.Count; i++)
		{
			switch (sequence[i].Tag)
			{
			case 129:
				if (rfc822Name == null)
				{
					rfc822Name = new ArrayList();
				}
				rfc822Name.Add(Encoding.ASCII.GetString(sequence[i].Value));
				break;
			case 130:
				if (dnsName == null)
				{
					dnsName = new ArrayList();
				}
				dnsName.Add(Encoding.ASCII.GetString(sequence[i].Value));
				break;
			case 132:
			case 164:
				if (directoryNames == null)
				{
					directoryNames = new ArrayList();
				}
				directoryNames.Add(X501.ToString(sequence[i][0]));
				break;
			case 134:
				if (uris == null)
				{
					uris = new ArrayList();
				}
				uris.Add(Encoding.ASCII.GetString(sequence[i].Value));
				break;
			case 135:
			{
				if (ipAddr == null)
				{
					ipAddr = new ArrayList();
				}
				byte[] value = sequence[i].Value;
				string value2 = ((value.Length != 4) ? ":" : ".");
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < value.Length; j++)
				{
					stringBuilder.Append(value[j].ToString());
					if (j < value.Length - 1)
					{
						stringBuilder.Append(value2);
					}
				}
				ipAddr.Add(stringBuilder.ToString());
				if (ipAddr == null)
				{
					ipAddr = new ArrayList();
				}
				break;
			}
			}
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (rfc822Name != null)
		{
			foreach (string item in rfc822Name)
			{
				stringBuilder.Append("RFC822 Name=");
				stringBuilder.Append(item);
				stringBuilder.Append(Environment.NewLine);
			}
		}
		if (dnsName != null)
		{
			foreach (string item2 in dnsName)
			{
				stringBuilder.Append("DNS Name=");
				stringBuilder.Append(item2);
				stringBuilder.Append(Environment.NewLine);
			}
		}
		if (directoryNames != null)
		{
			foreach (string directoryName in directoryNames)
			{
				stringBuilder.Append("Directory Address: ");
				stringBuilder.Append(directoryName);
				stringBuilder.Append(Environment.NewLine);
			}
		}
		if (uris != null)
		{
			foreach (string uri in uris)
			{
				stringBuilder.Append("URL=");
				stringBuilder.Append(uri);
				stringBuilder.Append(Environment.NewLine);
			}
		}
		if (ipAddr != null)
		{
			foreach (string item3 in ipAddr)
			{
				stringBuilder.Append("IP Address=");
				stringBuilder.Append(item3);
				stringBuilder.Append(Environment.NewLine);
			}
		}
		return stringBuilder.ToString();
	}
}
