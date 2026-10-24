using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public class AsyncOperation : YieldInstruction
{
	internal IntPtr m_Ptr;

	private Action<AsyncOperation> m_completeCallback;

	public extern bool isDone
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern float progress
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void InternalDestroy();

	~AsyncOperation()
	{
		InternalDestroy();
	}

	[RequiredByNativeCode]
	internal void InvokeCompletionEvent()
	{
		if (m_completeCallback != null)
		{
			m_completeCallback(this);
			m_completeCallback = null;
		}
	}
}
