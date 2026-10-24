using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.X509;

internal class PKCS12 : ICloneable
{
	public class DeriveBytes
	{
		private static byte[] keyDiversifier = new byte[64]
		{
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1
		};

		private static byte[] ivDiversifier = new byte[64]
		{
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2
		};

		private static byte[] macDiversifier = new byte[64]
		{
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3
		};

		private string _hashName;

		private int _iterations;

		private byte[] _password;

		private byte[] _salt;

		public string HashName
		{
			set
			{
				_hashName = value;
			}
		}

		public int IterationCount
		{
			set
			{
				_iterations = value;
			}
		}

		public byte[] Password
		{
			set
			{
				if (value == null)
				{
					_password = new byte[0];
				}
				else
				{
					_password = (byte[])value.Clone();
				}
			}
		}

		public byte[] Salt
		{
			set
			{
				if (value != null)
				{
					_salt = (byte[])value.Clone();
				}
				else
				{
					_salt = null;
				}
			}
		}

		private void Adjust(byte[] a, int aOff, byte[] b)
		{
			int num = (b[b.Length - 1] & 0xFF) + (a[aOff + b.Length - 1] & 0xFF) + 1;
			a[aOff + b.Length - 1] = (byte)num;
			num >>= 8;
			for (int num2 = b.Length - 2; num2 >= 0; num2--)
			{
				num += (b[num2] & 0xFF) + (a[aOff + num2] & 0xFF);
				a[aOff + num2] = (byte)num;
				num >>= 8;
			}
		}

		private byte[] Derive(byte[] diversifier, int n)
		{
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(_hashName);
			int num = hashAlgorithm.HashSize >> 3;
			int num2 = 64;
			byte[] array = new byte[n];
			byte[] array2;
			if (_salt != null && _salt.Length != 0)
			{
				array2 = new byte[num2 * ((_salt.Length + num2 - 1) / num2)];
				for (int i = 0; i != array2.Length; i++)
				{
					array2[i] = _salt[i % _salt.Length];
				}
			}
			else
			{
				array2 = new byte[0];
			}
			byte[] array3;
			if (_password != null && _password.Length != 0)
			{
				array3 = new byte[num2 * ((_password.Length + num2 - 1) / num2)];
				for (int j = 0; j != array3.Length; j++)
				{
					array3[j] = _password[j % _password.Length];
				}
			}
			else
			{
				array3 = new byte[0];
			}
			byte[] array4 = new byte[array2.Length + array3.Length];
			Buffer.BlockCopy(array2, 0, array4, 0, array2.Length);
			Buffer.BlockCopy(array3, 0, array4, array2.Length, array3.Length);
			byte[] array5 = new byte[num2];
			int num3 = (n + num - 1) / num;
			for (int k = 1; k <= num3; k++)
			{
				hashAlgorithm.TransformBlock(diversifier, 0, diversifier.Length, diversifier, 0);
				hashAlgorithm.TransformFinalBlock(array4, 0, array4.Length);
				byte[] array6 = hashAlgorithm.Hash;
				hashAlgorithm.Initialize();
				for (int l = 1; l != _iterations; l++)
				{
					array6 = hashAlgorithm.ComputeHash(array6, 0, array6.Length);
				}
				for (int m = 0; m != array5.Length; m++)
				{
					array5[m] = array6[m % array6.Length];
				}
				for (int num4 = 0; num4 != array4.Length / num2; num4++)
				{
					Adjust(array4, num4 * num2, array5);
				}
				if (k == num3)
				{
					Buffer.BlockCopy(array6, 0, array, (k - 1) * num, array.Length - (k - 1) * num);
				}
				else
				{
					Buffer.BlockCopy(array6, 0, array, (k - 1) * num, array6.Length);
				}
			}
			return array;
		}

		public byte[] DeriveKey(int size)
		{
			return Derive(keyDiversifier, size);
		}

		public byte[] DeriveIV(int size)
		{
			return Derive(ivDiversifier, size);
		}

		public byte[] DeriveMAC(int size)
		{
			return Derive(macDiversifier, size);
		}
	}

	private static int recommendedIterationCount = 2000;

	private byte[] _password;

	private ArrayList _keyBags;

	private ArrayList _secretBags;

	private X509CertificateCollection _certs;

	private bool _keyBagsChanged;

	private bool _secretBagsChanged;

	private bool _certsChanged;

