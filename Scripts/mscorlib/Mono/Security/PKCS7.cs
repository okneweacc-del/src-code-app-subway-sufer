using System;

namespace Mono.Security;

internal sealed class PKCS7
{
	public class ContentInfo
	{
		private string contentType;

		private ASN1 content;

		public ASN1 ASN1 => GetASN1();

		public ASN1 Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
			}
		}

		public string ContentType
		{
			get
			{
				return contentType;
			}
			set
			{
				contentType = value;
			}
		}

		public ContentInfo()
		{
			content = new ASN1(160);
		}

		public ContentInfo(string oid)
			: this()
		{
			contentType = oid;
		}

		public ContentInfo(byte[] data)
			: this(new ASN1(data))
		{
		}

		public ContentInfo(ASN1 asn1)
		{
			if (asn1.Tag != 48 || (asn1.Count < 1 && asn1.Count > 2))
			{
				throw new ArgumentException("Invalid ASN1");
			}
			if (asn1[0].Tag != 6)
			{
				throw new ArgumentException("Invalid contentType");
			}
			contentType = ASN1Convert.ToOid(asn1[0]);
			if (asn1.Count > 1)
			{
				if (asn1[1].Tag != 160)
				{
					throw new ArgumentException("Invalid content");
				}
				content = asn1[1];
			}
		}

		internal ASN1 GetASN1()
		{
			ASN1 aSN = new ASN1(48);
			aSN.Add(ASN1Convert.FromOid(contentType));
			if (content != null && content.Count > 0)
			{
				aSN.Add(content);
			}
			return aSN;
		}
	}

	public class EncryptedData
	{
		private byte _version;

		private ContentInfo _content;

		private ContentInfo _encryptionAlgorithm;

		private byte[] _encrypted;

		public ContentInfo EncryptionAlgorithm => _encryptionAlgorithm;

		public byte[] EncryptedContent
		{
			get
			{
				if (_encrypted == null)
				{
					return null;
				}
				return (byte[])_encrypted.Clone();
			}
		}

		public EncryptedData()
		{
			_version = 0;
		}

		public EncryptedData(ASN1 asn1)
			: this()
		{
			if (asn1.Tag != 48 || asn1.Count < 2)
			{
				throw new ArgumentException("Invalid EncryptedData");
			}
			if (asn1[0].Tag != 2)
			{
				throw new ArgumentException("Invalid version");
			}
			_version = asn1[0].Value[0];
			ASN1 aSN = asn1[1];
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("missing EncryptedContentInfo");
			}
			ASN1 aSN2 = aSN[0];
			if (aSN2.Tag != 6)
			{
				throw new ArgumentException("missing EncryptedContentInfo.ContentType");
			}
			_content = new ContentInfo(ASN1Convert.ToOid(aSN2));
			ASN1 aSN3 = aSN[1];
			if (aSN3.Tag != 48)
			{
				throw new ArgumentException("missing EncryptedContentInfo.ContentEncryptionAlgorithmIdentifier");
			}
			_encryptionAlgorithm = new ContentInfo(ASN1Convert.ToOid(aSN3[0]));
			_encryptionAlgorithm.Content = aSN3[1];
			ASN1 aSN4 = aSN[2];
			if (aSN4.Tag != 128)
			{
				throw new ArgumentException("missing EncryptedContentInfo.EncryptedContent");
			}
			_encrypted = aSN4.Value;
		}
	}
}
