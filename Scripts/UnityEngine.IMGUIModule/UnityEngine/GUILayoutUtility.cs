using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

public class GUILayoutUtility
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal sealed class LayoutCache
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal GUILayoutGroup topLevel = new GUILayoutGroup();

		internal GenericStack layoutGroups = new GenericStack();

		internal GUILayoutGroup windows = new GUILayoutGroup();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal LayoutCache()
		{
			layoutGroups.Push(topLevel);
		}
	}

	private static readonly Dictionary<int, LayoutCache> s_StoredLayouts = new Dictionary<int, LayoutCache>();

	private static readonly Dictionary<int, LayoutCache> s_StoredWindows = new Dictionary<int, LayoutCache>();

	internal static LayoutCache current = new LayoutCache();

	internal static readonly Rect kDummyRect = new Rect(0f, 0f, 1f, 1f);

	private static GUIStyle s_SpaceStyle;

	internal static GUIStyle spaceStyle
	{
		get
		{
			if (s_SpaceStyle == null)
			{
				s_SpaceStyle = new GUIStyle();
			}
			s_SpaceStyle.stretchWidth = false;
			return s_SpaceStyle;
		}
	}

	private static Rect Internal_GetWindowRect(int windowID)
	{
		INTERNAL_CALL_Internal_GetWindowRect(windowID, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_GetWindowRect(int windowID, out Rect value);

	private static void Internal_MoveWindow(int windowID, Rect r)
	{
		INTERNAL_CALL_Internal_MoveWindow(windowID, ref r);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_MoveWindow(int windowID, ref Rect r);

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static LayoutCache SelectIDList(int instanceID, bool isWindow)
	{
		Dictionary<int, LayoutCache> dictionary = ((!isWindow) ? s_StoredLayouts : s_StoredWindows);
		if (!dictionary.TryGetValue(instanceID, out var value))
		{
			value = (dictionary[instanceID] = new LayoutCache());
		}
		current.topLevel = value.topLevel;
		current.layoutGroups = value.layoutGroups;
		current.windows = value.windows;
		return value;
	}

	internal static void Begin(int instanceID)
	{
		LayoutCache layoutCache = SelectIDList(instanceID, isWindow: false);
		if (Event.current.type == EventType.Layout)
		{
			current.topLevel = (layoutCache.topLevel = new GUILayoutGroup());
			current.layoutGroups.Clear();
			current.layoutGroups.Push(current.topLevel);
			current.windows = (layoutCache.windows = new GUILayoutGroup());
		}
		else
		{
			current.topLevel = layoutCache.topLevel;
			current.layoutGroups = layoutCache.layoutGroups;
			current.windows = layoutCache.windows;
		}
	}

	internal static void BeginWindow(int windowID, GUIStyle style, GUILayoutOption[] options)
	{
		LayoutCache layoutCache = SelectIDList(windowID, isWindow: true);
		if (Event.current.type == EventType.Layout)
		{
			current.topLevel = (layoutCache.topLevel = new GUILayoutGroup());
			current.topLevel.style = style;
			current.topLevel.windowID = windowID;
			if (options != null)
			{
				current.topLevel.ApplyOptions(options);
			}
			current.layoutGroups.Clear();
			current.layoutGroups.Push(current.topLevel);
			current.windows = (layoutCache.windows = new GUILayoutGroup());
		}
		else
		{
			current.topLevel = layoutCache.topLevel;
			current.layoutGroups = layoutCache.layoutGroups;
			current.windows = layoutCache.windows;
		}
	}

	internal static void Layout()
	{
		if (current.topLevel.windowID == -1)
		{
			current.topLevel.CalcWidth();
			current.topLevel.SetHorizontal(0f, Mathf.Min((float)Screen.width / GUIUtility.pixelsPerPoint, current.topLevel.maxWidth));
			current.topLevel.CalcHeight();
			current.topLevel.SetVertical(0f, Mathf.Min((float)Screen.height / GUIUtility.pixelsPerPoint, current.topLevel.maxHeight));
			LayoutFreeGroup(current.windows);
		}
		else
		{
			LayoutSingleGroup(current.topLevel);
			LayoutFreeGroup(current.windows);
		}
	}

	internal static void LayoutFromEditorWindow()
	{
		if (current.topLevel != null)
		{
			current.topLevel.CalcWidth();
			current.topLevel.SetHorizontal(0f, (float)Screen.width / GUIUtility.pixelsPerPoint);
			current.topLevel.CalcHeight();
			current.topLevel.SetVertical(0f, (float)Screen.height / GUIUtility.pixelsPerPoint);
			LayoutFreeGroup(current.windows);
		}
		else
		{
			Debug.LogError("GUILayout state invalid. Verify that all layout begin/end calls match.");
		}
	}

	internal static void LayoutFreeGroup(GUILayoutGroup toplevel)
	{
		foreach (GUILayoutGroup entry in toplevel.entries)
		{
			LayoutSingleGroup(entry);
		}
		toplevel.ResetCursor();
	}

	private static void LayoutSingleGroup(GUILayoutGroup i)
	{
		if (!i.isWindow)
		{
			float minWidth = i.minWidth;
			float maxWidth = i.maxWidth;
			i.CalcWidth();
			i.SetHorizontal(i.rect.x, Mathf.Clamp(i.maxWidth, minWidth, maxWidth));
			float minHeight = i.minHeight;
			float maxHeight = i.maxHeight;
			i.CalcHeight();
			i.SetVertical(i.rect.y, Mathf.Clamp(i.maxHeight, minHeight, maxHeight));
		}
		else
		{
			i.CalcWidth();
			Rect rect = Internal_GetWindowRect(i.windowID);
			i.SetHorizontal(rect.x, Mathf.Clamp(rect.width, i.minWidth, i.maxWidth));
			i.CalcHeight();
			i.SetVertical(rect.y, Mathf.Clamp(rect.height, i.minHeight, i.maxHeight));
			Internal_MoveWindow(i.windowID, i.rect);
		}
	}

	[SecuritySafeCritical]
	private static GUILayoutGroup CreateGUILayoutGroupInstanceOfType(Type LayoutType)
	{
		if (!typeof(GUILayoutGroup).IsAssignableFrom(LayoutType))
		{
			throw new ArgumentException("LayoutType needs to be of type GUILayoutGroup");
		}
		return (GUILayoutGroup)Activator.CreateInstance(LayoutType);
	}

	internal static GUILayoutGroup BeginLayoutGroup(GUIStyle style, GUILayoutOption[] options, Type layoutType)
	{
		EventType type = Event.current.type;
		GUILayoutGroup gUILayoutGroup;
		if (type == EventType.Used || type == EventType.Layout)
		{
			gUILayoutGroup = CreateGUILayoutGroupInstanceOfType(layoutType);
			gUILayoutGroup.style = style;
			if (options != null)
			{
				gUILayoutGroup.ApplyOptions(options);
			}
			current.topLevel.Add(gUILayoutGroup);
		}
		else
		{
			gUILayoutGroup = current.topLevel.GetNext() as GUILayoutGroup;
			if (gUILayoutGroup == null)
			{
				throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
			}
			gUILayoutGroup.ResetCursor();
		}
		current.layoutGroups.Push(gUILayoutGroup);
		current.topLevel = gUILayoutGroup;
		return gUILayoutGroup;
	}

	internal static void EndLayoutGroup()
	{
		if (current.layoutGroups.Count == 0)
		{
			Debug.LogError("EndLayoutGroup: BeginLayoutGroup must be called first.");
			return;
		}
		current.layoutGroups.Pop();
		if (0 < current.layoutGroups.Count)
		{
			current.topLevel = (GUILayoutGroup)current.layoutGroups.Peek();
		}
		else
		{
			current.topLevel = new GUILayoutGroup();
		}
	}

	internal static GUILayoutGroup BeginLayoutArea(GUIStyle style, Type layoutType)
	{
		EventType type = Event.current.type;
		GUILayoutGroup gUILayoutGroup;
		if (type == EventType.Used || type == EventType.Layout)
		{
			gUILayoutGroup = CreateGUILayoutGroupInstanceOfType(layoutType);
			gUILayoutGroup.style = style;
			current.windows.Add(gUILayoutGroup);
		}
		else
		{
			gUILayoutGroup = current.windows.GetNext() as GUILayoutGroup;
			if (gUILayoutGroup == null)
			{
				throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
			}
			gUILayoutGroup.ResetCursor();
		}
		current.layoutGroups.Push(gUILayoutGroup);
		current.topLevel = gUILayoutGroup;
		return gUILayoutGroup;
	}

	public static Rect GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoGetRect(content, style, options);
	}

	private static Rect DoGetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		GUIUtility.CheckOnGUI();
		switch (Event.current.type)
		{
		case EventType.Layout:
			if (style.isHeightDependantOnWidth)
			{
				current.topLevel.Add(new GUIWordWrapSizer(style, content, options));
			}
			else
			{
				Vector2 constraints = new Vector2(0f, 0f);
				if (options != null)
				{
					foreach (GUILayoutOption gUILayoutOption in options)
					{
						switch (gUILayoutOption.type)
						{
						case GUILayoutOption.Type.maxHeight:
							constraints.y = (float)gUILayoutOption.value;
							break;
						case GUILayoutOption.Type.maxWidth:
							constraints.x = (float)gUILayoutOption.value;
							break;
						}
					}
				}
				Vector2 vector = style.CalcSizeWithConstraints(content, constraints);
				current.topLevel.Add(new GUILayoutEntry(vector.x, vector.x, vector.y, vector.y, style, options));
			}
			return kDummyRect;
		case EventType.Used:
			return kDummyRect;
		default:
		{
			GUILayoutEntry next = current.topLevel.GetNext();
			return next.rect;
		}
		}
	}
}
