using System;
using System.Collections;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography;

internal sealed class PKCS8
{
	public class PrivateKeyInfo
	{
		private int _version;

		private string _algorithm;

		private byte[] _key;

		private ArrayList _list;

		public byte[] PrivateKey
		{
			get
			{
				if (_key == null)
				{
					return null;
				}
				return (byte[])_key.Clone();
			}
		}

		public PrivateKeyInfo()
		{
			_version = 0;
			_list = new ArrayList();
		}

		public PrivateKeyInfo(byte[] data)
			: this()
		{
			Decode(data);
		}

		private void Decode(byte[] data)
		{
			ASN1 aSN = new ASN1(data);
			if (aSN.Tag != 48)
			{
				throw new CryptographicException("invalid PrivateKeyInfo");
			}
			ASN1 aSN2 = aSN[0];
			if (aSN2.Tag != 2)
			{
				throw new CryptographicException("invalid version");
			}
			_version = aSN2.Value[0];
			ASN1 aSN3 = aSN[1];
			if (aSN3.Tag != 48)
			{
				throw new CryptographicException("invalid algorithm");
			}
			ASN1 aSN4 = aSN3[0];
			if (aSN4.Tag != 6)
			{
				throw new CryptographicException("missing algorithm OID");
			}
			_algorithm = ASN1Convert.ToOid(aSN4);
			ASN1 aSN5 = aSN[2];
			_key = aSN5.Value;
			if (aSN.Count > 3)
			{
				ASN1 aSN6 = aSN[3];
				for (int i = 0; i < aSN6.Count; i++)
				{
					_list.Add(aSN6[i]);
				}
			}
		}

		private static byte[] RemoveLeadingZero(byte[] bigInt)
		{
			int srcOffset = 0;
			int num = bigInt.Length;
			if (bigInt[0] == 0)
			{
				srcOffset = 1;
				num--;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(bigInt, srcOffset, array, 0, num);
			return array;
		}

		private static byte[] Normalize(byte[] bigInt, int length)
		{
			if (bigInt.Length == length)
			{
				return bigInt;
			}
			if (bigInt.Length > length)
			{
				return RemoveLeadingZero(bigInt);
			}
			byte[] array = new byte[length];
			Buffer.BlockCopy(bigInt, 0, array, length - bigInt.Length, bigInt.Length);
			return array;
		}

		public static RSA DecodeRSA(byte[] keypair)
		{
			ASN1 aSN = new ASN1(keypair);
			if (aSN.Tag != 48)
			{
				throw new CryptographicException("invalid private key format");
			}
			ASN1 aSN2 = aSN[0];
			if (aSN2.Tag != 2)
			{
				throw new CryptographicException("missing version");
			}
			if (aSN.Count < 9)
			{
				throw new CryptographicException("not enough key parameters");
			}
			RSAParameters parameters = new RSAParameters
			{
				Modulus = RemoveLeadingZero(aSN[1].Value)
			};
			int num = parameters.Modulus.Length;
			int length = num >> 1;
			parameters.D = Normalize(aSN[3].Value, num);
			parameters.DP = Normalize(aSN[6].Value, length);
			parameters.DQ = Normalize(aSN[7].Value, length);
			parameters.Exponent = RemoveLeadingZero(aSN[2].Value);
			parameters.InverseQ = Normalize(aSN[8].Value, length);
			parameters.P = Normalize(aSN[4].Value, length);
			parameters.Q = Normalize(aSN[5].Value, length);
			RSA rSA = null;
			try
			{
				rSA = RSA.Create();
				rSA.ImportParameters(parameters);
			}
			catch (CryptographicException)
			{
				CspParameters cspParameters = new CspParameters();
				cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
				rSA = new RSACryptoServiceProvider(cspParameters);
				rSA.ImportParameters(parameters);
			}
			return rSA;
		}

		public static DSA DecodeDSA(byte[] privateKey, DSAParameters dsaParameters)
		{
			ASN1 aSN = new ASN1(privateKey);
			if (aSN.Tag != 2)
			{
				throw new CryptographicException("invalid private key format");
			}
			dsaParameters.X = Normalize(aSN.Value, 20);
			DSA dSA = DSA.Create();
			dSA.ImportParameters(dsaParameters);
			return dSA;
		}
	}

	public class EncryptedPrivateKeyInfo
	{
		private string _algorithm;

		private byte[] _salt;

		private int _iterations;

		private byte[] _data;

		public string Algorithm => _algorithm;

		public byte[] EncryptedData => (_data != null) ? ((byte[])_data.Clone()) : null;

		public byte[] Salt
		{
			get
			{
				if (_salt == null)
				{
					RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
					_salt = new byte[8];
					randomNumberGenerator.GetBytes(_salt);
				}
				return (byte[])_salt.Clone();
			}
		}

		public int IterationCount => _iterations;

		public EncryptedPrivateKeyInfo()
		{
		}

		public EncryptedPrivateKeyInfo(byte[] data)
			: this()
		{
			Decode(data);
		}

		private void Decode(byte[] data)
		{
			ASN1 aSN = new ASN1(data);
			if (aSN.Tag != 48)
			{
				throw new CryptographicException("invalid EncryptedPrivateKeyInfo");
			}
			ASN1 aSN2 = aSN[0];
			if (aSN2.Tag != 48)
			{
				throw new CryptographicException("invalid encryptionAlgorithm");
			}
			ASN1 aSN3 = aSN2[0];
			if (aSN3.Tag != 6)
			{
				throw new CryptographicException("invalid algorithm");
			}
			_algorithm = ASN1Convert.ToOid(aSN3);
			if (aSN2.Count > 1)
			{
				ASN1 aSN4 = aSN2[1];
				if (aSN4.Tag != 48)
				{
					throw new CryptographicException("invalid parameters");
				}
				ASN1 aSN5 = aSN4[0];
				if (aSN5.Tag != 4)
				{
					throw new CryptographicException("invalid salt");
				}
				_salt = aSN5.Value;
				ASN1 aSN6 = aSN4[1];
				if (aSN6.Tag != 2)
				{
					throw new CryptographicException("invalid iterationCount");
				}
				_iterations = ASN1Convert.ToInt32(aSN6);
			}
			ASN1 aSN7 = aSN[1];
			if (aSN7.Tag != 4)
			{
				throw new CryptographicException("invalid EncryptedData");
			}
			_data = aSN7.Value;
		}
	}
}
