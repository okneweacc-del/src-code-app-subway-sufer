using System.Runtime.ConstrainedExecution;

namespace System.Security;

[MonoTODO("work in progress - encryption is missing")]
public sealed class SecureString : CriticalFinalizerObject, IDisposable
{
	private int length;

	private bool disposed;

	private byte[] data;

	public int Length
	{
		get
		{
			if (disposed)
			{
				throw new ObjectDisposedException("SecureString");
			}
			return length;
		}
	}

	static SecureString()
	{
	}

	public void Dispose()
	{
		disposed = true;
		if (data != null)
		{
			Array.Clear(data, 0, data.Length);
			data = null;
		}
		length = 0;
	}

	private void Encrypt()
	{
		if (data != null && data.Length <= 0)
		{
		}
	}

	private void Decrypt()
	{
		if (data != null && data.Length <= 0)
		{
		}
	}

	internal byte[] GetBuffer()
	{
		byte[] array = new byte[length << 1];
		try
		{
			Decrypt();
			Buffer.BlockCopy(data, 0, array, 0, array.Length);
			return array;
		}
		finally
		{
			Encrypt();
		}
	}
}
