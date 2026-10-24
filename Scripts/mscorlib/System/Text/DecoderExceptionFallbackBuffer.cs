namespace System.Text;

public sealed class DecoderExceptionFallbackBuffer : DecoderFallbackBuffer
{
	public override int Remaining => 0;

	public override bool Fallback(byte[] bytesUnknown, int index)
	{
		throw new DecoderFallbackException(null, bytesUnknown, index);
	}

	public override char GetNextChar()
	{
		return '\0';
	}
}
