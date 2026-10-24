using System;
using System.Diagnostics;

namespace UnityEngine.Playables;

public struct PlayableBinding
{
	public static readonly PlayableBinding[] None = new PlayableBinding[0];

	public static readonly double DefaultDuration = double.PositiveInfinity;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string streamName;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DataStreamType streamType;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Object sourceObject;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Type sourceBindingType;
}
