using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Text;

[Serializable]
[ComVisible(true)]
public abstract class Encoding : ICloneable
{
	private sealed class ForwardingDecoder : Decoder
	{
		private Encoding encoding;

		public ForwardingDecoder(Encoding enc)
		{
			encoding = enc;
			DecoderFallback decoderFallback = encoding.DecoderFallback;
			if (decoderFallback != null)
			{
				base.Fallback = decoderFallback;
			}
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return encoding.GetCharCount(bytes, index, count);
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			return encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
		}
	}

	internal int codePage;

	internal int windows_code_page;

	private bool is_readonly = true;

	private DecoderFallback decoder_fallback;

	private EncoderFallback encoder_fallback;

	private static Assembly i18nAssembly;

	private static bool i18nDisabled;

	private static readonly object[] encodings = new object[43]
	{
		20127, "ascii", "us_ascii", "us", "ansi_x3.4_1968", "ansi_x3.4_1986", "cp367", "csascii", "ibm367", "iso_ir_6",
		"iso646_us", "iso_646.irv:1991", 65000, "utf_7", "csunicode11utf7", "unicode_1_1_utf_7", "unicode_2_0_utf_7", "x_unicode_1_1_utf_7", "x_unicode_2_0_utf_7", 65001,
		"utf_8", "unicode_1_1_utf_8", "unicode_2_0_utf_8", "x_unicode_1_1_utf_8", "x_unicode_2_0_utf_8", 1200, "utf_16", "UTF_16LE", "ucs_2", "unicode",
		"iso_10646_ucs2", 1201, "unicodefffe", "utf_16be", 12000, "utf_32", "UTF_32LE", "ucs_4", 12001, "UTF_32BE",
		28591, "iso_8859_1", "latin1"
	};

	internal string body_name;

	internal string encoding_name;

	internal string header_name;

	internal bool is_mail_news_display;

	internal bool is_mail_news_save;

	internal bool is_browser_save;

	internal bool is_browser_display;

	internal string web_name;

	private static volatile Encoding asciiEncoding;

	private static volatile Encoding bigEndianEncoding;

	private static volatile Encoding defaultEncoding;

	private static volatile Encoding utf7Encoding;

	private static volatile Encoding utf8EncodingWithMarkers;

	private static volatile Encoding utf8EncodingWithoutMarkers;

	private static volatile Encoding unicodeEncoding;

	private static volatile Encoding isoLatin1Encoding;

	private static volatile Encoding utf8EncodingUnsafe;

	private static volatile Encoding utf32Encoding;

	private static volatile Encoding bigEndianUTF32Encoding;

	private static readonly object lockobj = new object();

	[ComVisible(false)]
	public bool IsReadOnly => is_readonly;

	[ComVisible(false)]
	public DecoderFallback DecoderFallback
	{
		get
		{
			return decoder_fallback;
		}
		set
		{
			if (IsReadOnly)
			{
				throw new InvalidOperationException("This Encoding is readonly.");
			}
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			decoder_fallback = value;
		}
	}

	[ComVisible(false)]
	public EncoderFallback EncoderFallback => encoder_fallback;

	public virtual string HeaderName => header_name;

	public virtual string WebName => web_name;

	public static Encoding ASCII
	{
		get
		{
			if (asciiEncoding == null)
			{
				lock (lockobj)
				{
					if (asciiEncoding == null)
					{
						asciiEncoding = new ASCIIEncoding();
					}
				}
			}
			return asciiEncoding;
		}
	}

	public static Encoding BigEndianUnicode
	{
		get
		{
			if (bigEndianEncoding == null)
			{
				lock (lockobj)
				{
					if (bigEndianEncoding == null)
					{
						bigEndianEncoding = new UnicodeEncoding(bigEndian: true, byteOrderMark: true);
					}
				}
			}
			return bigEndianEncoding;
		}
	}

	public static Encoding Default
	{
		get
		{
			if (defaultEncoding == null)
			{
				lock (lockobj)
				{
					if (defaultEncoding == null)
					{
						int code_page = 1;
						string name = InternalCodePage(ref code_page);
						try
						{
							if (code_page == -1)
							{
								defaultEncoding = GetEncoding(name);
							}
							else
							{
								code_page &= 0xFFFFFFF;
								switch (code_page)
								{
								case 1:
									code_page = 20127;
									break;
								case 2:
									code_page = 65000;
									break;
								case 3:
									code_page = 65001;
									break;
								case 4:
									code_page = 1200;
									break;
								case 5:
									code_page = 1201;
									break;
								case 6:
									code_page = 28591;
									break;
								}
								defaultEncoding = GetEncoding(code_page);
							}
						}
						catch (NotSupportedException)
						{
							defaultEncoding = UTF8Unmarked;
						}
						catch (ArgumentException)
						{
							defaultEncoding = UTF8Unmarked;
						}
						defaultEncoding.is_readonly = true;
					}
				}
			}
			return defaultEncoding;
		}
	}

