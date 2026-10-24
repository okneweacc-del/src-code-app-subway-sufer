using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public sealed class Gradient
{
	internal IntPtr m_Ptr;

	[RequiredByNativeCode]
	public Gradient()
	{
		Init();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Init();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void Cleanup();

	~Gradient()
	{
		Cleanup();
	}
}
