using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Net;

[Serializable]
[ComVisible(true)]
[DefaultMember("Item")]
public class WebHeaderCollection : NameValueCollection, ISerializable
{
	private static readonly Hashtable restricted;

	private static readonly Hashtable multiValue;

	private static readonly Dictionary<string, bool> restricted_response;

	private bool internallyCreated;

	private static bool[] allowed_chars;

	public override int Count => base.Count;

	public override KeysCollection Keys => base.Keys;

	public WebHeaderCollection()
	{
	}

	protected WebHeaderCollection(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		try
		{
			int @int = serializationInfo.GetInt32("Count");
			for (int i = 0; i < @int; i++)
			{
				Add(serializationInfo.GetString(i.ToString()), serializationInfo.GetString((@int + i).ToString()));
			}
		}
		catch (SerializationException)
		{
			int @int = serializationInfo.GetInt32("count");
			for (int j = 0; j < @int; j++)
			{
				Add(serializationInfo.GetString("k" + j), serializationInfo.GetString("v" + j));
			}
		}
	}

	internal WebHeaderCollection(bool internallyCreated)
	{
		this.internallyCreated = internallyCreated;
	}

	static WebHeaderCollection()
	{
		allowed_chars = new bool[126]
		{
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, true, false, true, true, true, true, false,
			false, false, true, true, false, true, true, false, true, true,
			true, true, true, true, true, true, true, true, false, false,
			false, false, false, false, false, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, false, false, false, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, false, true, false
		};
		restricted = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
		restricted.Add("accept", true);
		restricted.Add("connection", true);
		restricted.Add("content-length", true);
		restricted.Add("content-type", true);
		restricted.Add("date", true);
		restricted.Add("expect", true);
		restricted.Add("host", true);
		restricted.Add("if-modified-since", true);
		restricted.Add("range", true);
		restricted.Add("referer", true);
		restricted.Add("transfer-encoding", true);
		restricted.Add("user-agent", true);
		restricted.Add("proxy-connection", true);
		restricted_response = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
		restricted_response.Add("Content-Length", value: true);
		restricted_response.Add("Transfer-Encoding", value: true);
		restricted_response.Add("WWW-Authenticate", value: true);
		multiValue = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
		multiValue.Add("accept", true);
		multiValue.Add("accept-charset", true);
		multiValue.Add("accept-encoding", true);
		multiValue.Add("accept-language", true);
		multiValue.Add("accept-ranges", true);
		multiValue.Add("allow", true);
		multiValue.Add("authorization", true);
		multiValue.Add("cache-control", true);
		multiValue.Add("connection", true);
		multiValue.Add("content-encoding", true);
		multiValue.Add("content-language", true);
		multiValue.Add("expect", true);
		multiValue.Add("if-match", true);
		multiValue.Add("if-none-match", true);
		multiValue.Add("proxy-authenticate", true);
		multiValue.Add("public", true);
		multiValue.Add("range", true);
		multiValue.Add("transfer-encoding", true);
		multiValue.Add("upgrade", true);
		multiValue.Add("vary", true);
		multiValue.Add("via", true);
		multiValue.Add("warning", true);
		multiValue.Add("www-authenticate", true);
		multiValue.Add("set-cookie", true);
		multiValue.Add("set-cookie2", true);
	}

	void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		GetObjectData(serializationInfo, streamingContext);
	}

	public override void Add(string name, string value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (internallyCreated && IsRestricted(name))
		{
			throw new ArgumentException("This header must be modified with the appropiate property.");
		}
		AddWithoutValidate(name, value);
	}

	protected void AddWithoutValidate(string headerName, string headerValue)
	{
		if (!IsHeaderName(headerName))
		{
			throw new ArgumentException("invalid header name: " + headerName, "headerName");
		}
		headerValue = ((headerValue != null) ? headerValue.Trim() : string.Empty);
		if (!IsHeaderValue(headerValue))
		{
			throw new ArgumentException("invalid header value: " + headerValue, "headerValue");
		}
		base.Add(headerName, headerValue);
	}

	public static bool IsRestricted(string headerName)
	{
		if (headerName == null)
		{
			throw new ArgumentNullException("headerName");
		}
		if (headerName == string.Empty)
		{
			throw new ArgumentException("empty string", "headerName");
		}
		if (!IsHeaderName(headerName))
		{
			throw new ArgumentException("Invalid character in header");
		}
		return restricted.ContainsKey(headerName);
	}

	public override void OnDeserialization(object sender)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(GetKey(i)).Append(": ").Append(Get(i))
				.Append("\r\n");
		}
		return stringBuilder.Append("\r\n").ToString();
	}

	public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		int count = base.Count;
		serializationInfo.AddValue("Count", count);
		for (int i = 0; i < count; i++)
		{
			serializationInfo.AddValue(i.ToString(), GetKey(i));
			serializationInfo.AddValue((count + i).ToString(), Get(i));
		}
	}

	public override string Get(int index)
	{
		return base.Get(index);
	}

	public override string GetKey(int index)
	{
		return base.GetKey(index);
	}

	public override IEnumerator GetEnumerator()
	{
		return base.GetEnumerator();
	}

	internal static bool IsHeaderValue(string value)
	{
		int length = value.Length;
		for (int i = 0; i < length; i++)
		{
			char c = value[i];
			if (c == '\u007f')
			{
				return false;
			}
			if (c < ' ' && c != '\r' && c != '\n' && c != '\t')
			{
				return false;
			}
			if (c == '\n' && ++i < length)
			{
				c = value[i];
				if (c != ' ' && c != '\t')
				{
					return false;
				}
			}
		}
		return true;
	}

	internal static bool IsHeaderName(string name)
	{
		if (name == null || name.Length == 0)
		{
			return false;
		}
		int length = name.Length;
		for (int i = 0; i < length; i++)
		{
			char c = name[i];
			if (c > '~' || !allowed_chars[(uint)c])
			{
				return false;
			}
		}
		return true;
	}
}
