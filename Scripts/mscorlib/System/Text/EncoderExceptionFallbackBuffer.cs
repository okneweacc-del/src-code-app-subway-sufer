namespace System.Text;

public sealed class EncoderExceptionFallbackBuffer : EncoderFallbackBuffer
{
	public override int Remaining => 0;

	public override bool Fallback(char charUnknown, int index)
	{
		throw new EncoderFallbackException(charUnknown, index);
	}

	public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
	{
		throw new EncoderFallbackException(charUnknownHigh, charUnknownLow, index);
	}

	public override char GetNextChar()
	{
		return '\0';
	}
}