	private int _iterations;

	private ArrayList _safeBags;

	private RandomNumberGenerator _rng;

	private static int password_max_length = int.MaxValue;

	public string Password
	{
		set
		{
			if (value != null)
			{
				if (value.Length > 0)
				{
					int num = value.Length;
					int num2 = 0;
					if (num < MaximumPasswordLength)
					{
						if (value[num - 1] != 0)
						{
							num2 = 1;
						}
					}
					else
					{
						num = MaximumPasswordLength;
					}
					_password = new byte[num + num2 << 1];
					Encoding.BigEndianUnicode.GetBytes(value, 0, num, _password, 0);
				}
				else
				{
					_password = new byte[2];
				}
			}
			else
			{
				_password = null;
			}
		}
	}

	public int IterationCount
	{
		get
		{
			return _iterations;
		}
		set
		{
			_iterations = value;
		}
	}

	public X509CertificateCollection Certificates
	{
		get
		{
			if (_certsChanged)
			{
				_certs.Clear();
				foreach (SafeBag safeBag in _safeBags)
				{
					if (safeBag.BagOID.Equals("1.2.840.113549.1.12.10.1.3"))
					{
						ASN1 aSN = safeBag.ASN1;
						ASN1 aSN2 = aSN[1];
						PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(aSN2.Value);
						_certs.Add(new X509Certificate(contentInfo.Content[0].Value));
					}
				}
				_certsChanged = false;
			}
			return _certs;
		}
	}

	internal RandomNumberGenerator RNG
	{
		get
		{
			if (_rng == null)
			{
				_rng = RandomNumberGenerator.Create();
			}
			return _rng;
		}
	}

	public static int MaximumPasswordLength => password_max_length;

	public PKCS12()
	{
		_iterations = recommendedIterationCount;
		_keyBags = new ArrayList();
		_secretBags = new ArrayList();
		_certs = new X509CertificateCollection();
		_keyBagsChanged = false;
		_secretBagsChanged = false;
		_certsChanged = false;
		_safeBags = new ArrayList();
	}

	public PKCS12(byte[] data)
		: this()
	{
		Password = null;
		Decode(data);
	}

	public PKCS12(byte[] data, string password)
		: this()
	{
		Password = password;
		Decode(data);
	}

	private void Decode(byte[] data)
	{
		ASN1 aSN = new ASN1(data);
		if (aSN.Tag != 48)
		{
			throw new ArgumentException("invalid data");
		}
		ASN1 aSN2 = aSN[0];
		if (aSN2.Tag != 2)
		{
			throw new ArgumentException("invalid PFX version");
		}
		PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(aSN[1]);
		if (contentInfo.ContentType != "1.2.840.113549.1.7.1")
		{
			throw new ArgumentException("invalid authenticated safe");
		}
		if (aSN.Count > 2)
		{
			ASN1 aSN3 = aSN[2];
			if (aSN3.Tag != 48)
			{
				throw new ArgumentException("invalid MAC");
			}
			ASN1 aSN4 = aSN3[0];
			if (aSN4.Tag != 48)
			{
				throw new ArgumentException("invalid MAC");
			}
			ASN1 aSN5 = aSN4[0];
			string text = ASN1Convert.ToOid(aSN5[0]);
			if (text != "1.3.14.3.2.26")
			{
				throw new ArgumentException("unsupported HMAC");
			}
			byte[] value = aSN4[1].Value;
			ASN1 aSN6 = aSN3[1];
			if (aSN6.Tag != 4)
			{
				throw new ArgumentException("missing MAC salt");
			}
			_iterations = 1;
			if (aSN3.Count > 2)
			{
				ASN1 aSN7 = aSN3[2];
				if (aSN7.Tag != 2)
				{
					throw new ArgumentException("invalid MAC iteration");
				}
				_iterations = ASN1Convert.ToInt32(aSN7);
			}
			byte[] value2 = contentInfo.Content[0].Value;
			byte[] actual = MAC(_password, aSN6.Value, _iterations, value2);
			if (!Compare(value, actual))
			{
				throw new CryptographicException("Invalid MAC - file may have been tampered!");
			}
		}
		ASN1 aSN8 = new ASN1(contentInfo.Content[0].Value);
		for (int i = 0; i < aSN8.Count; i++)
		{
			PKCS7.ContentInfo contentInfo2 = new PKCS7.ContentInfo(aSN8[i]);
			switch (contentInfo2.ContentType)
			{
			case "1.2.840.113549.1.7.1":
			{
				ASN1 aSN10 = new ASN1(contentInfo2.Content[0].Value);
				for (int k = 0; k < aSN10.Count; k++)
				{
					ASN1 safeBag2 = aSN10[k];
					ReadSafeBag(safeBag2);
				}
				break;
			}
			case "1.2.840.113549.1.7.6":
			{
				PKCS7.EncryptedData ed = new PKCS7.EncryptedData(contentInfo2.Content[0]);
				ASN1 aSN9 = new ASN1(Decrypt(ed));
				for (int j = 0; j < aSN9.Count; j++)
				{
					ASN1 safeBag = aSN9[j];
					ReadSafeBag(safeBag);
				}
				break;
			}
			case "1.2.840.113549.1.7.3":
				throw new NotImplementedException("public key encrypted");
			default:
				throw new ArgumentException("unknown authenticatedSafe");
			}
		}
	}

