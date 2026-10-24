using System.Runtime.InteropServices;

namespace System.IO;

[Serializable]
[ComVisible(true)]
public class StringReader : TextReader
{
	private string source;

	private int nextChar;

	private int sourceLength;

	public StringReader(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		source = s;
		nextChar = 0;
		sourceLength = s.Length;
	}

	public override void Close()
	{
		Dispose(disposing: true);
	}

	protected override void Dispose(bool disposing)
	{
		source = null;
		base.Dispose(disposing);
	}

	public override int Peek()
	{
		CheckObjectDisposedException();
		if (nextChar >= sourceLength)
		{
			return -1;
		}
		return source[nextChar];
	}

	public override int Read()
	{
		CheckObjectDisposedException();
		if (nextChar >= sourceLength)
		{
			return -1;
		}
		return source[nextChar++];
	}

	public override int Read([In][Out] char[] buffer, int index, int count)
	{
		CheckObjectDisposedException();
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (buffer.Length - index < count)
		{
			throw new ArgumentException();
		}
		if (index < 0 || count < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		int num = ((nextChar <= sourceLength - count) ? count : (sourceLength - nextChar));
		source.CopyTo(nextChar, buffer, index, num);
		nextChar += num;
		return num;
	}

	public override string ReadLine()
	{
		CheckObjectDisposedException();
		int i;
		for (i = nextChar; i < sourceLength; i++)
		{
			char c = source[i];
			if (c == '\r' || c == '\n')
			{
				string result = source.Substring(nextChar, i - nextChar);
				nextChar = i + 1;
				if (c == '\r' && nextChar < sourceLength && source[nextChar] == '\n')
				{
					nextChar++;
				}
				return result;
			}
		}
		if (i > nextChar)
		{
			string result2 = source.Substring(nextChar, i - nextChar);
			nextChar = i;
			return result2;
		}
		return null;
	}

	public override string ReadToEnd()
	{
		CheckObjectDisposedException();
		string result = source.Substring(nextChar, sourceLength - nextChar);
		nextChar = sourceLength;
		return result;
	}

	private void CheckObjectDisposedException()
	{
		if (source == null)
		{
			throw new ObjectDisposedException("StringReader", Locale.GetText("Cannot read from a closed StringReader"));
		}
	}
}
