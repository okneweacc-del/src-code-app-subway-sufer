using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Input;

[NativeConditional("ENABLE_NEW_INPUT_SYSTEM")]
[NativeHeader("Modules/Input/Private/InputInternal.h")]
[NativeHeader("Modules/Input/Private/InputModuleBindings.h")]
public class NativeInputSystem
{
	public static NativeUpdateCallback onUpdate;

	public static NativeEventCallback onEvents;

	private static NativeDeviceDiscoveredCallback s_OnDeviceDiscoveredCallback;

	public static extern bool hasDeviceDiscoveredCallback
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	static NativeInputSystem()
	{
		hasDeviceDiscoveredCallback = false;
	}

	[RequiredByNativeCode]
	internal static void NotifyUpdate(NativeInputUpdateType updateType)
	{
		onUpdate?.Invoke(updateType);
	}

	[RequiredByNativeCode]
	internal static void NotifyEvents(int eventCount, IntPtr eventData)
	{
		onEvents?.Invoke(eventCount, eventData);
	}

	[RequiredByNativeCode]
	internal static void NotifyDeviceDiscovered(NativeInputDeviceInfo deviceInfo)
	{
		s_OnDeviceDiscoveredCallback?.Invoke(deviceInfo);
	}
}
