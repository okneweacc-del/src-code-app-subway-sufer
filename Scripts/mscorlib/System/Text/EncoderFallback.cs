namespace System.Text;

[Serializable]
public abstract class EncoderFallback
{
	private static EncoderFallback exception_fallback = new EncoderExceptionFallback();

	private static EncoderFallback replacement_fallback = new EncoderReplacementFallback();

	private static EncoderFallback standard_safe_fallback = new EncoderReplacementFallback("\ufffd");

	public static EncoderFallback ExceptionFallback => exception_fallback;

	public static EncoderFallback ReplacementFallback => replacement_fallback;

	internal static EncoderFallback StandardSafeFallback => standard_safe_fallback;

	public abstract EncoderFallbackBuffer CreateFallbackBuffer();
}
