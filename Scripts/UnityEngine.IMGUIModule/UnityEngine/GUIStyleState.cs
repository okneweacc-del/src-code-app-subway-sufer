using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[StructLayout((LayoutKind)0)]
public sealed class GUIStyleState
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	private readonly GUIStyle m_SourceStyle;

	[NonSerialized]
	private Texture2D m_Background;

	public Color textColor
	{
		set
		{
			INTERNAL_set_textColor(ref value);
		}
	}

	public Texture2D background
	{
		set
		{
			SetBackgroundInternal(value);
			m_Background = value;
		}
	}

	public GUIStyleState()
	{
		Init();
	}

	private GUIStyleState(GUIStyle sourceStyle, IntPtr source)
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

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetBackgroundInternal(Texture2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Texture2D GetBackgroundInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_textColor(ref Color value);

	internal static GUIStyleState GetGUIStyleState(GUIStyle sourceStyle, IntPtr source)
	{
		GUIStyleState gUIStyleState = new GUIStyleState(sourceStyle, source);
		gUIStyleState.m_Background = gUIStyleState.GetBackgroundInternal();
		return gUIStyleState;
	}

	~GUIStyleState()
	{
		if (m_SourceStyle == null)
		{
			Cleanup();
		}
	}
}
