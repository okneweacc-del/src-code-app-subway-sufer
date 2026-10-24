using System;
using System.IO;

namespace Mono.Security.X509;

public class X509Stores
{
	private string _storePath;

	private X509Store _trusted;

	public X509Store TrustedRoot
	{
		get
		{
			if (_trusted == null)
			{
				string path = Path.Combine(_storePath, "Trust");
				_trusted = new X509Store(path, crl: true);
			}
			return _trusted;
		}
	}

	internal X509Stores(string path)
	{
		_storePath = path;
	}

	public X509Store Open(string storeName, bool create)
	{
		if (storeName == null)
		{
			throw new ArgumentNullException("storeName");
		}
		string path = Path.Combine(_storePath, storeName);
		if (!create && !Directory.Exists(path))
		{
			return null;
		}
		return new X509Store(path, crl: true);
	}
}
