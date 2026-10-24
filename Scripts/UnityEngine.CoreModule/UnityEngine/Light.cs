using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Camera/Light.h")]
public sealed class Light : Behaviour
{
	private int m_BakedIndex;

	public Color color
	{
		get
		{
			get_color_Injected(out var ret);
			return ret;
		}
		set
		{
			set_color_Injected(ref value);
		}
	}

	public extern float intensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_color_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_color_Injected(ref Color value);
}
