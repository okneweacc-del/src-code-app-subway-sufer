using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Mono.Security.X509;

internal class X509Certificate : ISerializable
{
	private ASN1 decoder;

	private byte[] m_encodedcert;

	private DateTime m_from;

	private DateTime m_until;

	private ASN1 issuer;

	private string m_issuername;

	private string m_keyalgo;

	private byte[] m_keyalgoparams;

	private ASN1 subject;

	private string m_subject;

	private byte[] m_publickey;

	private byte[] signature;

	private string m_signaturealgo;

	private byte[] m_signaturealgoparams;

	private DSA _dsa;

	private int version;

	private byte[] serialnumber;

	private byte[] issuerUniqueID;

	private byte[] subjectUniqueID;

	private X509ExtensionCollection extensions;

	private static string encoding_error = Locale.GetText("Input data cannot be coded as a valid certificate.");

	public DSA DSA
	{
		get
		{
			if (m_keyalgoparams == null)
			{
				throw new CryptographicException("Missing key algorithm parameters.");
			}
			if (_dsa == null)
			{
				DSAParameters parameters = default;
				ASN1 aSN = new ASN1(m_publickey);
				if (aSN == null || aSN.Tag != 2)
				{
					return null;
				}
				parameters.Y = GetUnsignedBigInteger(aSN.Value);
				ASN1 aSN2 = new ASN1(m_keyalgoparams);
				if (aSN2 == null || aSN2.Tag != 48 || aSN2.Count < 3)
				{
					return null;
				}
				if (aSN2[0].Tag != 2 || aSN2[1].Tag != 2 || aSN2[2].Tag != 2)
				{
					return null;
				}
				parameters.P = GetUnsignedBigInteger(aSN2[0].Value);
				parameters.Q = GetUnsignedBigInteger(aSN2[1].Value);
				parameters.G = GetUnsignedBigInteger(aSN2[2].Value);
				_dsa = new DSACryptoServiceProvider(parameters.Y.Length << 3);
				_dsa.ImportParameters(parameters);
			}
			return _dsa;
		}
	}

	public virtual string IssuerName => m_issuername;

	public virtual byte[] KeyAlgorithmParameters
	{
		get
		{
			if (m_keyalgoparams == null)
			{
				return null;
			}
			return (byte[])m_keyalgoparams.Clone();
		}
	}

	public virtual byte[] PublicKey
	{
		get
		{
			if (m_publickey == null)
			{
				return null;
			}
			return (byte[])m_publickey.Clone();
		}
	}

	public virtual byte[] RawData
	{
		get
		{
			if (m_encodedcert == null)
			{
				return null;
			}
			return (byte[])m_encodedcert.Clone();
		}
	}

	public virtual string SubjectName => m_subject;

	public virtual DateTime ValidFrom => m_from;

	public virtual DateTime ValidUntil => m_until;

	public X509Certificate(byte[] data)
	{
		if (data == null)
		{
			return;
		}
		if (data.Length > 0 && data[0] != 48)
		{
			try
			{
				data = PEM("CERTIFICATE", data);
			}
			catch (Exception inner)
			{
				throw new CryptographicException(encoding_error, inner);
			}
		}
		Parse(data);
	}

	private void Parse(byte[] data)
	{
		try
		{
			decoder = new ASN1(data);
			if (decoder.Tag != 48)
			{
				throw new CryptographicException(encoding_error);
			}
			if (decoder[0].Tag != 48)
			{
				throw new CryptographicException(encoding_error);
			}
			ASN1 aSN = decoder[0];
			int num = 0;
			ASN1 aSN2 = decoder[0][num];
			version = 1;
			if (aSN2.Tag == 160 && aSN2.Count > 0)
			{
				version += aSN2[0].Value[0];
				num++;
			}
			ASN1 aSN3 = decoder[0][num++];
			if (aSN3.Tag != 2)
			{
				throw new CryptographicException(encoding_error);
			}
			serialnumber = aSN3.Value;
			Array.Reverse(serialnumber, 0, serialnumber.Length);
			num++;
			issuer = aSN.Element(num++, 48);
			m_issuername = X501.ToString(issuer);
			ASN1 aSN4 = aSN.Element(num++, 48);
			ASN1 time = aSN4[0];
			m_from = ASN1Convert.ToDateTime(time);
			ASN1 time2 = aSN4[1];
			m_until = ASN1Convert.ToDateTime(time2);
			subject = aSN.Element(num++, 48);
			m_subject = X501.ToString(subject);
			ASN1 aSN5 = aSN.Element(num++, 48);
			ASN1 aSN6 = aSN5.Element(0, 48);
			ASN1 asn = aSN6.Element(0, 6);
			m_keyalgo = ASN1Convert.ToOid(asn);
			ASN1 aSN7 = aSN6[1];
			m_keyalgoparams = ((aSN6.Count <= 1) ? null : aSN7.GetBytes());
			ASN1 aSN8 = aSN5.Element(1, 3);
			int num2 = aSN8.Length - 1;
			m_publickey = new byte[num2];
			Buffer.BlockCopy(aSN8.Value, 1, m_publickey, 0, num2);
			byte[] value = decoder[2].Value;
			signature = new byte[value.Length - 1];
			Buffer.BlockCopy(value, 1, signature, 0, signature.Length);
			aSN6 = decoder[1];
			asn = aSN6.Element(0, 6);
			m_signaturealgo = ASN1Convert.ToOid(asn);
			aSN7 = aSN6[1];
			if (aSN7 != null)
			{
				m_signaturealgoparams = aSN7.GetBytes();
			}
			else
			{
				m_signaturealgoparams = null;
			}
			ASN1 aSN9 = aSN.Element(num, 129);
			if (aSN9 != null)
			{
				num++;
				issuerUniqueID = aSN9.Value;
			}
			ASN1 aSN10 = aSN.Element(num, 130);
			if (aSN10 != null)
			{
				num++;
				subjectUniqueID = aSN10.Value;
			}
			ASN1 aSN11 = aSN.Element(num, 163);
			if (aSN11 != null && aSN11.Count == 1)
			{
				extensions = new X509ExtensionCollection(aSN11[0]);
			}
			else
			{
				extensions = new X509ExtensionCollection(null);
			}
			m_encodedcert = (byte[])data.Clone();
		}
		catch (Exception inner)
		{
			throw new CryptographicException(encoding_error, inner);
		}
	}

	private byte[] GetUnsignedBigInteger(byte[] integer)
	{
		if (integer[0] == 0)
		{
			int num = integer.Length - 1;
			byte[] array = new byte[num];
			Buffer.BlockCopy(integer, 1, array, 0, num);
			return array;
		}
		return integer;
	}

	public ASN1 GetIssuerName()
	{
		return issuer;
	}

	public ASN1 GetSubjectName()
	{
		return subject;
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("raw", m_encodedcert);
	}

	private static byte[] PEM(string type, byte[] data)
	{
		string text = Encoding.ASCII.GetString(data);
		string text2 = $"-----BEGIN {type}-----";
		string value = $"-----END {type}-----";
		int num = text.IndexOf(text2) + text2.Length;
		int num2 = text.IndexOf(value, num);
		string s = text.Substring(num, num2 - num);
		return Convert.FromBase64String(s);
	}
}
