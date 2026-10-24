namespace System.Net;

[Obsolete("Use WebRequest.DefaultProxy instead")]
public class GlobalProxySelection
{
	public static IWebProxy Select => WebRequest.DefaultWebProxy;
}
