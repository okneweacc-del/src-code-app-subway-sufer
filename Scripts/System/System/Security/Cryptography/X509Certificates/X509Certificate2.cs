using System.Text;
using Mono.Security;
using Mono.Security.Cryptography;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates;

public class X509Certificate2 : X509Certificate
{
	private bool _archived;

	private X509ExtensionCollection _extensions;

	private string _name = string.Empty;

	private string _serial;

	private PublicKey _publicKey;

	private X500DistinguishedName issuer_name;

	private X500DistinguishedName subject_name;

	private Oid signature_algorithm;

	private Mono.Security.X509.X509Certificate _cert;

	private static string empty_error = global::Locale.GetText("Certificate instance is empty.");

	private static byte[] commonName = new byte[3] { 85, 4, 3 };

	private static byte[] email = new byte[9] { 42, 134, 72, 134, 247, 13, 1, 9, 1 };

	private static byte[] signedData = new byte[9] { 42, 134, 72, 134, 247, 13, 1, 7, 2 };

	public X509ExtensionCollection Extensions
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			if (_extensions == null)
			{
				_extensions = new X509ExtensionCollection(_cert);
			}
			return _extensions;
		}
	}

	public X500DistinguishedName IssuerName
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			if (issuer_name == null)
			{
				issuer_name = new X500DistinguishedName(_cert.GetIssuerName().GetBytes());
			}
			return issuer_name;
		}
	}

	public DateTime NotAfter
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			return _cert.ValidUntil.ToLocalTime();
		}
	}

	public DateTime NotBefore
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			return _cert.ValidFrom.ToLocalTime();
		}
	}

	public AsymmetricAlgorithm PrivateKey
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			try
			{
				if (_cert.RSA != null)
				{
					if (_cert.RSA is RSACryptoServiceProvider rSACryptoServiceProvider)
					{
						return (!rSACryptoServiceProvider.PublicOnly) ? rSACryptoServiceProvider : null;
					}
					if (_cert.RSA is RSAManaged rSAManaged)
					{
						return (!rSAManaged.PublicOnly) ? rSAManaged : null;
					}
					_cert.RSA.ExportParameters(includePrivateParameters: true);
					return _cert.RSA;
				}
				if (_cert.DSA != null)
				{
					if (_cert.DSA is DSACryptoServiceProvider dSACryptoServiceProvider)
					{
						return (!dSACryptoServiceProvider.PublicOnly) ? dSACryptoServiceProvider : null;
					}
					_cert.DSA.ExportParameters(includePrivateParameters: true);
					return _cert.DSA;
				}
			}
			catch
			{
			}
			return null;
		}
	}

	public PublicKey PublicKey
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			if (_publicKey == null)
			{
				try
				{
					_publicKey = new PublicKey(_cert);
				}
				catch (Exception inner)
				{
					string text = global::Locale.GetText("Unable to decode public key.");
					throw new CryptographicException(text, inner);
				}
			}
			return _publicKey;
		}
	}

	public string SerialNumber
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			if (_serial == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				byte[] serialNumber = _cert.SerialNumber;
				for (int num = serialNumber.Length - 1; num >= 0; num--)
				{
					stringBuilder.Append(serialNumber[num].ToString("X2"));
				}
				_serial = stringBuilder.ToString();
			}
			return _serial;
		}
	}

	public Oid SignatureAlgorithm
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			if (signature_algorithm == null)
			{
				signature_algorithm = new Oid(_cert.SignatureAlgorithm);
			}
			return signature_algorithm;
		}
	}

	public X500DistinguishedName SubjectName
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			if (subject_name == null)
			{
				subject_name = new X500DistinguishedName(_cert.GetSubjectName().GetBytes());
			}
			return subject_name;
		}
	}

	public string Thumbprint => base.GetCertHashString();

	public int Version
	{
		get
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			return _cert.Version;
		}
	}

	internal Mono.Security.X509.X509Certificate MonoCertificate => _cert;

	public X509Certificate2()
	{
		_cert = null;
	}

	public X509Certificate2(byte[] rawData)
	{
		Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
	}

	public X509Certificate2(X509Certificate certificate)
		: base(certificate)
	{
		_cert = new Mono.Security.X509.X509Certificate(base.GetRawCertData());
	}

	[System.MonoTODO("always return String.Empty for UpnName, DnsFromAlternativeName and UrlName")]
	public string GetNameInfo(X509NameType nameType, bool forIssuer)
	{
		switch (nameType)
		{
		case X509NameType.SimpleName:
		{
			if (_cert == null)
			{
				throw new CryptographicException(empty_error);
			}
			ASN1 aSN2 = ((!forIssuer) ? _cert.GetSubjectName() : _cert.GetIssuerName());
			ASN1 aSN3 = Find(commonName, aSN2);
			if (aSN3 != null)
			{
				return GetValueAsString(aSN3);
			}
			if (aSN2.Count == 0)
			{
				return string.Empty;
			}
			ASN1 aSN4 = aSN2[aSN2.Count - 1];
			if (aSN4.Count == 0)
			{
				return string.Empty;
			}
			return GetValueAsString(aSN4[0]);
		}
		case X509NameType.EmailName:
		{
			ASN1 aSN5 = Find(email, (!forIssuer) ? _cert.GetSubjectName() : _cert.GetIssuerName());
			if (aSN5 != null)
			{
				return GetValueAsString(aSN5);
			}
			return string.Empty;
		}
		case X509NameType.UpnName:
			return string.Empty;
		case X509NameType.DnsName:
		{
			ASN1 aSN = Find(commonName, (!forIssuer) ? _cert.GetSubjectName() : _cert.GetIssuerName());
			if (aSN != null)
			{
				return GetValueAsString(aSN);
			}
			return string.Empty;
		}
		case X509NameType.DnsFromAlternativeName:
			return string.Empty;
		case X509NameType.UrlName:
			return string.Empty;
		default:
			throw new ArgumentException("nameType");
		}
	}

	private ASN1 Find(byte[] oid, ASN1 dn)
	{
		if (dn.Count == 0)
		{
			return null;
		}
		for (int i = 0; i < dn.Count; i++)
		{
			ASN1 aSN = dn[i];
			for (int j = 0; j < aSN.Count; j++)
			{
				ASN1 aSN2 = aSN[j];
				if (aSN2.Count == 2)
				{
					ASN1 aSN3 = aSN2[0];
					if (aSN3 != null && aSN3.CompareValue(oid))
					{
						return aSN2;
					}
				}
			}
		}
		return null;
	}

	private string GetValueAsString(ASN1 pair)
	{
		if (pair.Count != 2)
		{
			return string.Empty;
		}
		ASN1 aSN = pair[1];
		if (aSN.Value == null || aSN.Length == 0)
		{
			return string.Empty;
		}
		if (aSN.Tag == 30)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 1; i < aSN.Value.Length; i += 2)
			{
				stringBuilder.Append((char)aSN.Value[i]);
			}
			return stringBuilder.ToString();
		}
		return Encoding.UTF8.GetString(aSN.Value);
	}

	private void ImportPkcs12(byte[] rawData, string password)
	{
		PKCS12 pKCS = ((password != null) ? new PKCS12(rawData, password) : new PKCS12(rawData));
		if (pKCS.Certificates.Count > 0)
		{
			_cert = pKCS.Certificates[0];
		}
		else
		{
			_cert = null;
		}
		if (pKCS.Keys.Count > 0)
		{
			_cert.RSA = pKCS.Keys[0] as RSA;
			_cert.DSA = pKCS.Keys[0] as DSA;
		}
	}

	public override void Import(byte[] rawData)
	{
		Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
	}

	[System.MonoTODO("missing KeyStorageFlags support")]
	public override void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
	{
		base.Import(rawData, password, keyStorageFlags);
		if (password == null)
		{
			try
			{
				_cert = new Mono.Security.X509.X509Certificate(rawData);
				return;
			}
			catch (Exception inner)
			{
				try
				{
					ImportPkcs12(rawData, null);
					return;
				}
				catch
				{
					string text = global::Locale.GetText("Unable to decode certificate.");
					throw new CryptographicException(text, inner);
				}
			}
		}
		try
		{
			ImportPkcs12(rawData, password);
		}
		catch
		{
			_cert = new Mono.Security.X509.X509Certificate(rawData);
		}
	}

	public override void Reset()
	{
		_cert = null;
		_archived = false;
		_extensions = null;
		_name = string.Empty;
		_serial = null;
		_publicKey = null;
		issuer_name = null;
		subject_name = null;
		signature_algorithm = null;
		base.Reset();
	}

	public override string ToString()
	{
		if (_cert == null)
		{
			return "System.Security.Cryptography.X509Certificates.X509Certificate2";
		}
		return base.ToString(fVerbose: true);
	}

	public override string ToString(bool verbose)
	{
		if (_cert == null)
		{
			return "System.Security.Cryptography.X509Certificates.X509Certificate2";
		}
		if (!verbose)
		{
			return base.ToString(fVerbose: true);
		}
		string newLine = Environment.NewLine;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("[Version]{0}  V{1}{0}{0}", newLine, Version);
		stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, base.Subject);
		stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, base.Issuer);
		stringBuilder.AppendFormat("[Serial Number]{0}  {1}{0}{0}", newLine, SerialNumber);
		stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, NotBefore);
		stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, NotAfter);
		stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}{0}", newLine, Thumbprint);
		stringBuilder.AppendFormat("[Signature Algorithm]{0}  {1}({2}){0}{0}", newLine, SignatureAlgorithm.FriendlyName, SignatureAlgorithm.Value);
		AsymmetricAlgorithm key = PublicKey.Key;
		stringBuilder.AppendFormat("[Public Key]{0}  Algorithm: ", newLine);
		if (key is RSA)
		{
			stringBuilder.Append("RSA");
		}
		else if (key is DSA)
		{
			stringBuilder.Append("DSA");
		}
		else
		{
			stringBuilder.Append(key.ToString());
		}
		stringBuilder.AppendFormat("{0}  Length: {1}{0}  Key Blob: ", newLine, key.KeySize);
		AppendBuffer(stringBuilder, PublicKey.EncodedKeyValue.RawData);
		stringBuilder.AppendFormat("{0}  Parameters: ", newLine);
		AppendBuffer(stringBuilder, PublicKey.EncodedParameters.RawData);
		stringBuilder.Append(newLine);
		return stringBuilder.ToString();
	}

	private static void AppendBuffer(StringBuilder sb, byte[] buffer)
	{
		if (buffer == null)
		{
			return;
		}
		for (int i = 0; i < buffer.Length; i++)
		{
			sb.Append(buffer[i].ToString("x2"));
			if (i < buffer.Length - 1)
			{
				sb.Append(" ");
			}
		}
	}

	[System.MonoTODO("by default this depends on the incomplete X509Chain")]
	public bool Verify()
	{
		if (_cert == null)
		{
			throw new CryptographicException(empty_error);
		}
		X509Chain x509Chain = (X509Chain)CryptoConfig.CreateFromName("X509Chain");
		if (!x509Chain.Build(this))
		{
			return false;
		}
		return true;
	}
}
