using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class Input
{
	private static Gyroscope m_MainGyro = null;

	public static Vector3 mousePosition
	{
		get
		{
			INTERNAL_get_mousePosition(out var value);
			return value;
		}
	}

	public static Vector2 mouseScrollDelta
	{
		get
		{
			INTERNAL_get_mouseScrollDelta(out var value);
			return value;
		}
	}

	public static extern bool mousePresent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern bool anyKeyDown
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern string inputString
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static Touch[] touches
	{
		get
		{
			int num = touchCount;
			Touch[] array = new Touch[num];
			for (int i = 0; i < num; i++)
			{
				ref Touch reference = ref array[i];
				reference = GetTouch(i);
			}
			return array;
		}
	}

	public static extern int touchCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern bool touchSupported
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern DeviceOrientation deviceOrientation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern IMECompositionMode imeCompositionMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern string compositionString
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static Vector2 compositionCursorPos
	{
		get
		{
			INTERNAL_get_compositionCursorPos(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_compositionCursorPos(ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetKeyInt(int key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetKeyUpInt(int key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetKeyDownInt(int key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetKeyDownString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern float GetAxis(string axisName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern float GetAxisRaw(string axisName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool GetButtonDown(string buttonName);

	public static bool GetKey(KeyCode key)
	{
		return GetKeyInt((int)key);
	}

	public static bool GetKeyDown(string name)
	{
		return GetKeyDownString(name);
	}

	public static bool GetKeyDown(KeyCode key)
	{
		return GetKeyDownInt((int)key);
	}

	public static bool GetKeyUp(KeyCode key)
	{
		return GetKeyUpInt((int)key);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool GetMouseButton(int button);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool GetMouseButtonDown(int button);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool GetMouseButtonUp(int button);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_mousePosition(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_mouseScrollDelta(out Vector2 value);

	public static Touch GetTouch(int index)
	{
		INTERNAL_CALL_GetTouch(index, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetTouch(int index, out Touch value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_compositionCursorPos(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_compositionCursorPos(ref Vector2 value);
}