	~PKCS12()
	{
		if (_password != null)
		{
			Array.Clear(_password, 0, _password.Length);
		}
		_password = null;
	}

	private bool Compare(byte[] expected, byte[] actual)
	{
		bool result = false;
		if (expected.Length == actual.Length)
		{
			for (int i = 0; i < expected.Length; i++)
			{
				if (expected[i] != actual[i])
				{
					return false;
				}
			}
			result = true;
		}
		return result;
	}

	private SymmetricAlgorithm GetSymmetricAlgorithm(string algorithmOid, byte[] salt, int iterationCount)
	{
		string text = null;
		int size = 8;
		int num = 8;
		DeriveBytes deriveBytes = new DeriveBytes();
		deriveBytes.Password = _password;
		deriveBytes.Salt = salt;
		deriveBytes.IterationCount = iterationCount;
		switch (algorithmOid)
		{
		case "1.2.840.113549.1.5.1":
			deriveBytes.HashName = "MD2";
			text = "DES";
			break;
		case "1.2.840.113549.1.5.3":
			deriveBytes.HashName = "MD5";
			text = "DES";
			break;
		case "1.2.840.113549.1.5.4":
			deriveBytes.HashName = "MD2";
			text = "RC2";
			size = 4;
			break;
		case "1.2.840.113549.1.5.6":
			deriveBytes.HashName = "MD5";
			text = "RC2";
			size = 4;
			break;
		case "1.2.840.113549.1.5.10":
			deriveBytes.HashName = "SHA1";
			text = "DES";
			break;
		case "1.2.840.113549.1.5.11":
			deriveBytes.HashName = "SHA1";
			text = "RC2";
			size = 4;
			break;
		case "1.2.840.113549.1.12.1.1":
			deriveBytes.HashName = "SHA1";
			text = "RC4";
			size = 16;
			num = 0;
			break;
		case "1.2.840.113549.1.12.1.2":
			deriveBytes.HashName = "SHA1";
			text = "RC4";
			size = 5;
			num = 0;
			break;
		case "1.2.840.113549.1.12.1.3":
			deriveBytes.HashName = "SHA1";
			text = "TripleDES";
			size = 24;
			break;
		case "1.2.840.113549.1.12.1.4":
			deriveBytes.HashName = "SHA1";
			text = "TripleDES";
			size = 16;
			break;
		case "1.2.840.113549.1.12.1.5":
			deriveBytes.HashName = "SHA1";
			text = "RC2";
			size = 16;
			break;
		case "1.2.840.113549.1.12.1.6":
			deriveBytes.HashName = "SHA1";
			text = "RC2";
			size = 5;
			break;
		default:
			throw new NotSupportedException("unknown oid " + text);
		}
		SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create(text);
		symmetricAlgorithm.Key = deriveBytes.DeriveKey(size);
		if (num > 0)
		{
			symmetricAlgorithm.IV = deriveBytes.DeriveIV(num);
			symmetricAlgorithm.Mode = CipherMode.CBC;
		}
		return symmetricAlgorithm;
	}

	public byte[] Decrypt(string algorithmOid, byte[] salt, int iterationCount, byte[] encryptedData)
	{
		SymmetricAlgorithm symmetricAlgorithm = null;
		byte[] array = null;
		try
		{
			symmetricAlgorithm = GetSymmetricAlgorithm(algorithmOid, salt, iterationCount);
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
			return cryptoTransform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
		}
		finally
		{
			symmetricAlgorithm?.Clear();
		}
	}

