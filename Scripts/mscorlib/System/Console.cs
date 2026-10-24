using System.IO;
using System.Text;

namespace System;

public static class Console
{
	internal static TextWriter stdout;

	private static TextWriter stderr;

	private static TextReader stdin;

	private static Encoding inputEncoding;

	private static Encoding outputEncoding;

	public static TextWriter Error => stderr;

	public static TextWriter Out => stdout;

	static Console()
	{
		if (Environment.IsRunningOnWindows)
		{
			inputEncoding = (outputEncoding = Encoding.Default);
		}
		else
		{
			int code_page = 0;
			Encoding.InternalCodePage(ref code_page);
			if (code_page != -1 && ((code_page & 0xFFFFFFF) == 3 || (code_page & 0x10000000) != 0))
			{
				inputEncoding = (outputEncoding = Encoding.UTF8Unmarked);
			}
			else
			{
				inputEncoding = (outputEncoding = Encoding.Default);
			}
		}
		SetEncodings(inputEncoding, outputEncoding);
	}

	private static void SetEncodings(Encoding inputEncoding, Encoding outputEncoding)
	{
		stderr = new UnexceptionalStreamWriter(OpenStandardError(0), outputEncoding);
		((StreamWriter)stderr).AutoFlush = true;
		stderr = TextWriter.Synchronized(stderr, neverClose: true);
		stdout = new UnexceptionalStreamWriter(OpenStandardOutput(0), outputEncoding);
		((StreamWriter)stdout).AutoFlush = true;
		stdout = TextWriter.Synchronized(stdout, neverClose: true);
		stdin = new UnexceptionalStreamReader(OpenStandardInput(0), inputEncoding);
		stdin = TextReader.Synchronized(stdin);
		GC.SuppressFinalize(stdout);
		GC.SuppressFinalize(stderr);
		GC.SuppressFinalize(stdin);
	}

	private static Stream Open(IntPtr handle, FileAccess access, int bufferSize)
	{
		try
		{
			return new FileStream(handle, access, ownsHandle: false, bufferSize, isAsync: false, bufferSize == 0);
		}
		catch (IOException)
		{
			return new NullStream();
		}
	}

	public static Stream OpenStandardError(int bufferSize)
	{
		return Open(MonoIO.ConsoleError, FileAccess.Write, bufferSize);
	}

	public static Stream OpenStandardInput(int bufferSize)
	{
		return Open(MonoIO.ConsoleInput, FileAccess.Read, bufferSize);
	}

	public static Stream OpenStandardOutput(int bufferSize)
	{
		return Open(MonoIO.ConsoleOutput, FileAccess.Write, bufferSize);
	}

	public static void SetOut(TextWriter newOut)
	{
		if (newOut == null)
		{
			throw new ArgumentNullException("newOut");
		}
		stdout = newOut;
	}

	public static void WriteLine(string value)
	{
		stdout.WriteLine(value);
	}
}
