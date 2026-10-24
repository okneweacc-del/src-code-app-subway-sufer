namespace UnityEngine;

public class GUILayout
{
	private sealed class LayoutedWindow
	{
		private readonly GUI.WindowFunction m_Func;

		private readonly Rect m_ScreenRect;

		private readonly GUILayoutOption[] m_Options;

		private readonly GUIStyle m_Style;

		internal LayoutedWindow(GUI.WindowFunction f, Rect screenRect, GUIContent content, GUILayoutOption[] options, GUIStyle style)
		{
			m_Func = f;
			m_ScreenRect = screenRect;
			m_Options = options;
			m_Style = style;
		}

		public void DoWindow(int windowID)
		{
			GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				topLevel.resetCoords = true;
				topLevel.rect = m_ScreenRect;
				if (m_Options != null)
				{
					topLevel.ApplyOptions(m_Options);
				}
				topLevel.isWindow = true;
				topLevel.windowID = windowID;
				topLevel.style = m_Style;
			}
			else
			{
				topLevel.ResetCursor();
			}
			m_Func(windowID);
		}
	}

	public static void Label(string text, params GUILayoutOption[] options)
	{
		DoLabel(GUIContent.Temp(text), GUI.skin.label, options);
	}

	private static void DoLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		GUI.Label(GUILayoutUtility.GetRect(content, style, options), content, style);
	}

	public static bool Button(string text, params GUILayoutOption[] options)
	{
		return DoButton(GUIContent.Temp(text), GUI.skin.button, options);
	}

	public static bool Button(GUIContent content, params GUILayoutOption[] options)
	{
		return DoButton(content, GUI.skin.button, options);
	}

	private static bool DoButton(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		return GUI.Button(GUILayoutUtility.GetRect(content, style, options), content, style);
	}

	public static bool Toggle(bool value, GUIContent content, params GUILayoutOption[] options)
	{
		return DoToggle(value, content, GUI.skin.toggle, options);
	}

	private static bool DoToggle(bool value, GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		return GUI.Toggle(GUILayoutUtility.GetRect(content, style, options), value, content, style);
	}

	public static void BeginHorizontal(params GUILayoutOption[] options)
	{
		BeginHorizontal(GUIContent.none, GUIStyle.none, options);
	}

	public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
	{
		BeginHorizontal(GUIContent.none, style, options);
	}

	public static void BeginHorizontal(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		BeginHorizontal(GUIContent.Temp(text), style, options);
	}

	public static void BeginHorizontal(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
		gUILayoutGroup.isVertical = false;
		if (style != GUIStyle.none || content != GUIContent.none)
		{
			GUI.Box(gUILayoutGroup.rect, content, style);
		}
	}

	public static void EndHorizontal()
	{
		GUILayoutUtility.EndLayoutGroup();
	}

	public static void BeginVertical(params GUILayoutOption[] options)
	{
		BeginVertical(GUIContent.none, GUIStyle.none, options);
	}

	public static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
	{
		BeginVertical(GUIContent.none, style, options);
	}

	public static void BeginVertical(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		BeginVertical(GUIContent.Temp(text), style, options);
	}

	public static void BeginVertical(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
		gUILayoutGroup.isVertical = true;
		if (style != GUIStyle.none || content != GUIContent.none)
		{
			GUI.Box(gUILayoutGroup.rect, content, style);
		}
	}

	public static void EndVertical()
	{
		GUILayoutUtility.EndLayoutGroup();
	}

	public static void BeginArea(Rect screenRect)
	{
		BeginArea(screenRect, GUIContent.none, GUIStyle.none);
	}

	public static void BeginArea(Rect screenRect, string text)
	{
		BeginArea(screenRect, GUIContent.Temp(text), GUIStyle.none);
	}

	public static void BeginArea(Rect screenRect, string text, GUIStyle style)
	{
		BeginArea(screenRect, GUIContent.Temp(text), style);
	}

	public static void BeginArea(Rect screenRect, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutArea(style, typeof(GUILayoutGroup));
		if (Event.current.type == EventType.Layout)
		{
			gUILayoutGroup.resetCoords = true;
			gUILayoutGroup.minWidth = (gUILayoutGroup.maxWidth = screenRect.width);
			gUILayoutGroup.minHeight = (gUILayoutGroup.maxHeight = screenRect.height);
			gUILayoutGroup.rect = Rect.MinMaxRect(screenRect.xMin, screenRect.yMin, gUILayoutGroup.rect.xMax, gUILayoutGroup.rect.yMax);
		}
		GUI.BeginGroup(gUILayoutGroup.rect, content, style);
	}

	public static void EndArea()
	{
		GUIUtility.CheckOnGUI();
		if (Event.current.type != EventType.Used)
		{
			GUILayoutUtility.current.layoutGroups.Pop();
			GUILayoutUtility.current.topLevel = (GUILayoutGroup)GUILayoutUtility.current.layoutGroups.Peek();
			GUI.EndGroup();
		}
	}

	public static Vector2 BeginScrollView(Vector2 scrollPosition, params GUILayoutOption[] options)
	{
		return BeginScrollView(scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
	}

	public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
	{
		return BeginScrollView(scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView, options);
	}

	public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
	{
		string name = style.name;
		GUIStyle gUIStyle = GUI.skin.FindStyle(name + "VerticalScrollbar");
		if (gUIStyle == null)
		{
			gUIStyle = GUI.skin.verticalScrollbar;
		}
		GUIStyle gUIStyle2 = GUI.skin.FindStyle(name + "HorizontalScrollbar");
		if (gUIStyle2 == null)
		{
			gUIStyle2 = GUI.skin.horizontalScrollbar;
		}
		return BeginScrollView(scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false, gUIStyle2, gUIStyle, style, options);
	}

	public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
	{
		GUIUtility.CheckOnGUI();
		GUIScrollGroup gUIScrollGroup = (GUIScrollGroup)GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
		EventType type = Event.current.type;
		if (type == EventType.Layout)
		{
			gUIScrollGroup.resetCoords = true;
			gUIScrollGroup.isVertical = true;
			gUIScrollGroup.stretchWidth = 1;
			gUIScrollGroup.stretchHeight = 1;
			gUIScrollGroup.verticalScrollbar = verticalScrollbar;
			gUIScrollGroup.horizontalScrollbar = horizontalScrollbar;
			gUIScrollGroup.needsVerticalScrollbar = alwaysShowVertical;
			gUIScrollGroup.needsHorizontalScrollbar = alwaysShowHorizontal;
			gUIScrollGroup.ApplyOptions(options);
		}
		return GUI.BeginScrollView(gUIScrollGroup.rect, scrollPosition, new Rect(0f, 0f, gUIScrollGroup.clientWidth, gUIScrollGroup.clientHeight), alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
	}

	public static void EndScrollView()
	{
		EndScrollView(handleScrollWheel: true);
	}

	internal static void EndScrollView(bool handleScrollWheel)
	{
		GUILayoutUtility.EndLayoutGroup();
		GUI.EndScrollView(handleScrollWheel);
	}

	public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options)
	{
		return DoWindow(id, screenRect, func, GUIContent.Temp(text), GUI.skin.window, options);
	}

	private static Rect DoWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		GUIUtility.CheckOnGUI();
		LayoutedWindow layoutedWindow = new LayoutedWindow(func, screenRect, content, options, style);
		return GUI.Window(id, screenRect, layoutedWindow.DoWindow, content, style);
	}

	public static GUILayoutOption Width(float width)
	{
		return new GUILayoutOption(GUILayoutOption.Type.fixedWidth, width);
	}

	public static GUILayoutOption Height(float height)
	{
		return new GUILayoutOption(GUILayoutOption.Type.fixedHeight, height);
	}

	public static GUILayoutOption MinHeight(float minHeight)
	{
		return new GUILayoutOption(GUILayoutOption.Type.minHeight, minHeight);
	}

	public static GUILayoutOption ExpandWidth(bool expand)
	{
		return new GUILayoutOption(GUILayoutOption.Type.stretchWidth, expand ? 1 : 0);
	}
}
