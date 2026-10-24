using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls;

namespace System.Net.Security;

[System.MonoTODO("Non-X509Certificate2 certificate is not supported")]
public class SslStream : AuthenticatedStream
{
	private SslStreamBase ssl_stream;

	private RemoteCertificateValidationCallback validation_callback;

	private LocalCertificateSelectionCallback selection_callback;

	public override bool CanRead => base.InnerStream.CanRead;

	public override bool CanSeek => base.InnerStream.CanSeek;

	public override bool CanWrite => base.InnerStream.CanWrite;

	public override long Length => base.InnerStream.Length;

	public override long Position
	{
		get
		{
			return base.InnerStream.Position;
		}
		set
		{
			throw new NotSupportedException("This stream does not support seek operations");
		}
	}

	public override bool IsAuthenticated => ssl_stream != null;

	[System.MonoTODO("certValidationCallback is not passed X509Chain and SslPolicyErrors correctly")]
	public SslStream(Stream innerStream, bool leaveStreamOpen, RemoteCertificateValidationCallback certValidationCallback)
		: this(innerStream, leaveStreamOpen, certValidationCallback, null)
	{
	}

	[System.MonoTODO("certValidationCallback is not passed X509Chain and SslPolicyErrors correctly")]
	public SslStream(Stream innerStream, bool leaveStreamOpen, RemoteCertificateValidationCallback certValidationCallback, LocalCertificateSelectionCallback certSelectionCallback)
		: base(innerStream, leaveStreamOpen)
	{
		validation_callback = certValidationCallback;
		selection_callback = certSelectionCallback;
	}

	private X509Certificate OnCertificateSelection(X509CertificateCollection clientCerts, X509Certificate serverCert, string targetHost, X509CertificateCollection serverRequestedCerts)
	{
		string[] array = new string[serverRequestedCerts?.Count ?? 0];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = serverRequestedCerts[i].GetIssuerName();
		}
		return selection_callback(this, targetHost, clientCerts, serverCert, array);
	}

	public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols sslProtocolType, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
	{
		if (IsAuthenticated)
		{
			throw new InvalidOperationException("This SslStream is already authenticated");
		}
		SslClientStream sslClientStream = new SslClientStream(base.InnerStream, targetHost, !base.LeaveInnerStreamOpen, GetMonoSslProtocol(sslProtocolType), clientCertificates);
		sslClientStream.CheckCertRevocationStatus = checkCertificateRevocation;
		sslClientStream.PrivateKeyCertSelectionDelegate = (X509Certificate cert, string host) =>
		{
			string certHashString = cert.GetCertHashString();
			foreach (X509Certificate clientCertificate in clientCertificates)
			{
				if (!(clientCertificate.GetCertHashString() != certHashString))
				{
					X509Certificate2 x509Certificate = clientCertificate as X509Certificate2;
					x509Certificate = x509Certificate ?? new X509Certificate2(clientCertificate);
					return x509Certificate.PrivateKey;
				}
			}
			return (AsymmetricAlgorithm)null;
		};
		if (validation_callback != null)
		{
			sslClientStream.ServerCertValidationDelegate = (X509Certificate cert, int[] certErrors) =>
			{
				X509Chain x509Chain = new X509Chain();
				X509Certificate2 x509Certificate = cert as X509Certificate2;
				if (x509Certificate == null)
				{
					x509Certificate = new X509Certificate2(cert);
				}
				if (!ServicePointManager.CheckCertificateRevocationList)
				{
					x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
				}
				SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
				for (int i = 0; i < certErrors.Length; i++)
				{
					sslPolicyErrors = certErrors[i] switch
					{
						-2146762490 => sslPolicyErrors | SslPolicyErrors.RemoteCertificateNotAvailable, 
						-2146762481 => sslPolicyErrors | SslPolicyErrors.RemoteCertificateNameMismatch, 
						_ => sslPolicyErrors | SslPolicyErrors.RemoteCertificateChainErrors, 
					};
				}
				x509Chain.Build(x509Certificate);
				X509ChainStatus[] chainStatus = x509Chain.ChainStatus;
				for (int j = 0; j < chainStatus.Length; j++)
				{
					X509ChainStatus x509ChainStatus = chainStatus[j];
					if (x509ChainStatus.Status != X509ChainStatusFlags.NoError)
					{
						sslPolicyErrors = (((x509ChainStatus.Status & X509ChainStatusFlags.PartialChain) == 0) ? (sslPolicyErrors | SslPolicyErrors.RemoteCertificateChainErrors) : (sslPolicyErrors | SslPolicyErrors.RemoteCertificateNotAvailable));
					}
				}
				return validation_callback(this, cert, x509Chain, sslPolicyErrors);
			};
		}
		if (selection_callback != null)
		{
			sslClientStream.ClientCertSelectionDelegate = OnCertificateSelection;
		}
		ssl_stream = sslClientStream;
		return BeginWrite(new byte[0], 0, 0, asyncCallback, asyncState);
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
	{
		CheckConnectionAuthenticated();
		return ssl_stream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
	}

	private Mono.Security.Protocol.Tls.SecurityProtocolType GetMonoSslProtocol(SslProtocols ms)
	{
		return ms switch
		{
			SslProtocols.Ssl2 => Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl2, 
			SslProtocols.Ssl3 => Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl3, 
			SslProtocols.Tls => Mono.Security.Protocol.Tls.SecurityProtocolType.Tls, 
			_ => Mono.Security.Protocol.Tls.SecurityProtocolType.Default, 
		};
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
	{
		CheckConnectionAuthenticated();
		return ssl_stream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
	}

	public virtual void AuthenticateAsClient(string targetHost)
	{
		AuthenticateAsClient(targetHost, new X509CertificateCollection(), SslProtocols.Tls, checkCertificateRevocation: false);
	}

	public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols sslProtocolType, bool checkCertificateRevocation)
	{
		EndAuthenticateAsClient(BeginAuthenticateAsClient(targetHost, clientCertificates, sslProtocolType, checkCertificateRevocation, null, null));
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (ssl_stream != null)
			{
				ssl_stream.Dispose();
			}
			ssl_stream = null;
		}
		base.Dispose(disposing);
	}

	public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
	{
		CheckConnectionAuthenticated();
		if (CanRead)
		{
			ssl_stream.EndRead(asyncResult);
		}
		else
		{
			ssl_stream.EndWrite(asyncResult);
		}
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		CheckConnectionAuthenticated();
		return ssl_stream.EndRead(asyncResult);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		CheckConnectionAuthenticated();
		ssl_stream.EndWrite(asyncResult);
	}

	public override void Flush()
	{
		CheckConnectionAuthenticated();
		base.InnerStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return EndRead(BeginRead(buffer, offset, count, null, null));
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException("This stream does not support seek operations");
	}

	public override void SetLength(long value)
	{
		base.InnerStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		EndWrite(BeginWrite(buffer, offset, count, null, null));
	}

	private void CheckConnectionAuthenticated()
	{
		if (!IsAuthenticated)
		{
			throw new InvalidOperationException("This operation is invalid until it is successfully authenticated");
		}
	}
}
