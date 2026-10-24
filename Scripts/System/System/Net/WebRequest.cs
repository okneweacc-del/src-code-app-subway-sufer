using System.Collections.Specialized;
using System.Net.Security;
using System.Runtime.Serialization;

namespace System.Net;

[Serializable]
public abstract class WebRequest : MarshalByRefObject, ISerializable
{
	private static HybridDictionary prefixes;

	private static bool isDefaultWebProxySet;

	private static IWebProxy defaultWebProxy;

	private AuthenticationLevel authentication_level = AuthenticationLevel.MutualAuthRequested;

	private static readonly object lockobj;

	public static IWebProxy DefaultWebProxy
	{
		get
		{
			if (!isDefaultWebProxySet)
			{
				lock (lockobj)
				{
					if (defaultWebProxy == null)
					{
						defaultWebProxy = GetDefaultWebProxy();
					}
				}
			}
			return defaultWebProxy;
		}
	}

	protected WebRequest()
	{
	}

	protected WebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
	}

	static WebRequest()
	{
		prefixes = new HybridDictionary();
		lockobj = new object();
		AddDynamicPrefix("http", "HttpRequestCreator");
		AddDynamicPrefix("https", "HttpRequestCreator");
		AddDynamicPrefix("file", "FileWebRequestCreator");
		AddDynamicPrefix("ftp", "FtpRequestCreator");
	}

	void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		throw new NotSupportedException();
	}

	private static void AddDynamicPrefix(string protocol, string implementor)
	{
		Type type = typeof(WebRequest).Assembly.GetType("System.Net." + implementor);
		if (type != null)
		{
			AddPrefix(protocol, type);
		}
	}

	private static Exception GetMustImplement()
	{
		return new NotImplementedException("This method must be implemented in derived classes");
	}

	[System.MonoTODO("Needs to respect Module, Proxy.AutoDetect, and Proxy.ScriptLocation config settings")]
	private static IWebProxy GetDefaultWebProxy()
	{
		return null;
	}

	protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		throw GetMustImplement();
	}

	internal static void AddPrefix(string prefix, Type type)
	{
		object value = Activator.CreateInstance(type, nonPublic: true);
		prefixes[prefix] = value;
	}
}
