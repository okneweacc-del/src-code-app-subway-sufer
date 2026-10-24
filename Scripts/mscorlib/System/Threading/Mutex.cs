using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System.Threading;

[ComVisible(true)]
public sealed class Mutex : WaitHandle
{
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public Mutex()
	{
		Handle = CreateMutex_internal(initiallyOwned: false, null, out var _);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public Mutex(bool initiallyOwned)
	{
		Handle = CreateMutex_internal(initiallyOwned, null, out var _);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public Mutex(bool initiallyOwned, string name)
	{
		Handle = CreateMutex_internal(initiallyOwned, name, out var _);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public Mutex(bool initiallyOwned, string name, out bool createdNew)
	{
		Handle = CreateMutex_internal(initiallyOwned, name, out createdNew);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateMutex_internal(bool initiallyOwned, string name, out bool created);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ReleaseMutex_internal(IntPtr handle);

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public void ReleaseMutex()
	{
		if (!ReleaseMutex_internal(Handle))
		{
			throw new ApplicationException("Mutex is not owned");
		}
	}
}
