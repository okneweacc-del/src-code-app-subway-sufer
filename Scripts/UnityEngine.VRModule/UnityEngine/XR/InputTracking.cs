using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR;

[RequiredByNativeCode]
public static class InputTracking
{
	private enum TrackingStateEventType
	{
		NodeAdded = 0,
		NodeRemoved = 1,
		TrackingAcquired = 2,
		TrackingLost = 3
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static Action<XRNodeState> trackingAcquired = null;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static Action<XRNodeState> trackingLost = null;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<XRNodeState> nodeAdded = null;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static Action<XRNodeState> nodeRemoved = null;

	[RequiredByNativeCode]
	private static void InvokeTrackingEvent(TrackingStateEventType eventType, XRNode nodeType, long uniqueID, bool tracked)
	{
		Action<XRNodeState> action = null;
		XRNodeState obj = new XRNodeState
		{
			uniqueID = (ulong)uniqueID,
			nodeType = nodeType,
			tracked = tracked
		};
		((Action<XRNodeState>)(eventType switch
		{
			TrackingStateEventType.TrackingAcquired => trackingAcquired, 
			TrackingStateEventType.TrackingLost => trackingLost, 
			TrackingStateEventType.NodeAdded => nodeAdded, 
			TrackingStateEventType.NodeRemoved => nodeRemoved, 
			_ => throw new ArgumentException("TrackingEventHandler - Invalid EventType: " + eventType), 
		}))?.Invoke(obj);
	}
}
