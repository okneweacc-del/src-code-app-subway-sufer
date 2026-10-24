namespace System.Security.Cryptography;

public sealed class KeySizes
{
	private int _maxSize;

	private int _minSize;

	private int _skipSize;

	public int MaxSize => _maxSize;

	public int MinSize => _minSize;

	public int SkipSize => _skipSize;

	public KeySizes(int minSize, int maxSize, int skipSize)
	{
		_maxSize = maxSize;
		_minSize = minSize;
		_skipSize = skipSize;
	}

	internal bool IsLegal(int keySize)
	{
		int num = keySize - MinSize;
		bool flag = num >= 0 && keySize <= MaxSize;
		return (SkipSize == 0) ? flag : (flag && num % SkipSize == 0);
	}

	internal static bool IsLegalKeySize(KeySizes[] legalKeys, int size)
	{
		foreach (KeySizes keySizes in legalKeys)
		{
			if (keySizes.IsLegal(size))
			{
				return true;
			}
		}
		return false;
	}
}
