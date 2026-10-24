using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public sealed class GUIStyle
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	[NonSerialized]
	private GUIStyleState m_Normal;

	[NonSerialized]
	private GUIStyleState m_Hover;

	[NonSerialized]
	private GUIStyleState m_Active;

	[NonSerialized]
	private GUIStyleState m_Focused;

	[NonSerialized]
	private GUIStyleState m_OnNormal;

	[NonSerialized]
	private GUIStyleState m_OnHover;

	[NonSerialized]
	private GUIStyleState m_OnActive;

	[NonSerialized]
	private GUIStyleState m_OnFocused;

	[NonSerialized]
	private RectOffset m_Border;

	[NonSerialized]
	private RectOffset m_Padding;

	[NonSerialized]
	private RectOffset m_Margin;

	[NonSerialized]
	private RectOffset m_Overflow;

	[NonSerialized]
	private Font m_FontInternal;

	internal static bool showKeyboardFocus = true;

	private static GUIStyle s_None;

	public extern string name
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern ImagePosition imagePosition
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern TextAnchor alignment
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool wordWrap
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern float fixedWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern float fixedHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool stretchWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool stretchHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int fontSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public GUIStyleState normal
	{
		get
		{
			if (m_Normal == null)
			{
				m_Normal = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(0));
			}
			return m_Normal;
		}
	}

	public RectOffset margin
	{
		get
		{
			if (m_Margin == null)
			{
				m_Margin = new RectOffset(this, GetRectOffsetPtr(1));
			}
			return m_Margin;
		}
	}

	public RectOffset padding
	{
		get
		{
			if (m_Padding == null)
			{
				m_Padding = new RectOffset(this, GetRectOffsetPtr(2));
			}
			return m_Padding;
		}
	}

	public static GUIStyle none
	{
		get
		{
			if (s_None == null)
			{
				s_None = new GUIStyle();
			}
			return s_None;
		}
	}

	public bool isHeightDependantOnWidth => fixedHeight == 0f && wordWrap && imagePosition != ImagePosition.ImageOnly;

	public GUIStyle()
	{
		Init();
	}

	public GUIStyle(GUIStyle other)
	{
		InitCopy(other);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void Init();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void InitCopy(GUIStyle other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void Cleanup();

	[ThreadAndSerializationSafe]
	private IntPtr GetStyleStatePtr(int idx)
	{
		INTERNAL_CALL_GetStyleStatePtr(this, idx, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetStyleStatePtr(GUIStyle self, int idx, out IntPtr value);

	private IntPtr GetRectOffsetPtr(int idx)
	{
		INTERNAL_CALL_GetRectOffsetPtr(this, idx, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetRectOffsetPtr(GUIStyle self, int idx, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_Draw(GUIContent content, ref Internal_DrawArguments arguments);

	private static void Internal_Draw2(IntPtr style, Rect position, GUIContent content, int controlID, bool on)
	{
		INTERNAL_CALL_Internal_Draw2(style, ref position, content, controlID, on);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_Draw2(IntPtr style, ref Rect position, GUIContent content, int controlID, bool on);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void SetDefaultFont(Font font);

	internal static void Internal_CalcSizeWithConstraints(IntPtr target, GUIContent content, Vector2 maxSize, out Vector2 ret)
	{
		INTERNAL_CALL_Internal_CalcSizeWithConstraints(target, content, ref maxSize, out ret);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_CalcSizeWithConstraints(IntPtr target, GUIContent content, ref Vector2 maxSize, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float Internal_CalcHeight(IntPtr target, GUIContent content, float width);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_CalcMinMaxWidth(IntPtr target, GUIContent content, out float minWidth, out float maxWidth);

	~GUIStyle()
	{
		Cleanup();
	}

	private static void Internal_Draw(IntPtr target, Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Internal_DrawArguments arguments = new Internal_DrawArguments
		{
			target = target,
			position = position,
			isHover = (isHover ? 1 : 0),
			isActive = (isActive ? 1 : 0),
			on = (on ? 1 : 0),
			hasKeyboardFocus = (hasKeyboardFocus ? 1 : 0)
		};
		Internal_Draw(content, ref arguments);
	}

	public void Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Internal_Draw(m_Ptr, position, GUIContent.none, isHover, isActive, on, hasKeyboardFocus);
	}

	public void Draw(Rect position, GUIContent content, int controlID)
	{
		Draw(position, content, controlID, on: false);
	}

	public void Draw(Rect position, GUIContent content, int controlID, bool on)
	{
		if (content != null)
		{
			Internal_Draw2(m_Ptr, position, content, controlID, on);
		}
		else
		{
			Debug.LogError("Style.Draw may not be called with GUIContent that is null.");
		}
	}

	public static implicit operator GUIStyle(string str)
	{
		if (GUISkin.current == null)
		{
			Debug.LogError("Unable to use a named GUIStyle without a current skin. Most likely you need to move your GUIStyle initialization code to OnGUI");
			return GUISkin.error;
		}
		return GUISkin.current.GetStyle(str);
	}

	internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)
	{
		Internal_CalcSizeWithConstraints(m_Ptr, content, constraints, out var ret);
		return ret;
	}

	public float CalcHeight(GUIContent content, float width)
	{
		return Internal_CalcHeight(m_Ptr, content, width);
	}

	public void CalcMinMaxWidth(GUIContent content, out float minWidth, out float maxWidth)
	{
		Internal_CalcMinMaxWidth(m_Ptr, content, out minWidth, out maxWidth);
	}

	public override string ToString()
	{
		return UnityString.Format("GUIStyle '{0}'", name);
	}
}