	public byte[] Decrypt(PKCS7.EncryptedData ed)
	{
		return Decrypt(ed.EncryptionAlgorithm.ContentType, ed.EncryptionAlgorithm.Content[0].Value, ASN1Convert.ToInt32(ed.EncryptionAlgorithm.Content[1]), ed.EncryptedContent);
	}

	public byte[] Encrypt(string algorithmOid, byte[] salt, int iterationCount, byte[] data)
	{
		byte[] array = null;
		using SymmetricAlgorithm symmetricAlgorithm = GetSymmetricAlgorithm(algorithmOid, salt, iterationCount);
		ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
		return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
	}

	private DSAParameters GetExistingParameters(out bool found)
	{
		foreach (X509Certificate certificate in Certificates)
		{
			if (certificate.KeyAlgorithmParameters != null)
			{
				DSA dSA = certificate.DSA;
				if (dSA != null)
				{
					found = true;
					return dSA.ExportParameters(includePrivateParameters: false);
				}
			}
		}
		found = false;
		return default;
	}

	private void AddPrivateKey(PKCS8.PrivateKeyInfo pki)
	{
		byte[] privateKey = pki.PrivateKey;
		switch (privateKey[0])
		{
		case 2:
		{
			DSAParameters existingParameters = GetExistingParameters(out var found);
			if (found)
			{
				_keyBags.Add(PKCS8.PrivateKeyInfo.DecodeDSA(privateKey, existingParameters));
			}
			break;
		}
		case 48:
			_keyBags.Add(PKCS8.PrivateKeyInfo.DecodeRSA(privateKey));
			break;
		default:
			Array.Clear(privateKey, 0, privateKey.Length);
			throw new CryptographicException("Unknown private key format");
		}
		Array.Clear(privateKey, 0, privateKey.Length);
	}

	private void ReadSafeBag(ASN1 safeBag)
	{
		if (safeBag.Tag != 48)
		{
			throw new ArgumentException("invalid safeBag");
		}
		ASN1 aSN = safeBag[0];
		if (aSN.Tag != 6)
		{
			throw new ArgumentException("invalid safeBag id");
		}
		ASN1 aSN2 = safeBag[1];
		string text = ASN1Convert.ToOid(aSN);
		switch (text)
		{
		case "1.2.840.113549.1.12.10.1.1":
			AddPrivateKey(new PKCS8.PrivateKeyInfo(aSN2.Value));
			break;
		case "1.2.840.113549.1.12.10.1.2":
		{
			PKCS8.EncryptedPrivateKeyInfo encryptedPrivateKeyInfo = new PKCS8.EncryptedPrivateKeyInfo(aSN2.Value);
			byte[] array = Decrypt(encryptedPrivateKeyInfo.Algorithm, encryptedPrivateKeyInfo.Salt, encryptedPrivateKeyInfo.IterationCount, encryptedPrivateKeyInfo.EncryptedData);
			AddPrivateKey(new PKCS8.PrivateKeyInfo(array));
			Array.Clear(array, 0, array.Length);
			break;
		}
		case "1.2.840.113549.1.12.10.1.3":
		{
			PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(aSN2.Value);
			if (contentInfo.ContentType != "1.2.840.113549.1.9.22.1")
			{
				throw new NotSupportedException("unsupport certificate type");
			}
			X509Certificate value2 = new X509Certificate(contentInfo.Content[0].Value);
			_certs.Add(value2);
			break;
		}
		case "1.2.840.113549.1.12.10.1.5":
		{
			byte[] value = aSN2.Value;
			_secretBags.Add(value);
			break;
		}
		default:
			throw new ArgumentException("unknown safeBag oid");
		case "1.2.840.113549.1.12.10.1.4":
		case "1.2.840.113549.1.12.10.1.6":
			break;
		}
		if (safeBag.Count > 2)
		{
			ASN1 aSN3 = safeBag[2];
			if (aSN3.Tag != 49)
			{
				throw new ArgumentException("invalid safeBag attributes id");
			}
			for (int i = 0; i < aSN3.Count; i++)
			{
				ASN1 aSN4 = aSN3[i];
				if (aSN4.Tag != 48)
				{
					throw new ArgumentException("invalid PKCS12 attributes id");
				}
				ASN1 aSN5 = aSN4[0];
				if (aSN5.Tag != 6)
				{
					throw new ArgumentException("invalid attribute id");
				}
				string text2 = ASN1Convert.ToOid(aSN5);
				ASN1 aSN6 = aSN4[1];
				for (int j = 0; j < aSN6.Count; j++)
				{
					ASN1 aSN7 = aSN6[j];
					switch (text2)
					{
					case "1.2.840.113549.1.9.20":
						if (aSN7.Tag != 30)
						{
							throw new ArgumentException("invalid attribute value id");
						}
						break;
					case "1.2.840.113549.1.9.21":
						if (aSN7.Tag != 4)
						{
							throw new ArgumentException("invalid attribute value id");
						}
						break;
					}
				}
			}
		}
		_safeBags.Add(new SafeBag(text, safeBag));
	}

