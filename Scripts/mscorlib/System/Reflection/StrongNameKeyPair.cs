using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Mono.Security;
using Mono.Security.Cryptography;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
public class StrongNameKeyPair : ISerializable, IDeserializationCallback
{
	private byte[] _publicKey;

	private string _keyPairContainer;

	private bool _keyPairExported;

	private byte[] _keyPairArray;

	[NonSerialized]
	private RSA _rsa;

	protected StrongNameKeyPair(SerializationInfo info, StreamingContext context)
	{
		_publicKey = (byte[])info.GetValue("_publicKey", typeof(byte[]));
		_keyPairContainer = info.GetString("_keyPairContainer");
		_keyPairExported = info.GetBoolean("_keyPairExported");
		_keyPairArray = (byte[])info.GetValue("_keyPairArray", typeof(byte[]));
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("_publicKey", _publicKey, typeof(byte[]));
		info.AddValue("_keyPairContainer", _keyPairContainer);
		info.AddValue("_keyPairExported", _keyPairExported);
		info.AddValue("_keyPairArray", _keyPairArray, typeof(byte[]));
	}

	void IDeserializationCallback.OnDeserialization(object sender)
	{
	}

	private RSA GetRSA()
	{
		if (_rsa != null)
		{
			return _rsa;
		}
		if (_keyPairArray != null)
		{
			try
			{
				_rsa = CryptoConvert.FromCapiKeyBlob(_keyPairArray);
			}
			catch
			{
				_keyPairArray = null;
			}
		}
		return _rsa;
	}

	internal StrongName StrongName()
	{
		RSA rSA = GetRSA();
		if (rSA != null)
		{
			return new StrongName(rSA);
		}
		if (_publicKey != null)
		{
			return new StrongName(_publicKey);
		}
		return null;
	}
}
