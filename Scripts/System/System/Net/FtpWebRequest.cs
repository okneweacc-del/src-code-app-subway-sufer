using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace System.Net;

public sealed class FtpWebRequest : WebRequest
{
	private Uri requestUri;

	private IWebProxy proxy;

	private int timeout = 100000;

	private int rwTimeout = 300000;

	private bool binary = true;

	private bool usePassive = true;

	private string method = "RETR";

	private object locker = new object();

	private static readonly string[] supportedCommands = new string[13]
	{
		"APPE", "DELE", "LIST", "MDTM", "MKD", "NLST", "PWD", "RENAME", "RETR", "RMD",
		"SIZE", "STOR", "STOU"
	};

	private RemoteCertificateValidationCallback callback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
	{
		if (ServicePointManager.ServerCertificateValidationCallback != null)
		{
			return ServicePointManager.ServerCertificateValidationCallback(sender, certificate, chain, sslPolicyErrors);
		}
		if (sslPolicyErrors != SslPolicyErrors.None)
		{
			throw new InvalidOperationException("SSL authentication error: " + sslPolicyErrors);
		}
		return true;
	};

	internal FtpWebRequest(Uri uri)
	{
		requestUri = uri;
		proxy = GlobalProxySelection.Select;
	}
}
