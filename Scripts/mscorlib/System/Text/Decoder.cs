using System.Runtime.InteropServices;

namespace System.Text;

[Serializable]
[ComVisible(true)]
public abstract class Decoder
{
	private DecoderFallback fallback = new DecoderReplacementFallback();

	private DecoderFallbackBuffer fallback_buffer;

	[ComVisible(false)]
	public DecoderFallback Fallback
	{
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			fallback = value;
			fallback_buffer = null;
		}
	}

	[ComVisible(false)]
	public DecoderFallbackBuffer FallbackBuffer
	{
		get
		{
			if (fallback_buffer == null)
			{
				fallback_buffer = fallback.CreateFallbackBuffer();
			}
			return fallback_buffer;
		}
	}

	public abstract int GetCharCount(byte[] bytes, int index, int count);

	public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);
}
