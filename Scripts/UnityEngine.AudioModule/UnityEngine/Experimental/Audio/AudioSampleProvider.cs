using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Audio;

[NativeType(Header = "Modules/Audio/Public/ScriptBindings/AudioSampleProvider.bindings.h")]
[StaticAccessor("AudioSampleProviderBindings", StaticAccessorType.DoubleColon)]
public class AudioSampleProvider : IDisposable
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate uint ConsumeSampleFramesNativeFunction(uint providerId, IntPtr interleavedSampleFrames, uint sampleFrameCount);

	public delegate void SampleFramesHandler(AudioSampleProvider provider, uint sampleFrameCount);

	private ConsumeSampleFramesNativeFunction m_ConsumeSampleFramesNativeFunction;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ushort trackIndex;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Object owner;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ushort channelCount;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private uint sampleRate;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SampleFramesHandler sampleFramesAvailable;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SampleFramesHandler sampleFramesOverflow;

	public uint id { get; private set; }

	~AudioSampleProvider()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (id != 0)
		{
			InternalSetScriptingPtr(id, null);
			id = 0u;
		}
		GC.SuppressFinalize(this);
	}

	[RequiredByNativeCode]
	private void InvokeSampleFramesAvailable(int sampleFrameCount)
	{
		if (sampleFramesAvailable != null)
		{
			sampleFramesAvailable(this, (uint)sampleFrameCount);
		}
	}

	[RequiredByNativeCode]
	private void InvokeSampleFramesOverflow(int droppedSampleFrameCount)
	{
		if (sampleFramesOverflow != null)
		{
			sampleFramesOverflow(this, (uint)droppedSampleFrameCount);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private static extern void InternalSetScriptingPtr(uint providerId, AudioSampleProvider provider);
}
