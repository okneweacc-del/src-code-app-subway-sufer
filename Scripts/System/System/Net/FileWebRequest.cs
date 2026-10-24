using System.IO;
using System.Runtime.Serialization;

namespace System.Net;

[Serializable]
public class FileWebRequest : WebRequest, ISerializable
{
	private Uri uri;

	private WebHeaderCollection webHeaders;

	private string connectionGroup;

	private long contentLength;

	private FileAccess fileAccess = FileAccess.Read;

	private string method = "GET";

	private IWebProxy proxy;

	private bool preAuthenticate;

	private int timeout = 100000;

	internal FileWebRequest(Uri uri)
	{
		this.uri = uri;
		webHeaders = new WebHeaderCollection();
	}

	[Obsolete("Serialization is obsoleted for this type", false)]
	protected FileWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		webHeaders = (WebHeaderCollection)serializationInfo.GetValue("headers", typeof(WebHeaderCollection));
		proxy = (IWebProxy)serializationInfo.GetValue("proxy", typeof(IWebProxy));
		uri = (Uri)serializationInfo.GetValue("uri", typeof(Uri));
		connectionGroup = serializationInfo.GetString("connectionGroupName");
		method = serializationInfo.GetString("method");
		contentLength = serializationInfo.GetInt64("contentLength");
		timeout = serializationInfo.GetInt32("timeout");
		fileAccess = (FileAccess)(int)serializationInfo.GetValue("fileAccess", typeof(FileAccess));
		preAuthenticate = serializationInfo.GetBoolean("preauthenticate");
	}

	void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		GetObjectData(serializationInfo, streamingContext);
	}

	protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		serializationInfo.AddValue("headers", webHeaders, typeof(WebHeaderCollection));
		serializationInfo.AddValue("proxy", proxy, typeof(IWebProxy));
		serializationInfo.AddValue("uri", uri, typeof(Uri));
		serializationInfo.AddValue("connectionGroupName", connectionGroup);
		serializationInfo.AddValue("method", method);
		serializationInfo.AddValue("contentLength", contentLength);
		serializationInfo.AddValue("timeout", timeout);
		serializationInfo.AddValue("fileAccess", fileAccess);
		serializationInfo.AddValue("preauthenticate", value: false);
	}
}
