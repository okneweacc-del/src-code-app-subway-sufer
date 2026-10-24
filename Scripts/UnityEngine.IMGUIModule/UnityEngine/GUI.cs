using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

public class GUI
{
	public delegate void WindowFunction(int id);

	private static float s_ScrollStepSize;

	private static int s_ScrollControlId;

	private static int s_HotTextField;

	private static readonly int s_BoxHash;

	private static readonly int s_RepeatButtonHash;

	private static readonly int s_ToggleHash;

	private static readonly int s_ButtonGridHash;

	private static readonly int s_SliderHash;

	private static readonly int s_BeginGroupHash;

	private static readonly int s_ScrollviewHash;

	private static GUISkin s_Skin;

	private static GenericStack s_ScrollViewStates;

	public static Color color
	{
		get
		{
			INTERNAL_get_color(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_color(ref value);
		}
	}

	public static Color backgroundColor
	{
		get
		{
			INTERNAL_get_backgroundColor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_backgroundColor(ref value);
		}
	}

	public static Color contentColor
	{
		get
		{
			INTERNAL_get_contentColor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_contentColor(ref value);
		}
	}

	public static extern bool changed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern int depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	internal static extern bool usePageScrollbars
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal static int scrollTroughSide { get; set; }

	internal static DateTime nextScrollStepTime { get; set; }

	public static GUISkin skin
	{
		get
		{
			GUIUtility.CheckOnGUI();
			return s_Skin;
		}
		set
		{
			GUIUtility.CheckOnGUI();
			DoSetSkin(value);
		}
	}

	static GUI()
	{
		s_ScrollStepSize = 10f;
		s_HotTextField = -1;
		s_BoxHash = "Box".GetHashCode();
		s_RepeatButtonHash = "repeatButton".GetHashCode();
		s_ToggleHash = "Toggle".GetHashCode();
		s_ButtonGridHash = "ButtonGrid".GetHashCode();
		s_SliderHash = "Slider".GetHashCode();
		s_BeginGroupHash = "BeginGroup".GetHashCode();
		s_ScrollviewHash = "scrollView".GetHashCode();
		s_ScrollViewStates = new GenericStack();
		nextScrollStepTime = DateTime.Now;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_color(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_color(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_backgroundColor(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_backgroundColor(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_contentColor(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_contentColor(ref Color value);

	private static void DoLabel(Rect position, GUIContent content, IntPtr style)
	{
		INTERNAL_CALL_DoLabel(ref position, content, style);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DoLabel(ref Rect position, GUIContent content, IntPtr style);

	private static bool DoButton(Rect position, GUIContent content, IntPtr style)
	{
		return INTERNAL_CALL_DoButton(ref position, content, style);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_DoButton(ref Rect position, GUIContent content, IntPtr style);

	internal static bool DoToggle(Rect position, int id, bool value, GUIContent content, IntPtr style)
	{
		return INTERNAL_CALL_DoToggle(ref position, id, value, content, style);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_DoToggle(ref Rect position, int id, bool value, GUIContent content, IntPtr style);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void InternalRepaintEditorWindow();

	private static Rect Internal_DoWindow(int id, int instanceID, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout)
	{
		INTERNAL_CALL_Internal_DoWindow(id, instanceID, ref clientRect, func, title, style, skin, forceRectOnLayout, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_DoWindow(int id, int instanceID, ref Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout, out Rect value);

	public static void DragWindow(Rect position)
	{
		INTERNAL_CALL_DragWindow(ref position);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DragWindow(ref Rect position);

	internal static void DoSetSkin(GUISkin newSkin)
	{
		if (!newSkin)
		{
			newSkin = GUIUtility.GetDefaultSkin();
		}
		s_Skin = newSkin;
		newSkin.MakeCurrent();
	}

	public static void Label(Rect position, string text)
	{
		Label(position, GUIContent.Temp(text), s_Skin.label);
	}

	public static void Label(Rect position, string text, GUIStyle style)
	{
		Label(position, GUIContent.Temp(text), style);
	}

	public static void Label(Rect position, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		DoLabel(position, content, style.m_Ptr);
	}

	public static void Box(Rect position, string text)
	{
		Box(position, GUIContent.Temp(text), s_Skin.box);
	}

	public static void Box(Rect position, string text, GUIStyle style)
	{
		Box(position, GUIContent.Temp(text), style);
	}

	public static void Box(Rect position, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_BoxHash, FocusType.Passive);
		if (Event.current.type == EventType.Repaint)
		{
			style.Draw(position, content, controlID);
		}
	}

	public static bool Button(Rect position, string text)
	{
		return Button(position, GUIContent.Temp(text), s_Skin.button);
	}

	public static bool Button(Rect position, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoButton(position, content, style.m_Ptr);
	}

	private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_RepeatButtonHash, focusType, position);
		switch (Event.current.GetTypeForControl(controlID))
		{
		case EventType.MouseDown:
			if (position.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = controlID;
				Event.current.Use();
			}
			return false;
		case EventType.MouseUp:
			if (GUIUtility.hotControl == controlID)
			{
				GUIUtility.hotControl = 0;
				Event.current.Use();
				return position.Contains(Event.current.mousePosition);
			}
			return false;
		case EventType.Repaint:
			style.Draw(position, content, controlID);
			return controlID == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
		default:
			return false;
		}
	}

	public static bool Toggle(Rect position, bool value, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoToggle(position, GUIUtility.GetControlID(s_ToggleHash, FocusType.Passive, position), value, content, style.m_Ptr);
	}

	public static float Slider(Rect position, float value, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id)
	{
		GUIUtility.CheckOnGUI();
		if (id == 0)
		{
			id = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);
		}
		return new SliderHandler(position, value, size, start, end, slider, thumb, horiz, id).Handle();
	}

	public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle style)
	{
		return Scroller(position, value, size, leftValue, rightValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "leftbutton"), skin.GetStyle(style.name + "rightbutton"), horiz: true);
	}

	internal static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
	{
		bool result = false;
		if (DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
		{
			bool flag = s_ScrollControlId != scrollerID;
			s_ScrollControlId = scrollerID;
			if (flag)
			{
				result = true;
				nextScrollStepTime = DateTime.Now.AddMilliseconds(250.0);
			}
			else if (DateTime.Now >= nextScrollStepTime)
			{
				result = true;
				nextScrollStepTime = DateTime.Now.AddMilliseconds(30.0);
			}
			if (Event.current.type == EventType.Repaint)
			{
				InternalRepaintEditorWindow();
			}
		}
		return result;
	}

	public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue, GUIStyle style)
	{
		return Scroller(position, value, size, topValue, bottomValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "upbutton"), skin.GetStyle(style.name + "downbutton"), horiz: false);
	}

	internal static float Scroller(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);
		Rect position2;
		Rect rect;
		Rect rect2;
		if (horiz)
		{
			position2 = new Rect(position.x + leftButton.fixedWidth, position.y, position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height);
			rect = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
			rect2 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
		}
		else
		{
			position2 = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, position.height - leftButton.fixedHeight - rightButton.fixedHeight);
			rect = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
			rect2 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
		}
		value = Slider(position2, value, size, leftValue, rightValue, slider, thumb, horiz, controlID);
		bool flag = false;
		if (Event.current.type == EventType.MouseUp)
		{
			flag = true;
		}
		if (ScrollerRepeatButton(controlID, rect, leftButton))
		{
			value -= s_ScrollStepSize * ((!(leftValue < rightValue)) ? (-1f) : 1f);
		}
		if (ScrollerRepeatButton(controlID, rect2, rightButton))
		{
			value += s_ScrollStepSize * ((!(leftValue < rightValue)) ? (-1f) : 1f);
		}
		if (flag && Event.current.type == EventType.Used)
		{
			s_ScrollControlId = 0;
		}
		value = ((!(leftValue < rightValue)) ? Mathf.Clamp(value, rightValue, leftValue - size) : Mathf.Clamp(value, leftValue, rightValue - size));
		return value;
	}

	public static void BeginGroup(Rect position, GUIContent content, GUIStyle style)
	{
		BeginGroup(position, content, style, Vector2.zero);
	}

	internal static void BeginGroup(Rect position, GUIContent content, GUIStyle style, Vector2 scrollOffset)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_BeginGroupHash, FocusType.Passive);
		if (content != GUIContent.none || style != GUIStyle.none)
		{
			EventType type = Event.current.type;
			if (type == EventType.Repaint)
			{
				style.Draw(position, content, controlID);
			}
			else if (position.Contains(Event.current.mousePosition))
			{
				GUIUtility.mouseUsed = true;
			}
		}
		GUIClip.Push(position, scrollOffset, Vector2.zero, resetOffset: false);
	}

	public static void EndGroup()
	{
		GUIUtility.CheckOnGUI();
		GUIClip.Internal_Pop();
	}

	internal static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_ScrollviewHash, FocusType.Passive);
		ScrollViewState scrollViewState = (ScrollViewState)GUIUtility.GetStateObject(typeof(ScrollViewState), controlID);
		if (scrollViewState.apply)
		{
			scrollPosition = scrollViewState.scrollPosition;
			scrollViewState.apply = false;
		}
		scrollViewState.position = position;
		scrollViewState.scrollPosition = scrollPosition;
		scrollViewState.visibleRect = (scrollViewState.viewRect = viewRect);
		scrollViewState.visibleRect.width = position.width;
		scrollViewState.visibleRect.height = position.height;
		s_ScrollViewStates.Push(scrollViewState);
		Rect screenRect = new Rect(position);
		switch (Event.current.type)
		{
		case EventType.Layout:
			GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			break;
		default:
		{
			bool flag = alwaysShowVertical;
			bool flag2 = alwaysShowHorizontal;
			if (flag2 || viewRect.width > screenRect.width)
			{
				scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
				screenRect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
				flag2 = true;
			}
			if (flag || viewRect.height > screenRect.height)
			{
				scrollViewState.visibleRect.width = position.width - verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
				screenRect.width -= verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
				flag = true;
				if (!flag2 && viewRect.width > screenRect.width)
				{
					scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
					screenRect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
					flag2 = true;
				}
			}
			if (Event.current.type == EventType.Repaint && background != GUIStyle.none)
			{
				background.Draw(position, position.Contains(Event.current.mousePosition), isActive: false, flag2 && flag, hasKeyboardFocus: false);
			}
			if (flag2 && horizontalScrollbar != GUIStyle.none)
			{
				scrollPosition.x = HorizontalScrollbar(new Rect(position.x, position.yMax - horizontalScrollbar.fixedHeight, screenRect.width, horizontalScrollbar.fixedHeight), scrollPosition.x, Mathf.Min(screenRect.width, viewRect.width), 0f, viewRect.width, horizontalScrollbar);
			}
			else
			{
				GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
				GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
				if (horizontalScrollbar != GUIStyle.none)
				{
					scrollPosition.x = 0f;
				}
				else
				{
					scrollPosition.x = Mathf.Clamp(scrollPosition.x, 0f, Mathf.Max(viewRect.width - position.width, 0f));
				}
			}
			if (flag && verticalScrollbar != GUIStyle.none)
			{
				scrollPosition.y = VerticalScrollbar(new Rect(screenRect.xMax + (float)verticalScrollbar.margin.left, screenRect.y, verticalScrollbar.fixedWidth, screenRect.height), scrollPosition.y, Mathf.Min(screenRect.height, viewRect.height), 0f, viewRect.height, verticalScrollbar);
				break;
			}
			GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			if (verticalScrollbar != GUIStyle.none)
			{
				scrollPosition.y = 0f;
			}
			else
			{
				scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(viewRect.height - position.height, 0f));
			}
			break;
		}
		case EventType.Used:
			break;
		}
		GUIClip.Push(screenRect, new Vector2(Mathf.Round(0f - scrollPosition.x - viewRect.x), Mathf.Round(0f - scrollPosition.y - viewRect.y)), Vector2.zero, resetOffset: false);
		return scrollPosition;
	}

	public static void EndScrollView(bool handleScrollWheel)
	{
		GUIUtility.CheckOnGUI();
		ScrollViewState scrollViewState = (ScrollViewState)s_ScrollViewStates.Peek();
		GUIClip.Pop();
		s_ScrollViewStates.Pop();
		if (handleScrollWheel && Event.current.type == EventType.ScrollWheel && scrollViewState.position.Contains(Event.current.mousePosition))
		{
			scrollViewState.scrollPosition.x = Mathf.Clamp(scrollViewState.scrollPosition.x + Event.current.delta.x * 20f, 0f, scrollViewState.viewRect.width - scrollViewState.visibleRect.width);
			scrollViewState.scrollPosition.y = Mathf.Clamp(scrollViewState.scrollPosition.y + Event.current.delta.y * 20f, 0f, scrollViewState.viewRect.height - scrollViewState.visibleRect.height);
			if (scrollViewState.scrollPosition.x < 0f)
			{
				scrollViewState.scrollPosition.x = 0f;
			}
			if (scrollViewState.scrollPosition.y < 0f)
			{
				scrollViewState.scrollPosition.y = 0f;
			}
			scrollViewState.apply = true;
			Event.current.Use();
		}
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, string text)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, GUIContent.Temp(text), skin.window, skin, forceRectOnLayout: true);
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, title, style, skin, forceRectOnLayout: true);
	}

	private static Rect DoWindow(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout)
	{
		return Internal_DoWindow(id, GUIUtility.s_OriginalID, clientRect, func, title, style, skin, forceRectOnLayout);
	}

	[RequiredByNativeCode]
	internal static void CallWindowDelegate(WindowFunction func, int id, int instanceID, GUISkin _skin, int forceRect, float width, float height, GUIStyle style)
	{
		GUILayoutUtility.SelectIDList(id, isWindow: true);
		GUISkin gUISkin = skin;
		if (Event.current.type == EventType.Layout)
		{
			if (forceRect != 0)
			{
				GUILayoutOption[] options = new GUILayoutOption[2]
				{
					GUILayout.Width(width),
					GUILayout.Height(height)
				};
				GUILayoutUtility.BeginWindow(id, style, options);
			}
			else
			{
				GUILayoutUtility.BeginWindow(id, style, null);
			}
		}
		else
		{
			GUILayoutUtility.BeginWindow(id, GUIStyle.none, null);
		}
		skin = _skin;
		func(id);
		if (Event.current.type == EventType.Layout)
		{
			GUILayoutUtility.Layout();
		}
		skin = gUISkin;
	}
}
