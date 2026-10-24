using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class ReflectionProbe : Behaviour
{
	public extern int resolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool boxProjection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern ReflectionProbeMode mode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern Texture customBakedTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}
}
