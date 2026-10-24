using System.Runtime.InteropServices;
using System.Threading;

namespace System;

[ComVisible(true)]
public interface IAsyncResult
{
	object AsyncState { get; }

	WaitHandle AsyncWaitHandle { get; }

	bool IsCompleted { get; }
}
