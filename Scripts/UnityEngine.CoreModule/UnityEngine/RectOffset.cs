using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[StructLayout((LayoutKind)0)]
[UsedByNativeCode]
public sealed class RectOffset
{
	[NonSerialized]
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	internal IntPtr m_Ptr;

	private readonly object m_SourceStyle;

	public extern int left
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int right
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int top
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int bottom
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int horizontal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int vertical
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public RectOffset()
	{
		Init();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	internal RectOffset(object sourceStyle, IntPtr source)
	{
		m_SourceStyle = sourceStyle;
		m_Ptr = source;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void Init();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void Cleanup();

	~RectOffset()
	{
		if (m_SourceStyle == null)
		{
			Cleanup();
		}
	}

	public override string ToString()
	{
		return UnityString.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", left, right, top, bottom);
	}
}
