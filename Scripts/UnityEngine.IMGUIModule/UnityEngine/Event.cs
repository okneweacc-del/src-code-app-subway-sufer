using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
public sealed class Event
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	private static Event s_Current;

	private static Event s_MasterEvent;

	public extern EventType rawType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern EventType type
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern EventModifiers modifiers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern char character
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern string commandName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern KeyCode keyCode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int displayIndex
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public Vector2 mousePosition
	{
		get
		{
			Internal_GetMousePosition(out var value);
			return value;
		}
	}

	public Vector2 delta
	{
		get
		{
			Internal_GetMouseDelta(out var value);
			return value;
		}
	}

	public static Event current => s_Current;

	public bool isKey
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.KeyDown || eventType == EventType.KeyUp;
		}
	}

	public bool isMouse
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.MouseMove || eventType == EventType.MouseDown || eventType == EventType.MouseUp || eventType == EventType.MouseDrag || eventType == EventType.ContextClick || eventType == EventType.MouseEnterWindow || eventType == EventType.MouseLeaveWindow;
		}
	}

	public Event()
	{
		Init(0);
	}

	public Event(int displayIndex)
	{
		Init(displayIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void Init(int displayIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void Cleanup();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern EventType GetTypeForControl(int controlID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_GetMousePosition(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_GetMouseDelta(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetNativeEvent(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_Use();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool PopEvent(Event outEvent);

	~Event()
	{
		Cleanup();
	}

	[RequiredByNativeCode]
	private static void Internal_MakeMasterEventCurrent(int displayIndex)
	{
		if (s_MasterEvent == null)
		{
			s_MasterEvent = new Event(displayIndex);
		}
		s_MasterEvent.displayIndex = displayIndex;
		s_Current = s_MasterEvent;
		Internal_SetNativeEvent(s_MasterEvent.m_Ptr);
	}

	public override int GetHashCode()
	{
		int num = 1;
		if (isKey)
		{
			num = (ushort)keyCode;
		}
		if (isMouse)
		{
			num = mousePosition.GetHashCode();
		}
		return (num * 37) | (int)modifiers;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (object.ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		Event obj2 = (Event)obj;
		if (type != obj2.type || (modifiers & ~EventModifiers.CapsLock) != (obj2.modifiers & ~EventModifiers.CapsLock))
		{
			return false;
		}
		if (isKey)
		{
			return keyCode == obj2.keyCode;
		}
		if (isMouse)
		{
			return mousePosition == obj2.mousePosition;
		}
		return false;
	}

	public override string ToString()
	{
		if (isKey)
		{
			if (character == '\0')
			{
				return UnityString.Format("Event:{0}   Character:\\0   Modifiers:{1}   KeyCode:{2}", type, modifiers, keyCode);
			}
			return string.Concat("Event:", type, "   Character:", (int)character, "   Modifiers:", modifiers, "   KeyCode:", keyCode);
		}
		if (isMouse)
		{
			return UnityString.Format("Event: {0}   Position: {1} Modifiers: {2}", type, mousePosition, modifiers);
		}
		if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
		{
			return UnityString.Format("Event: {0}  \"{1}\"", type, commandName);
		}
		return "" + type;
	}

	public void Use()
	{
		if (type == EventType.Repaint || type == EventType.Layout)
		{
			Debug.LogWarning(UnityString.Format("Event.Use() should not be called for events of type {0}", type));
		}
		Internal_Use();
	}
}
