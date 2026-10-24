using System.Text;
using Mono.Security;

namespace System.Security.Cryptography.X509Certificates;

public sealed class X509EnhancedKeyUsageExtension : X509Extension
{
	private OidCollection _enhKeyUsage;

	private System.Security.Cryptography.AsnDecodeStatus _status;

	public X509EnhancedKeyUsageExtension(AsnEncodedData encodedEnhancedKeyUsages, bool critical)
	{
		_oid = new Oid("2.5.29.37", "Enhanced Key Usage");
		_raw = encodedEnhancedKeyUsages.RawData;
		base.Critical = critical;
		_status = Decode(base.RawData);
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		if (asnEncodedData == null)
		{
			throw new ArgumentNullException("encodedData");
		}
		if (!(asnEncodedData is X509Extension x509Extension))
		{
			throw new ArgumentException(global::Locale.GetText("Wrong type."), "asnEncodedData");
		}
		if (x509Extension._oid == null)
		{
			_oid = new Oid("2.5.29.37", "Enhanced Key Usage");
		}
		else
		{
			_oid = new Oid(x509Extension._oid);
		}
		base.RawData = x509Extension.RawData;
		base.Critical = x509Extension.Critical;
		_status = Decode(base.RawData);
	}

	internal System.Security.Cryptography.AsnDecodeStatus Decode(byte[] extension)
	{
		if (extension == null || extension.Length == 0)
		{
			return System.Security.Cryptography.AsnDecodeStatus.BadAsn;
		}
		if (extension[0] != 48)
		{
			return System.Security.Cryptography.AsnDecodeStatus.BadTag;
		}
		if (_enhKeyUsage == null)
		{
			_enhKeyUsage = new OidCollection();
		}
		try
		{
			ASN1 aSN = new ASN1(extension);
			if (aSN.Tag != 48)
			{
				throw new CryptographicException(global::Locale.GetText("Invalid ASN.1 Tag"));
			}
			for (int i = 0; i < aSN.Count; i++)
			{
				_enhKeyUsage.Add(new Oid(ASN1Convert.ToOid(aSN[i])));
			}
		}
		catch
		{
			return System.Security.Cryptography.AsnDecodeStatus.BadAsn;
		}
		return System.Security.Cryptography.AsnDecodeStatus.Ok;
	}

	internal override string ToString(bool multiLine)
	{
		switch (_status)
		{
		case System.Security.Cryptography.AsnDecodeStatus.BadAsn:
			return string.Empty;
		case System.Security.Cryptography.AsnDecodeStatus.BadTag:
		case System.Security.Cryptography.AsnDecodeStatus.BadLength:
			return FormatUnkownData(_raw);
		case System.Security.Cryptography.AsnDecodeStatus.InformationNotAvailable:
			return "Information Not Available";
		default:
		{
			if (_oid.Value != "2.5.29.37")
			{
				return $"Unknown Key Usage ({_oid.Value})";
			}
			if (_enhKeyUsage.Count == 0)
			{
				return "Information Not Available";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _enhKeyUsage.Count; i++)
			{
				Oid oid = _enhKeyUsage[i];
				switch (oid.Value)
				{
				case "1.3.6.1.5.5.7.3.1":
					stringBuilder.Append("Server Authentication (");
					break;
				default:
					stringBuilder.Append("Unknown Key Usage (");
					break;
				}
				stringBuilder.Append(oid.Value);
				stringBuilder.Append(")");
				if (multiLine)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				else if (i != _enhKeyUsage.Count - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}
		}
	}
}
