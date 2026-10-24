namespace System.Text;

[Serializable]
public sealed class DecoderFallbackException : ArgumentException
{
	private byte[] bytes_unknown;

	private int index = -1;

	public DecoderFallbackException()
		: this(null)
	{
	}

	public DecoderFallbackException(string message)
		: base(message)
	{
	}

	public DecoderFallbackException(string message, byte[] bytesUnknown, int index)
		: base(message)
	{
		bytes_unknown = bytesUnknown;
		this.index = index;
	}
}
