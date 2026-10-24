using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Video;

[UsedByNativeCode]
[RequireComponent(typeof(Transform))]
public sealed class VideoPlayer : Behaviour
{
	public delegate void EventHandler(VideoPlayer source);

	public delegate void ErrorEventHandler(VideoPlayer source, string message);

	public delegate void FrameReadyEventHandler(VideoPlayer source, long frameIdx);

	public delegate void TimeEventHandler(VideoPlayer source, double seconds);

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private EventHandler prepareCompleted;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private EventHandler loopPointReached;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private EventHandler started;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private EventHandler frameDropped;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ErrorEventHandler errorReceived;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private EventHandler seekCompleted;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private TimeEventHandler clockResyncOccurred;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private FrameReadyEventHandler frameReady;

	[RequiredByNativeCode]
	private static void InvokePrepareCompletedCallback_Internal(VideoPlayer source)
	{
		if (source.prepareCompleted != null)
		{
			source.prepareCompleted(source);
		}
	}

	[RequiredByNativeCode]
	private static void InvokeFrameReadyCallback_Internal(VideoPlayer source, long frameIdx)
	{
		if (source.frameReady != null)
		{
			source.frameReady(source, frameIdx);
		}
	}

	[RequiredByNativeCode]
	private static void InvokeLoopPointReachedCallback_Internal(VideoPlayer source)
	{
		if (source.loopPointReached != null)
		{
			source.loopPointReached(source);
		}
	}

	[RequiredByNativeCode]
	private static void InvokeStartedCallback_Internal(VideoPlayer source)
	{
		if (source.started != null)
		{
			source.started(source);
		}
	}

	[RequiredByNativeCode]
	private static void InvokeFrameDroppedCallback_Internal(VideoPlayer source)
	{
		if (source.frameDropped != null)
		{
			source.frameDropped(source);
		}
	}

	[RequiredByNativeCode]
	private static void InvokeErrorReceivedCallback_Internal(VideoPlayer source, string errorStr)
	{
		if (source.errorReceived != null)
		{
			source.errorReceived(source, errorStr);
		}
	}

	[RequiredByNativeCode]
	private static void InvokeSeekCompletedCallback_Internal(VideoPlayer source)
	{
		if (source.seekCompleted != null)
		{
			source.seekCompleted(source);
		}
	}

	[RequiredByNativeCode]
	private static void InvokeClockResyncOccurredCallback_Internal(VideoPlayer source, double seconds)
	{
		if (source.clockResyncOccurred != null)
		{
			source.clockResyncOccurred(source, seconds);
		}
	}
}
