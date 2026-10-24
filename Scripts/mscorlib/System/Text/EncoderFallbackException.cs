namespace System.Text;

[Serializable]
public sealed class EncoderFallbackException : ArgumentException
{
	private char char_unknown;

	private char char_unknown_high;

	private char char_unknown_low;

	private int index = -1;

	public EncoderFallbackException()
		: this(null)
	{
	}

	public EncoderFallbackException(string message)
		: base(message)
	{
	}

	internal EncoderFallbackException(char charUnknown, int index)
		: base(null)
	{
		char_unknown = charUnknown;
		this.index = index;
	}

	internal EncoderFallbackException(char charUnknownHigh, char charUnknownLow, int index)
		: base(null)
	{
		char_unknown_high = charUnknownHigh;
		char_unknown_low = charUnknownLow;
		this.index = index;
	}
}
