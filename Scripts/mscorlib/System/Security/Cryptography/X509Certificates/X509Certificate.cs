using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates;

[Serializable]
[MonoTODO("X509ContentType.SerializedCert isn't supported (anywhere in the class)")]
[ComVisible(true)]
public class X509Certificate : ISerializable, IDeserializationCallback
{
	private Mono.Security.X509.X509Certificate x509;

	private bool hideDates;

	private byte[] cachedCertificateHash;

	private string issuer_name;

	private string subject_name;

	public string Issuer
	{
		get
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			if (issuer_name == null)
			{
				issuer_name = X501.ToString(x509.GetIssuerName(), reversed: true, ", ", quotes: true);
			}
			return issuer_name;
		}
	}

	public string Subject
	{
		get
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			if (subject_name == null)
			{
				subject_name = X501.ToString(x509.GetSubjectName(), reversed: true, ", ", quotes: true);
			}
			return subject_name;
		}
	}

	internal X509Certificate(byte[] data, bool dates)
	{
		if (data != null)
		{
			Import(data, null, X509KeyStorageFlags.DefaultKeySet);
			hideDates = !dates;
		}
	}

	public X509Certificate(byte[] data)
		: this(data, dates: true)
	{
	}

	public X509Certificate(X509Certificate cert)
	{
		if (cert == null)
		{
			throw new ArgumentNullException("cert");
		}
		if (cert != null)
		{
			byte[] rawCertData = cert.GetRawCertData();
			if (rawCertData != null)
			{
				x509 = new Mono.Security.X509.X509Certificate(rawCertData);
			}
			hideDates = false;
		}
	}

	public X509Certificate()
	{
	}

	public X509Certificate(SerializationInfo info, StreamingContext context)
	{
		byte[] rawData = (byte[])info.GetValue("RawData", typeof(byte[]));
		Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
	}

	void IDeserializationCallback.OnDeserialization(object sender)
	{
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("RawData", x509.RawData);
	}

	private string tostr(byte[] data)
	{
		if (data != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				stringBuilder.Append(data[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}
		return null;
	}

	public virtual bool Equals(X509Certificate other)
	{
		if (other == null)
		{
			return false;
		}
		if (other.x509 == null)
		{
			if (x509 == null)
			{
				return true;
			}
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		byte[] rawData = other.x509.RawData;
		if (rawData != null)
		{
			if (x509 == null)
			{
				return false;
			}
			if (x509.RawData == null)
			{
				return false;
			}
			if (rawData.Length == x509.RawData.Length)
			{
				for (int i = 0; i < rawData.Length; i++)
				{
					if (rawData[i] != x509.RawData[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		return x509 == null || x509.RawData == null;
	}

	public virtual byte[] GetCertHash()
	{
		if (x509 == null)
		{
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		if (cachedCertificateHash == null && x509 != null)
		{
			SHA1 sHA = SHA1.Create();
			cachedCertificateHash = sHA.ComputeHash(x509.RawData);
		}
		return cachedCertificateHash;
	}

	public virtual string GetCertHashString()
	{
		return tostr(GetCertHash());
	}

	public virtual string GetEffectiveDateString()
	{
		if (hideDates)
		{
			return null;
		}
		if (x509 == null)
		{
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		return x509.ValidFrom.ToLocalTime().ToString();
	}

	public virtual string GetExpirationDateString()
	{
		if (hideDates)
		{
			return null;
		}
		if (x509 == null)
		{
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		return x509.ValidUntil.ToLocalTime().ToString();
	}

	public override int GetHashCode()
	{
		if (x509 == null)
		{
			return 0;
		}
		if (cachedCertificateHash == null)
		{
			GetCertHash();
		}
		if (cachedCertificateHash != null && cachedCertificateHash.Length >= 4)
		{
			return (cachedCertificateHash[0] << 24) | (cachedCertificateHash[1] << 16) | (cachedCertificateHash[2] << 8) | cachedCertificateHash[3];
		}
		return 0;
	}

	[Obsolete("Use the Issuer property.")]
	public virtual string GetIssuerName()
	{
		if (x509 == null)
		{
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		return x509.IssuerName;
	}

	[Obsolete("Use the Subject property.")]
	public virtual string GetName()
	{
		if (x509 == null)
		{
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		return x509.SubjectName;
	}

	public virtual byte[] GetPublicKey()
	{
		if (x509 == null)
		{
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		return x509.PublicKey;
	}

	public virtual byte[] GetRawCertData()
	{
		if (x509 == null)
		{
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}
		return x509.RawData;
	}

	public override string ToString()
	{
		return base.ToString();
	}

	public virtual string ToString(bool fVerbose)
	{
		if (!fVerbose || x509 == null)
		{
			return base.ToString();
		}
		string newLine = Environment.NewLine;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, Subject);
		stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, Issuer);
		stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, GetEffectiveDateString());
		stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, GetExpirationDateString());
		stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}", newLine, GetCertHashString());
		stringBuilder.Append(newLine);
		return stringBuilder.ToString();
	}

	[ComVisible(false)]
	public override bool Equals(object obj)
	{
		if (obj is X509Certificate other)
		{
			return Equals(other);
		}
		return false;
	}

	[ComVisible(false)]
	public virtual void Import(byte[] rawData)
	{
		Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
	}

	[MonoTODO("missing KeyStorageFlags support")]
	[ComVisible(false)]
	public virtual void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
	{
		Reset();
		if (password == null)
		{
			try
			{
				x509 = new Mono.Security.X509.X509Certificate(rawData);
				return;
			}
			catch (Exception inner)
			{
				try
				{
					PKCS12 pKCS = new PKCS12(rawData);
					if (pKCS.Certificates.Count > 0)
					{
						x509 = pKCS.Certificates[0];
					}
					else
					{
						x509 = null;
					}
					return;
				}
				catch
				{
					string text = Locale.GetText("Unable to decode certificate.");
					throw new CryptographicException(text, inner);
				}
			}
		}
		try
		{
			PKCS12 pKCS2 = new PKCS12(rawData, password);
			if (pKCS2.Certificates.Count > 0)
			{
				x509 = pKCS2.Certificates[0];
			}
			else
			{
				x509 = null;
			}
		}
		catch
		{
			x509 = new Mono.Security.X509.X509Certificate(rawData);
		}
	}

	[ComVisible(false)]
	public virtual void Reset()
	{
		x509 = null;
		issuer_name = null;
		subject_name = null;
		hideDates = false;
		cachedCertificateHash = null;
	}
}
