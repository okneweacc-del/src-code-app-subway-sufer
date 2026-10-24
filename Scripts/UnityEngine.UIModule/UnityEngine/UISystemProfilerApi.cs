using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class UISystemProfilerApi
{
	public enum SampleType
	{
		Layout = 0,
		Render = 1
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void BeginSample(SampleType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void EndSample(SampleType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void AddMarker(string name, Object obj);
}
