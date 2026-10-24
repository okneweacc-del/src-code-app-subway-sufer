using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace System.Net;

[Serializable]
public class HttpWebRequest : WebRequest, ISerializable
{
	private Uri requestUri;

	private Uri actualUri;

	private bool hostChanged;

	private bool allowAutoRedirect = true;

	private bool allowBuffering = true;

	private X509CertificateCollection certificates;

	private string connectionGroup;

	private long contentLength = -1L;

	private WebHeaderCollection webHeaders = new WebHeaderCollection(internallyCreated: true);

	private bool keepAlive = true;

	private int maxAutoRedirect = 50;

	private string mediaType = string.Empty;

	private string method = "GET";

	private string initialMethod = "GET";

	private bool pipelined = true;

	private Version version = HttpVersion.Version11;

	private IWebProxy proxy;

	private bool sendChunked;

	private ServicePoint servicePoint;

	private int timeout = 100000;

	private int redirects;

	private object locker = new object();

	private static int defaultMaxResponseHeadersLength;

	private int readWriteTimeout = 300000;

	public Uri Address => actualUri;

	public ServicePoint ServicePoint => GetServicePoint();

	public HttpWebRequest(Uri uri)
	{
		requestUri = uri;
		actualUri = uri;
		proxy = GlobalProxySelection.Select;
	}

	[Obsolete("Serialization is obsoleted for this type", false)]
	protected HttpWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		requestUri = (Uri)serializationInfo.GetValue("requestUri", typeof(Uri));
		actualUri = (Uri)serializationInfo.GetValue("actualUri", typeof(Uri));
		allowAutoRedirect = serializationInfo.GetBoolean("allowAutoRedirect");
		allowBuffering = serializationInfo.GetBoolean("allowBuffering");
		certificates = (X509CertificateCollection)serializationInfo.GetValue("certificates", typeof(X509CertificateCollection));
		connectionGroup = serializationInfo.GetString("connectionGroup");
		contentLength = serializationInfo.GetInt64("contentLength");
		webHeaders = (WebHeaderCollection)serializationInfo.GetValue("webHeaders", typeof(WebHeaderCollection));
		keepAlive = serializationInfo.GetBoolean("keepAlive");
		maxAutoRedirect = serializationInfo.GetInt32("maxAutoRedirect");
		mediaType = serializationInfo.GetString("mediaType");
		method = serializationInfo.GetString("method");
		initialMethod = serializationInfo.GetString("initialMethod");
		pipelined = serializationInfo.GetBoolean("pipelined");
		version = (Version)serializationInfo.GetValue("version", typeof(Version));
		proxy = (IWebProxy)serializationInfo.GetValue("proxy", typeof(IWebProxy));
		sendChunked = serializationInfo.GetBoolean("sendChunked");
		timeout = serializationInfo.GetInt32("timeout");
		redirects = serializationInfo.GetInt32("redirects");
	}

	static HttpWebRequest()
	{
		defaultMaxResponseHeadersLength = 65536;
	}

	void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		GetObjectData(serializationInfo, streamingContext);
	}

	internal ServicePoint GetServicePoint()
	{
		lock (locker)
		{
			if (hostChanged || servicePoint == null)
			{
				servicePoint = ServicePointManager.FindServicePoint(actualUri, proxy);
				hostChanged = false;
			}
		}
		return servicePoint;
	}

	protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		serializationInfo.AddValue("requestUri", requestUri, typeof(Uri));
		serializationInfo.AddValue("actualUri", actualUri, typeof(Uri));
		serializationInfo.AddValue("allowAutoRedirect", allowAutoRedirect);
		serializationInfo.AddValue("allowBuffering", allowBuffering);
		serializationInfo.AddValue("certificates", certificates, typeof(X509CertificateCollection));
		serializationInfo.AddValue("connectionGroup", connectionGroup);
		serializationInfo.AddValue("contentLength", contentLength);
		serializationInfo.AddValue("webHeaders", webHeaders, typeof(WebHeaderCollection));
		serializationInfo.AddValue("keepAlive", keepAlive);
		serializationInfo.AddValue("maxAutoRedirect", maxAutoRedirect);
		serializationInfo.AddValue("mediaType", mediaType);
		serializationInfo.AddValue("method", method);
		serializationInfo.AddValue("initialMethod", initialMethod);
		serializationInfo.AddValue("pipelined", pipelined);
		serializationInfo.AddValue("version", version, typeof(Version));
		serializationInfo.AddValue("proxy", proxy, typeof(IWebProxy));
		serializationInfo.AddValue("sendChunked", sendChunked);
		serializationInfo.AddValue("timeout", timeout);
		serializationInfo.AddValue("redirects", redirects);
	}
}
