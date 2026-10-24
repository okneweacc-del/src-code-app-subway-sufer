using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
public sealed class CullingGroup : IDisposable
{
	public delegate void StateChanged(CullingGroupEvent sphere);

	internal IntPtr m_Ptr;

	private StateChanged m_OnStateChanged;

	~CullingGroup()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			FinalizerFailure();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Dispose();

	[RequiredByNativeCode]
	[SecuritySafeCritical]
	private unsafe static void SendEvents(CullingGroup cullingGroup, IntPtr eventsPtr, int count)
	{
		CullingGroupEvent* ptr = (CullingGroupEvent*)eventsPtr.ToPointer();
		if (cullingGroup.m_OnStateChanged != null)
		{
			for (int i = 0; i < count; i++)
			{
				cullingGroup.m_OnStateChanged(ptr[i]);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void FinalizerFailure();
}
