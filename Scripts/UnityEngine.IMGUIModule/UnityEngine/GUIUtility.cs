using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
[NativeHeader("Modules/IMGUI/GUIUtility.h")]
public class GUIUtility
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static int s_SkinMode;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static int s_OriginalID;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static Action takeCapture;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static Action releaseCapture;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static Func<int, IntPtr, bool> processEvent;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static Func<Exception, bool> endContainerGUIFromException;

	internal static Vector2 s_EditorScreenPointOffset = Vector2.zero;

	public static extern string systemCopyBuffer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static extern bool mouseUsed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	internal static float pixelsPerPoint
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		get
		{
			return Internal_GetPixelsPerPoint();
		}
	}

	internal static bool guiIsExiting { set; }

	public static int hotControl
	{
		get
		{
			return Internal_GetHotControl();
		}
		set
		{
			Internal_SetHotControl(value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float Internal_GetPixelsPerPoint();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern int GetControlID(int hint, FocusType focus);

	private static int Internal_GetNextControlID2(int hint, FocusType focusType, Rect rect)
	{
		return INTERNAL_CALL_Internal_GetNextControlID2(hint, focusType, ref rect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_Internal_GetNextControlID2(int hint, FocusType focusType, ref Rect rect);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Internal_GetHotControl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetHotControl(int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern GUISkin Internal_GetDefaultSkin(int skinMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_ExitGUI();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[GeneratedByOldBindingsGenerator]
	internal static extern int Internal_GetGUIDepth();

	public static int GetControlID(int hint, FocusType focus, Rect position)
	{
		return Internal_GetNextControlID2(hint, focus, position);
	}

	public static object GetStateObject(Type t, int controlID)
	{
		return GUIStateObjects.GetStateObject(t, controlID);
	}

	[RequiredByNativeCode]
	internal static void TakeCapture()
	{
		if (takeCapture != null)
		{
			takeCapture();
		}
	}

	[RequiredByNativeCode]
	internal static void RemoveCapture()
	{
		if (releaseCapture != null)
		{
			releaseCapture();
		}
	}

	internal static GUISkin GetDefaultSkin()
	{
		return Internal_GetDefaultSkin(s_SkinMode);
	}

	[RequiredByNativeCode]
	internal static bool ProcessEvent(int instanceID, IntPtr nativeEventPtr)
	{
		if (processEvent != null)
		{
			return processEvent(instanceID, nativeEventPtr);
		}
		return false;
	}

	[RequiredByNativeCode]
	internal static void BeginGUI(int skinMode, int instanceID, int useGUILayout)
	{
		s_SkinMode = skinMode;
		s_OriginalID = instanceID;
		ResetGlobalState();
		if (useGUILayout != 0)
		{
			GUILayoutUtility.Begin(instanceID);
		}
	}

	[RequiredByNativeCode]
	internal static void EndGUI(int layoutType)
	{
		try
		{
			if (Event.current.type == EventType.Layout)
			{
				switch (layoutType)
				{
				case 1:
					GUILayoutUtility.Layout();
					break;
				case 2:
					GUILayoutUtility.LayoutFromEditorWindow();
					break;
				}
			}
			GUILayoutUtility.SelectIDList(s_OriginalID, isWindow: false);
			GUIContent.ClearStaticCache();
		}
		finally
		{
			Internal_ExitGUI();
		}
	}

	[RequiredByNativeCode]
	internal static bool EndGUIFromException(Exception exception)
	{
		Internal_ExitGUI();
		return ShouldRethrowException(exception);
	}

	[RequiredByNativeCode]
	internal static bool EndContainerGUIFromException(Exception exception)
	{
		if (endContainerGUIFromException != null)
		{
			return endContainerGUIFromException(exception);
		}
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void ResetGlobalState()
	{
		GUI.skin = null;
		guiIsExiting = false;
		GUI.changed = false;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static bool IsExitGUIException(Exception exception)
	{
		while (exception is TargetInvocationException && exception.InnerException != null)
		{
			exception = exception.InnerException;
		}
		return exception is ExitGUIException;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static bool ShouldRethrowException(Exception exception)
	{
		return IsExitGUIException(exception);
	}

	internal static void CheckOnGUI()
	{
		if (Internal_GetGUIDepth() <= 0)
		{
			throw new ArgumentException("You can only call GUI functions from inside OnGUI.");
		}
	}
}