	private ASN1 CertificateSafeBag(X509Certificate x509, IDictionary attributes)
	{
		ASN1 asn = new ASN1(4, x509.RawData);
		PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo();
		contentInfo.ContentType = "1.2.840.113549.1.9.22.1";
		contentInfo.Content.Add(asn);
		ASN1 aSN = new ASN1(160);
		aSN.Add(contentInfo.ASN1);
		ASN1 aSN2 = new ASN1(48);
		aSN2.Add(ASN1Convert.FromOid("1.2.840.113549.1.12.10.1.3"));
		aSN2.Add(aSN);
		if (attributes != null)
		{
			ASN1 aSN3 = new ASN1(49);
			IDictionaryEnumerator enumerator = attributes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				switch ((string)enumerator.Key)
				{
				case "1.2.840.113549.1.9.20":
				{
					ArrayList arrayList2 = (ArrayList)enumerator.Value;
					if (arrayList2.Count <= 0)
					{
						break;
					}
					ASN1 aSN7 = new ASN1(48);
					aSN7.Add(ASN1Convert.FromOid("1.2.840.113549.1.9.20"));
					ASN1 aSN8 = new ASN1(49);
					foreach (byte[] item in arrayList2)
					{
						ASN1 aSN9 = new ASN1(30);
						aSN9.Value = item;
						aSN8.Add(aSN9);
					}
					aSN7.Add(aSN8);
					aSN3.Add(aSN7);
					break;
				}
				case "1.2.840.113549.1.9.21":
				{
					ArrayList arrayList = (ArrayList)enumerator.Value;
					if (arrayList.Count <= 0)
					{
						break;
					}
					ASN1 aSN4 = new ASN1(48);
					aSN4.Add(ASN1Convert.FromOid("1.2.840.113549.1.9.21"));
					ASN1 aSN5 = new ASN1(49);
					foreach (byte[] item2 in arrayList)
					{
						ASN1 aSN6 = new ASN1(4);
						aSN6.Value = item2;
						aSN5.Add(aSN6);
					}
					aSN4.Add(aSN5);
					aSN3.Add(aSN4);
					break;
				}
				}
			}
			if (aSN3.Count > 0)
			{
				aSN2.Add(aSN3);
			}
		}
		return aSN2;
	}

	private byte[] MAC(byte[] password, byte[] salt, int iterations, byte[] data)
	{
		DeriveBytes deriveBytes = new DeriveBytes();
		deriveBytes.HashName = "SHA1";
		deriveBytes.Password = password;
		deriveBytes.Salt = salt;
		deriveBytes.IterationCount = iterations;
		HMACSHA1 hMACSHA = (HMACSHA1)HMAC.Create();
		hMACSHA.Key = deriveBytes.DeriveMAC(20);
		return hMACSHA.ComputeHash(data, 0, data.Length);
	}

	public byte[] GetBytes()
	{
		ASN1 aSN = new ASN1(48);
		ArrayList arrayList = new ArrayList();
		foreach (SafeBag safeBag5 in _safeBags)
		{
			if (safeBag5.BagOID.Equals("1.2.840.113549.1.12.10.1.3"))
			{
				ASN1 aSN2 = safeBag5.ASN1;
				ASN1 aSN3 = aSN2[1];
				PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(aSN3.Value);
				arrayList.Add(new X509Certificate(contentInfo.Content[0].Value));
			}
		}
		ArrayList arrayList2 = new ArrayList();
		ArrayList arrayList3 = new ArrayList();
		foreach (X509Certificate certificate in Certificates)
		{
			bool flag = false;
			foreach (X509Certificate item in arrayList)
			{
				if (Compare(certificate.RawData, item.RawData))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				arrayList2.Add(certificate);
			}
		}
		foreach (X509Certificate item2 in arrayList)
		{
			bool flag2 = false;
			foreach (X509Certificate certificate2 in Certificates)
			{
				if (Compare(item2.RawData, certificate2.RawData))
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				arrayList3.Add(item2);
			}
		}
		foreach (X509Certificate item3 in arrayList3)
		{
			RemoveCertificate(item3);
		}
		foreach (X509Certificate item4 in arrayList2)
		{
			AddCertificate(item4);
		}
		if (_safeBags.Count > 0)
		{
			ASN1 aSN4 = new ASN1(48);
			foreach (SafeBag safeBag6 in _safeBags)
			{
				if (safeBag6.BagOID.Equals("1.2.840.113549.1.12.10.1.3"))
				{
					aSN4.Add(safeBag6.ASN1);
				}
			}
			if (aSN4.Count > 0)
			{
				PKCS7.ContentInfo contentInfo2 = EncryptedContentInfo(aSN4, "1.2.840.113549.1.12.1.3");
				aSN.Add(contentInfo2.ASN1);
			}
		}
		if (_safeBags.Count > 0)
		{
			ASN1 aSN5 = new ASN1(48);
			foreach (SafeBag safeBag7 in _safeBags)
			{
				if (safeBag7.BagOID.Equals("1.2.840.113549.1.12.10.1.1") || safeBag7.BagOID.Equals("1.2.840.113549.1.12.10.1.2"))
				{
					aSN5.Add(safeBag7.ASN1);
				}
			}
			if (aSN5.Count > 0)
			{
				ASN1 aSN6 = new ASN1(160);
				aSN6.Add(new ASN1(4, aSN5.GetBytes()));
				PKCS7.ContentInfo contentInfo3 = new PKCS7.ContentInfo("1.2.840.113549.1.7.1");
				contentInfo3.Content = aSN6;
				aSN.Add(contentInfo3.ASN1);
			}
		}
		if (_safeBags.Count > 0)
		{
			ASN1 aSN7 = new ASN1(48);
			foreach (SafeBag safeBag8 in _safeBags)
			{
				if (safeBag8.BagOID.Equals("1.2.840.113549.1.12.10.1.5"))
				{
					aSN7.Add(safeBag8.ASN1);
				}
			}
			if (aSN7.Count > 0)
			{
				PKCS7.ContentInfo contentInfo4 = EncryptedContentInfo(aSN7, "1.2.840.113549.1.12.1.3");
				aSN.Add(contentInfo4.ASN1);
			}
		}
		ASN1 asn = new ASN1(4, aSN.GetBytes());
		ASN1 aSN8 = new ASN1(160);
		aSN8.Add(asn);
		PKCS7.ContentInfo contentInfo5 = new PKCS7.ContentInfo("1.2.840.113549.1.7.1");
		contentInfo5.Content = aSN8;
		ASN1 aSN9 = new ASN1(48);
		if (_password != null)
		{
			byte[] array = new byte[20];
			RNG.GetBytes(array);
			byte[] data = MAC(_password, array, _iterations, contentInfo5.Content[0].Value);
			ASN1 aSN10 = new ASN1(48);
			aSN10.Add(ASN1Convert.FromOid("1.3.14.3.2.26"));
			aSN10.Add(new ASN1(5));
			ASN1 aSN11 = new ASN1(48);
			aSN11.Add(aSN10);
			aSN11.Add(new ASN1(4, data));
			aSN9.Add(aSN11);
			aSN9.Add(new ASN1(4, array));
			aSN9.Add(ASN1Convert.FromInt32(_iterations));
		}
		ASN1 asn2 = new ASN1(2, new byte[1] { 3 });
		ASN1 aSN12 = new ASN1(48);
		aSN12.Add(asn2);
		aSN12.Add(contentInfo5.ASN1);
		if (aSN9.Count > 0)
		{
			aSN12.Add(aSN9);
		}
		return aSN12.GetBytes();
	}

	private PKCS7.ContentInfo EncryptedContentInfo(ASN1 safeBags, string algorithmOid)
	{
		byte[] array = new byte[8];
		RNG.GetBytes(array);
		ASN1 aSN = new ASN1(48);
		aSN.Add(new ASN1(4, array));
		aSN.Add(ASN1Convert.FromInt32(_iterations));
		ASN1 aSN2 = new ASN1(48);
		aSN2.Add(ASN1Convert.FromOid(algorithmOid));
		aSN2.Add(aSN);
		byte[] data = Encrypt(algorithmOid, array, _iterations, safeBags.GetBytes());
		ASN1 asn = new ASN1(128, data);
		ASN1 aSN3 = new ASN1(48);
		aSN3.Add(ASN1Convert.FromOid("1.2.840.113549.1.7.1"));
		aSN3.Add(aSN2);
		aSN3.Add(asn);
		ASN1 asn2 = new ASN1(2, new byte[1]);
		ASN1 aSN4 = new ASN1(48);
		aSN4.Add(asn2);
		aSN4.Add(aSN3);
		ASN1 aSN5 = new ASN1(160);
		aSN5.Add(aSN4);
		PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo("1.2.840.113549.1.7.6");
		contentInfo.Content = aSN5;
		return contentInfo;
	}

	public void AddCertificate(X509Certificate cert)
	{
		AddCertificate(cert, null);
	}

	public void AddCertificate(X509Certificate cert, IDictionary attributes)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < _safeBags.Count)
		{
			SafeBag safeBag = (SafeBag)_safeBags[num];
			if (safeBag.BagOID.Equals("1.2.840.113549.1.12.10.1.3"))
			{
				ASN1 aSN = safeBag.ASN1;
				ASN1 aSN2 = aSN[1];
				PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(aSN2.Value);
				X509Certificate x509Certificate = new X509Certificate(contentInfo.Content[0].Value);
				if (Compare(cert.RawData, x509Certificate.RawData))
				{
					flag = true;
				}
			}
			num++;
		}
		if (!flag)
		{
			_safeBags.Add(new SafeBag("1.2.840.113549.1.12.10.1.3", CertificateSafeBag(cert, attributes)));
			_certsChanged = true;
		}
	}

	public void RemoveCertificate(X509Certificate cert)
	{
		RemoveCertificate(cert, null);
	}

	public void RemoveCertificate(X509Certificate cert, IDictionary attrs)
	{
		int num = -1;
		int num2 = 0;
		while (num == -1 && num2 < _safeBags.Count)
		{
			SafeBag safeBag = (SafeBag)_safeBags[num2];
			if (safeBag.BagOID.Equals("1.2.840.113549.1.12.10.1.3"))
			{
				ASN1 aSN = safeBag.ASN1;
				ASN1 aSN2 = aSN[1];
				PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(aSN2.Value);
				X509Certificate x509Certificate = new X509Certificate(contentInfo.Content[0].Value);
				if (Compare(cert.RawData, x509Certificate.RawData))
				{
					if (attrs != null)
					{
						if (aSN.Count == 3)
						{
							ASN1 aSN3 = aSN[2];
							int num3 = 0;
							for (int i = 0; i < aSN3.Count; i++)
							{
								ASN1 aSN4 = aSN3[i];
								ASN1 asn = aSN4[0];
								string key = ASN1Convert.ToOid(asn);
								ArrayList arrayList = (ArrayList)attrs[key];
								if (arrayList == null)
								{
									continue;
								}
								ASN1 aSN5 = aSN4[1];
								if (arrayList.Count != aSN5.Count)
								{
									continue;
								}
								int num4 = 0;
								for (int j = 0; j < aSN5.Count; j++)
								{
									ASN1 aSN6 = aSN5[j];
									byte[] expected = (byte[])arrayList[j];
									if (Compare(expected, aSN6.Value))
									{
										num4++;
									}
								}
								if (num4 == aSN5.Count)
								{
									num3++;
								}
							}
							if (num3 == aSN3.Count)
							{
								num = num2;
							}
						}
					}
					else
					{
						num = num2;
					}
				}
			}
			num2++;
		}
		if (num != -1)
		{
			_safeBags.RemoveAt(num);
			_certsChanged = true;
		}
	}

	public object Clone()
	{
		PKCS12 pKCS = null;
		pKCS = ((_password == null) ? new PKCS12(GetBytes()) : new PKCS12(GetBytes(), Encoding.BigEndianUnicode.GetString(_password)));
		pKCS.IterationCount = IterationCount;
		return pKCS;
	}
}
