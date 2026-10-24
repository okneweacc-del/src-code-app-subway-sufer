namespace System.Security.Cryptography;

public abstract class Aes : SymmetricAlgorithm
{
	protected Aes()
	{
		KeySizeValue = 256;
		BlockSizeValue = 128;
		LegalKeySizesValue = new KeySizes[1];
		LegalKeySizesValue[0] = new KeySizes(128, 256, 64);
		LegalBlockSizesValue = new KeySizes[1];
		LegalBlockSizesValue[0] = new KeySizes(128, 128, 0);
	}
}