	private static Encoding ISOLatin1
	{
		get
		{
			if (isoLatin1Encoding == null)
			{
				lock (lockobj)
				{
					if (isoLatin1Encoding == null)
					{
						isoLatin1Encoding = new Latin1Encoding();
					}
				}
			}
			return isoLatin1Encoding;
		}
	}

	public static Encoding UTF7
	{
		get
		{
			if (utf7Encoding == null)
			{
				lock (lockobj)
				{
					if (utf7Encoding == null)
					{
						utf7Encoding = new UTF7Encoding();
					}
				}
			}
			return utf7Encoding;
		}
	}

	public static Encoding UTF8
	{
		get
		{
			if (utf8EncodingWithMarkers == null)
			{
				lock (lockobj)
				{
					if (utf8EncodingWithMarkers == null)
					{
						utf8EncodingWithMarkers = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
					}
				}
			}
			return utf8EncodingWithMarkers;
		}
	}

	internal static Encoding UTF8Unmarked
	{
		get
		{
			if (utf8EncodingWithoutMarkers == null)
			{
				lock (lockobj)
				{
					if (utf8EncodingWithoutMarkers == null)
					{
						utf8EncodingWithoutMarkers = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);
					}
				}
			}
			return utf8EncodingWithoutMarkers;
		}
	}

	internal static Encoding UTF8UnmarkedUnsafe
	{
		get
		{
			if (utf8EncodingUnsafe == null)
			{
				lock (lockobj)
				{
					if (utf8EncodingUnsafe == null)
					{
						utf8EncodingUnsafe = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);
						utf8EncodingUnsafe.is_readonly = false;
						utf8EncodingUnsafe.DecoderFallback = new DecoderReplacementFallback(string.Empty);
						utf8EncodingUnsafe.is_readonly = true;
					}
				}
			}
			return utf8EncodingUnsafe;
		}
	}

	public static Encoding Unicode
	{
		get
		{
			if (unicodeEncoding == null)
			{
				lock (lockobj)
				{
					if (unicodeEncoding == null)
					{
						unicodeEncoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
					}
				}
			}
			return unicodeEncoding;
		}
	}

	public static Encoding UTF32
	{
		get
		{
			if (utf32Encoding == null)
			{
				lock (lockobj)
				{
					if (utf32Encoding == null)
					{
						utf32Encoding = new UTF32Encoding(bigEndian: false, byteOrderMark: true);
					}
				}
			}
			return utf32Encoding;
		}
	}

	internal static Encoding BigEndianUTF32
	{
		get
		{
			if (bigEndianUTF32Encoding == null)
			{
				lock (lockobj)
				{
					if (bigEndianUTF32Encoding == null)
					{
						bigEndianUTF32Encoding = new UTF32Encoding(bigEndian: true, byteOrderMark: true);
					}
				}
			}
			return bigEndianUTF32Encoding;
		}
	}

	protected Encoding()
	{
	}

	protected Encoding(int codePage)
	{
		this.codePage = (windows_code_page = codePage);
		switch (codePage)
		{
		default:
			decoder_fallback = DecoderFallback.ReplacementFallback;
			encoder_fallback = EncoderFallback.ReplacementFallback;
			break;
		case 20127:
		case 54936:
			decoder_fallback = DecoderFallback.ReplacementFallback;
			encoder_fallback = EncoderFallback.ReplacementFallback;
			break;
		case 1200:
		case 1201:
		case 12000:
		case 12001:
		case 65000:
		case 65001:
			decoder_fallback = DecoderFallback.StandardSafeFallback;
			encoder_fallback = EncoderFallback.StandardSafeFallback;
			break;
		}
	}

	internal static string _(string arg)
	{
		return arg;
	}

	internal void SetFallbackInternal(EncoderFallback e, DecoderFallback d)
	{
		if (e != null)
		{
			encoder_fallback = e;
		}
		if (d != null)
		{
			decoder_fallback = d;
		}
	}

	public override bool Equals(object value)
	{
		if (value is Encoding encoding)
		{
			return codePage == encoding.codePage && DecoderFallback.Equals(encoding.DecoderFallback) && EncoderFallback.Equals(encoding.EncoderFallback);
		}
		return false;
	}

	public abstract int GetByteCount(char[] chars, int index, int count);

	public unsafe virtual int GetByteCount(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (s.Length == 0)
		{
			return 0;
		}
		fixed (char* chars = s)
		{
			return GetByteCount(chars, s.Length);
		}
	}

	public virtual int GetByteCount(char[] chars)
	{
		if (chars != null)
		{
			return GetByteCount(chars, 0, chars.Length);
		}
		throw new ArgumentNullException("chars");
	}

	public abstract int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex);

	public unsafe virtual int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
	{
		//IL_00c0->IL00c8: Incompatible stack types: I vs Ref
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (charIndex < 0 || charIndex > s.Length)
		{
			throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_Array"));
		}
		if (charCount < 0 || charIndex > s.Length - charCount)
		{
			throw new ArgumentOutOfRangeException("charCount", _("ArgRange_Array"));
		}
		if (byteIndex < 0 || byteIndex > bytes.Length)
		{
			throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
		}
		if (charCount == 0 || bytes.Length == byteIndex)
		{
			return 0;
		}
		fixed (char* ptr = s)
		{
			fixed (byte* ptr2 = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
			{
				return GetBytes((char*)((byte*)ptr + charIndex * 2), charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
			}
		}
	}

	public unsafe virtual byte[] GetBytes(string s)
	{
		//IL_0061->IL0068: Incompatible stack types: I vs Ref
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (s.Length == 0)
		{
			return new byte[0];
		}
		int byteCount = GetByteCount(s);
		if (byteCount == 0)
		{
			return new byte[0];
		}
		fixed (char* chars = s)
		{
			byte[] array = new byte[byteCount];
			fixed (byte* bytes = &(array != null && array.Length != 0 ? ref array[0] : ref *(byte*)null))
			{
				GetBytes(chars, s.Length, bytes, byteCount);
				return array;
			}
		}
	}

	public virtual byte[] GetBytes(char[] chars, int index, int count)
	{
		int byteCount = GetByteCount(chars, index, count);
		byte[] array = new byte[byteCount];
		GetBytes(chars, index, count, array, 0);
		return array;
	}

	public virtual byte[] GetBytes(char[] chars)
	{
		int byteCount = GetByteCount(chars, 0, chars.Length);
		byte[] array = new byte[byteCount];
		GetBytes(chars, 0, chars.Length, array, 0);
		return array;
	}

	public abstract int GetCharCount(byte[] bytes, int index, int count);

	public virtual int GetCharCount(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		return GetCharCount(bytes, 0, bytes.Length);
	}

	public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);

	public virtual char[] GetChars(byte[] bytes, int index, int count)
	{
		int charCount = GetCharCount(bytes, index, count);
		char[] array = new char[charCount];
		GetChars(bytes, index, count, array, 0);
		return array;
	}

	public virtual char[] GetChars(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		int charCount = GetCharCount(bytes, 0, bytes.Length);
		char[] array = new char[charCount];
		GetChars(bytes, 0, bytes.Length, array, 0);
		return array;
	}

	public virtual Decoder GetDecoder()
	{
		return new ForwardingDecoder(this);
	}

	private static object InvokeI18N(string name, params object[] args)
	{
		lock (lockobj)
		{
			if (i18nDisabled)
			{
				return null;
			}
			if (i18nAssembly == null)
			{
				try
				{
					try
					{
						i18nAssembly = Assembly.Load("I18N, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
					}
					catch (NotImplementedException)
					{
						i18nDisabled = true;
						return null;
					}
					if (i18nAssembly == null)
					{
						return null;
					}
				}
				catch (SystemException)
				{
					return null;
				}
			}
			Type type;
			try
			{
				type = i18nAssembly.GetType("I18N.Common.Manager");
			}
			catch (NotImplementedException)
			{
				i18nDisabled = true;
				return null;
			}
			if (type == null)
			{
				return null;
			}
			object obj;
			try
			{
				obj = type.InvokeMember("PrimaryManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, null, null, null, null);
				if (obj == null)
				{
					return null;
				}
			}
			catch (MissingMethodException)
			{
				return null;
			}
			catch (SecurityException)
			{
				return null;
			}
			catch (NotImplementedException)
			{
				i18nDisabled = true;
				return null;
			}
			try
			{
				return type.InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj, args, null, null, null);
			}
			catch (MissingMethodException)
			{
				return null;
			}
			catch (SecurityException)
			{
				return null;
			}
		}
	}

	public static Encoding GetEncoding(int codepage)
	{
		if (codepage < 0 || codepage > 65535)
		{
			throw new ArgumentOutOfRangeException("codepage", "Valid values are between 0 and 65535, inclusive.");
		}
		switch (codepage)
		{
		case 0:
			return Default;
		case 20127:
			return ASCII;
		case 65000:
			return UTF7;
		case 65001:
			return UTF8;
		case 12000:
			return UTF32;
		case 12001:
			return BigEndianUTF32;
		case 1200:
			return Unicode;
		case 1201:
			return BigEndianUnicode;
		case 28591:
			return ISOLatin1;
		default:
		{
			Encoding encoding = (Encoding)InvokeI18N("GetEncoding", codepage);
			if (encoding != null)
			{
				encoding.is_readonly = true;
				return encoding;
			}
			string text = "System.Text.CP" + codepage;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType(text);
			if (type != null)
			{
				encoding = (Encoding)Activator.CreateInstance(type);
				encoding.is_readonly = true;
				return encoding;
			}
			type = Type.GetType(text);
			if (type != null)
			{
				encoding = (Encoding)Activator.CreateInstance(type);
				encoding.is_readonly = true;
				return encoding;
			}
			throw new NotSupportedException($"CodePage {codepage.ToString()} not supported");
		}
		}
	}

	[ComVisible(false)]
	public virtual object Clone()
	{
		Encoding encoding = (Encoding)MemberwiseClone();
		encoding.is_readonly = false;
		return encoding;
	}

	public static Encoding GetEncoding(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		string text = name.ToLowerInvariant().Replace('-', '_');
		int codepage = 0;
		for (int i = 0; i < encodings.Length; i++)
		{
			object obj = encodings[i];
			if (obj is int)
			{
				codepage = (int)obj;
			}
			else if (text == (string)encodings[i])
			{
				return GetEncoding(codepage);
			}
		}
		Encoding encoding = (Encoding)InvokeI18N("GetEncoding", name);
		if (encoding != null)
		{
			return encoding;
		}
		string text2 = "System.Text.ENC" + text;
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Type type = executingAssembly.GetType(text2);
		if (type != null)
		{
			return (Encoding)Activator.CreateInstance(type);
		}
		type = Type.GetType(text2);
		if (type != null)
		{
			return (Encoding)Activator.CreateInstance(type);
		}
		throw new ArgumentException($"Encoding name '{name}' not supported", "name");
	}

	public override int GetHashCode()
	{
		return DecoderFallback.GetHashCode() << 24 + EncoderFallback.GetHashCode() << 16 + codePage;
	}

	public abstract int GetMaxByteCount(int charCount);

	public abstract int GetMaxCharCount(int byteCount);

	public virtual byte[] GetPreamble()
	{
		return new byte[0];
	}

	public virtual string GetString(byte[] bytes, int index, int count)
	{
		return new string(GetChars(bytes, index, count));
	}

	public virtual string GetString(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		return GetString(bytes, 0, bytes.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern string InternalCodePage(ref int code_page);

	[ComVisible(false)]
	[CLSCompliant(false)]
	public unsafe virtual int GetByteCount(char* chars, int count)
	{
		if (chars == null)
		{
			throw new ArgumentNullException("chars");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		char[] array = new char[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = *(char*)((byte*)chars + i * 2);
		}
		return GetByteCount(array);
	}

	[CLSCompliant(false)]
	[ComVisible(false)]
	public unsafe virtual int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (chars == null)
		{
			throw new ArgumentNullException("chars");
		}
		if (charCount < 0)
		{
			throw new ArgumentOutOfRangeException("charCount");
		}
		if (byteCount < 0)
		{
			throw new ArgumentOutOfRangeException("byteCount");
		}
		char[] array = new char[charCount];
		for (int i = 0; i < charCount; i++)
		{
			array[i] = *(char*)((byte*)chars + i * 2);
		}
		byte[] bytes2 = GetBytes(array, 0, charCount);
		int num = bytes2.Length;
		if (num > byteCount)
		{
			throw new ArgumentException("byteCount is less that the number of bytes produced", "byteCount");
		}
		for (int j = 0; j < num; j++)
		{
			bytes[j] = bytes2[j];
		}
		return bytes2.Length;
	}
}
