namespace System.Text;

[Serializable]
public sealed class EncoderReplacementFallback : EncoderFallback
{
	private string replacement;

	public string DefaultString => replacement;

	public EncoderReplacementFallback()
		: this("?")
	{
	}

	[MonoTODO]
	public EncoderReplacementFallback(string replacement)
	{
		if (replacement == null)
		{
			throw new ArgumentNullException();
		}
		this.replacement = replacement;
	}

	public override EncoderFallbackBuffer CreateFallbackBuffer()
	{
		return new EncoderReplacementFallbackBuffer(this);
	}

	public override bool Equals(object value)
	{
		return value is EncoderReplacementFallback encoderReplacementFallback && replacement == encoderReplacementFallback.replacement;
	}

	public override int GetHashCode()
	{
		return replacement.GetHashCode();
	}
}
