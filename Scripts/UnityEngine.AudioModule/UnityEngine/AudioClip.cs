using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class AudioClip : Object
{
	public delegate void PCMReaderCallback(float[] data);

	public delegate void PCMSetPositionCallback(int position);

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PCMReaderCallback m_PCMReaderCallback = null;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PCMSetPositionCallback m_PCMSetPositionCallback = null;

	public extern float length
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool ambisonic
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	private AudioClip()
	{
	}

	[RequiredByNativeCode]
	private void InvokePCMReaderCallback_Internal(float[] data)
	{
		if (m_PCMReaderCallback != null)
		{
			m_PCMReaderCallback(data);
		}
	}

	[RequiredByNativeCode]
	private void InvokePCMSetPositionCallback_Internal(int position)
	{
		if (m_PCMSetPositionCallback != null)
		{
			m_PCMSetPositionCallback(position);
		}
	}
}
