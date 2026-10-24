using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public class Behaviour : Component
{
	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool isActiveAndEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}
}
