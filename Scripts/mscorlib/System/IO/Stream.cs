using System.Runtime.InteropServices;

namespace System.IO;

[Serializable]
[ComVisible(true)]
public abstract class Stream : IDisposable
{
	public static readonly Stream Null = new NullStream();

	public abstract bool CanRead { get; }

	public abstract bool CanSeek { get; }

	public abstract bool CanWrite { get; }

	public abstract long Length { get; }

	public abstract long Position { get; set; }

	public void Dispose()
	{
		Close();
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	public virtual void Close()
	{
		Dispose(disposing: true);
	}

	public abstract void Flush();

	public abstract int Read([In][Out] byte[] buffer, int offset, int count);

	public virtual int ReadByte()
	{
		byte[] array = new byte[1];
		if (Read(array, 0, 1) == 1)
		{
			return array[0];
		}
		return -1;
	}

	public abstract long Seek(long offset, SeekOrigin origin);

	public abstract void SetLength(long value);

	public abstract void Write(byte[] buffer, int offset, int count);

	public virtual void WriteByte(byte value)
	{
		Write(new byte[1] { value }, 0, 1);
	}

	public virtual IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		if (!CanRead)
		{
			throw new NotSupportedException("This stream does not support reading");
		}
		StreamAsyncResult streamAsyncResult = new StreamAsyncResult(state);
		try
		{
			int nbytes = Read(buffer, offset, count);
			streamAsyncResult.SetComplete(null, nbytes);
		}
		catch (Exception e)
		{
			streamAsyncResult.SetComplete(e, 0);
		}
		callback?.Invoke(streamAsyncResult);
		return streamAsyncResult;
	}

	public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		if (!CanWrite)
		{
			throw new NotSupportedException("This stream does not support writing");
		}
		StreamAsyncResult streamAsyncResult = new StreamAsyncResult(state);
		try
		{
			Write(buffer, offset, count);
			streamAsyncResult.SetComplete(null);
		}
		catch (Exception complete)
		{
			streamAsyncResult.SetComplete(complete);
		}
		callback?.BeginInvoke(streamAsyncResult, null, null);
		return streamAsyncResult;
	}

	public virtual int EndRead(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		if (!(asyncResult is StreamAsyncResult { NBytes: not -1 } streamAsyncResult))
		{
			throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
		}
		if (streamAsyncResult.Done)
		{
			throw new InvalidOperationException("EndRead already called.");
		}
		streamAsyncResult.Done = true;
		if (streamAsyncResult.Exception != null)
		{
			throw streamAsyncResult.Exception;
		}
		return streamAsyncResult.NBytes;
	}

	public virtual void EndWrite(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		if (!(asyncResult is StreamAsyncResult { NBytes: -1 } streamAsyncResult))
		{
			throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
		}
		if (streamAsyncResult.Done)
		{
			throw new InvalidOperationException("EndWrite already called.");
		}
		streamAsyncResult.Done = true;
		if (streamAsyncResult.Exception != null)
		{
			throw streamAsyncResult.Exception;
		}
	}
}
