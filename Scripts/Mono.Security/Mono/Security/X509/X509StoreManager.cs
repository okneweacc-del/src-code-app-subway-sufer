using System;
using System.IO;

namespace Mono.Security.X509;

public sealed class X509StoreManager
{
	private static X509Stores _userStore;

	private static X509Stores _machineStore;

	public static X509Stores CurrentUser
	{
		get
		{
			if (_userStore == null)
			{
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".mono");
				path = Path.Combine(path, "certs");
				_userStore = new X509Stores(path);
			}
			return _userStore;
		}
	}

	public static X509Stores LocalMachine
	{
		get
		{
			if (_machineStore == null)
			{
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ".mono");
				path = Path.Combine(path, "certs");
				_machineStore = new X509Stores(path);
			}
			return _machineStore;
		}
	}

	public static X509CertificateCollection TrustedRootCertificates
	{
		get
		{
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
			x509CertificateCollection.AddRange(CurrentUser.TrustedRoot.Certificates);
			x509CertificateCollection.AddRange(LocalMachine.TrustedRoot.Certificates);
			return x509CertificateCollection;
		}
	}
}
