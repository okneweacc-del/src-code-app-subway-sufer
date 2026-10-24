using System.Globalization;

namespace System.Security.Cryptography.X509Certificates;

public class X509Certificate2Collection : X509CertificateCollection
{
	public new X509Certificate2 this[int index]
	{
		get
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("negative index");
			}
			if (index >= base.InnerList.Count)
			{
				throw new ArgumentOutOfRangeException("index >= Count");
			}
			return (X509Certificate2)base.InnerList[index];
		}
	}

	public X509Certificate2Collection()
	{
	}

	public X509Certificate2Collection(X509Certificate2Collection certificates)
	{
		AddRange(certificates);
	}

	public int Add(X509Certificate2 certificate)
	{
		if (certificate == null)
		{
			throw new ArgumentNullException("certificate");
		}
		return base.InnerList.Add(certificate);
	}

	[System.MonoTODO("Method isn't transactional (like documented)")]
	public void AddRange(X509Certificate2Collection certificates)
	{
		if (certificates == null)
		{
			throw new ArgumentNullException("certificates");
		}
		base.InnerList.AddRange(certificates);
	}

	public bool Contains(X509Certificate2 certificate)
	{
		if (certificate == null)
		{
			throw new ArgumentNullException("certificate");
		}
		foreach (X509Certificate2 inner in base.InnerList)
		{
			if (inner.Equals(certificate))
			{
				return true;
			}
		}
		return false;
	}

	[System.MonoTODO("Does not support X509FindType.FindByTemplateName, FindByApplicationPolicy and FindByCertificatePolicy")]
	public X509Certificate2Collection Find(X509FindType findType, object findValue, bool validOnly)
	{
		if (findValue == null)
		{
			throw new ArgumentNullException("findValue");
		}
		string text = string.Empty;
		string text2 = string.Empty;
		X509KeyUsageFlags x509KeyUsageFlags = X509KeyUsageFlags.None;
		DateTime dateTime = DateTime.MinValue;
		switch (findType)
		{
		case X509FindType.FindByThumbprint:
		case X509FindType.FindBySubjectName:
		case X509FindType.FindBySubjectDistinguishedName:
		case X509FindType.FindByIssuerName:
		case X509FindType.FindByIssuerDistinguishedName:
		case X509FindType.FindBySerialNumber:
		case X509FindType.FindByTemplateName:
		case X509FindType.FindBySubjectKeyIdentifier:
			try
			{
				text = (string)findValue;
			}
			catch (Exception inner4)
			{
				string text8 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", findValue.GetType(), "string");
				throw new CryptographicException(text8, inner4);
			}
			break;
		case X509FindType.FindByApplicationPolicy:
		case X509FindType.FindByCertificatePolicy:
		case X509FindType.FindByExtension:
			try
			{
				text2 = (string)findValue;
			}
			catch (Exception inner3)
			{
				string text6 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", findValue.GetType(), "X509KeyUsageFlags");
				throw new CryptographicException(text6, inner3);
			}
			try
			{
				CryptoConfig.EncodeOID(text2);
			}
			catch (CryptographicUnexpectedOperationException)
			{
				string text7 = global::Locale.GetText("Invalid OID value '{0}'.", text2);
				throw new ArgumentException("findValue", text7);
			}
			break;
		case X509FindType.FindByKeyUsage:
			try
			{
				x509KeyUsageFlags = (X509KeyUsageFlags)(int)findValue;
			}
			catch (Exception inner2)
			{
				string text5 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", findValue.GetType(), "X509KeyUsageFlags");
				throw new CryptographicException(text5, inner2);
			}
			break;
		case X509FindType.FindByTimeValid:
		case X509FindType.FindByTimeNotYetValid:
		case X509FindType.FindByTimeExpired:
			try
			{
				dateTime = (DateTime)findValue;
			}
			catch (Exception inner)
			{
				string text4 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", findValue.GetType(), "X509DateTime");
				throw new CryptographicException(text4, inner);
			}
			break;
		default:
		{
			string text3 = global::Locale.GetText("Invalid find type '{0}'.", findType);
			throw new CryptographicException(text3);
		}
		}
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		foreach (X509Certificate2 inner5 in base.InnerList)
		{
			bool flag = false;
			switch (findType)
			{
			case X509FindType.FindByThumbprint:
				flag = string.Compare(text, inner5.Thumbprint, ignoreCase: true, invariantCulture) == 0 || string.Compare(text, inner5.GetCertHashString(), ignoreCase: true, invariantCulture) == 0;
				break;
			case X509FindType.FindBySubjectName:
			{
				string nameInfo2 = inner5.GetNameInfo(X509NameType.SimpleName, forIssuer: false);
				flag = nameInfo2.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0;
				break;
			}
			case X509FindType.FindBySubjectDistinguishedName:
				flag = string.Compare(text, inner5.Subject, ignoreCase: true, invariantCulture) == 0;
				break;
			case X509FindType.FindByIssuerName:
			{
				string nameInfo = inner5.GetNameInfo(X509NameType.SimpleName, forIssuer: true);
				flag = nameInfo.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0;
				break;
			}
			case X509FindType.FindByIssuerDistinguishedName:
				flag = string.Compare(text, inner5.Issuer, ignoreCase: true, invariantCulture) == 0;
				break;
			case X509FindType.FindBySerialNumber:
				flag = string.Compare(text, inner5.SerialNumber, ignoreCase: true, invariantCulture) == 0;
				break;
			case X509FindType.FindBySubjectKeyIdentifier:
				if (inner5.Extensions["2.5.29.14"] is X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension)
				{
					flag = string.Compare(text, x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier, ignoreCase: true, invariantCulture) == 0;
				}
				break;
			case X509FindType.FindByApplicationPolicy:
				flag = inner5.Extensions.Count == 0;
				break;
			case X509FindType.FindByExtension:
				flag = inner5.Extensions[text2] != null;
				break;
			case X509FindType.FindByKeyUsage:
				flag = !(inner5.Extensions["2.5.29.15"] is X509KeyUsageExtension x509KeyUsageExtension) || (x509KeyUsageExtension.KeyUsages & x509KeyUsageFlags) == x509KeyUsageFlags;
				break;
			case X509FindType.FindByTimeValid:
				flag = dateTime >= inner5.NotBefore && dateTime <= inner5.NotAfter;
				break;
			case X509FindType.FindByTimeNotYetValid:
				flag = dateTime < inner5.NotBefore;
				break;
			case X509FindType.FindByTimeExpired:
				flag = dateTime > inner5.NotAfter;
				break;
			}
			if (!flag)
			{
				continue;
			}
			if (validOnly)
			{
				try
				{
					if (inner5.Verify())
					{
						x509Certificate2Collection.Add(inner5);
					}
				}
				catch
				{
				}
			}
			else
			{
				x509Certificate2Collection.Add(inner5);
			}
		}
		return x509Certificate2Collection;
	}

	public new X509Certificate2Enumerator GetEnumerator()
	{
		return new X509Certificate2Enumerator(this);
	}

	[System.MonoTODO("same limitations as X509Certificate2.Import")]
	public void Import(byte[] rawData)
	{
		X509Certificate2 x509Certificate = new X509Certificate2();
		x509Certificate.Import(rawData);
		Add(x509Certificate);
	}
}
