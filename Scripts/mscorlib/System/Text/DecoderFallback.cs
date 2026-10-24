namespace System.Text;

[Serializable]
public abstract class DecoderFallback
{
	private static DecoderFallback exception_fallback = new DecoderExceptionFallback();

	private static DecoderFallback replacement_fallback = new DecoderReplacementFallback();

	private static DecoderFallback standard_safe_fallback = new DecoderReplacementFallback("\ufffd");

	public static DecoderFallback ExceptionFallback => exception_fallback;

	public static DecoderFallback ReplacementFallback => replacement_fallback;

	internal static DecoderFallback StandardSafeFallback => standard_safe_fallback;

	public abstract DecoderFallbackBuffer CreateFallbackBuffer();
}
