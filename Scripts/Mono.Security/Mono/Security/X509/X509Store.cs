using System.Collections;
using System.IO;

namespace Mono.Security.X509;

public class X509Store
{
	private string _storePath;

	private X509CertificateCollection _certificates;

	private ArrayList _crls;

	private bool _crl;

	public X509CertificateCollection Certificates
	{
		get
		{
			if (_certificates == null)
			{
				_certificates = BuildCertificatesCollection(_storePath);
			}
			return _certificates;
		}
	}

	public ArrayList Crls
	{
		get
		{
			if (!_crl)
			{
				_crls = new ArrayList();
			}
			if (_crls == null)
			{
				_crls = BuildCrlsCollection(_storePath);
			}
			return _crls;
		}
	}

	internal X509Store(string path, bool crl)
	{
		_storePath = path;
		_crl = crl;
	}

	private byte[] Load(string filename)
	{
		byte[] array = null;
		using FileStream fileStream = File.OpenRead(filename);
		array = new byte[fileStream.Length];
		fileStream.Read(array, 0, array.Length);
		fileStream.Close();
		return array;
	}

	private X509Certificate LoadCertificate(string filename)
	{
		byte[] data = Load(filename);
		return new X509Certificate(data);
	}

	private X509Crl LoadCrl(string filename)
	{
		byte[] crl = Load(filename);
		return new X509Crl(crl);
	}

	private bool CheckStore(string path, bool throwException)
	{
		try
		{
			if (Directory.Exists(path))
			{
				return true;
			}
			Directory.CreateDirectory(path);
			return Directory.Exists(path);
		}
		catch
		{
			if (throwException)
			{
				throw;
			}
			return false;
		}
	}

	private X509CertificateCollection BuildCertificatesCollection(string storeName)
	{
		X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
		string path = Path.Combine(_storePath, storeName);
		if (!CheckStore(path, throwException: false))
		{
			return x509CertificateCollection;
		}
		string[] files = Directory.GetFiles(path, "*.cer");
		if (files != null && files.Length > 0)
		{
			string[] array = files;
			foreach (string filename in array)
			{
				try
				{
					X509Certificate value = LoadCertificate(filename);
					x509CertificateCollection.Add(value);
				}
				catch
				{
				}
			}
		}
		return x509CertificateCollection;
	}

	private ArrayList BuildCrlsCollection(string storeName)
	{
		ArrayList arrayList = new ArrayList();
		string path = Path.Combine(_storePath, storeName);
		if (!CheckStore(path, throwException: false))
		{
			return arrayList;
		}
		string[] files = Directory.GetFiles(path, "*.crl");
		if (files != null && files.Length > 0)
		{
			string[] array = files;
			foreach (string filename in array)
			{
				try
				{
					X509Crl value = LoadCrl(filename);
					arrayList.Add(value);
				}
				catch
				{
				}
			}
		}
		return arrayList;
	}
}
