using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Camera/RenderLoops/MotionVectorRenderLoop.h")]
[NativeHeader("Runtime/Input/GetInput.h")]
[NativeHeader("Runtime/Misc/SystemInfo.h")]
[NativeHeader("Runtime/Shaders/GraphicsCapsScriptBindings.h")]
public sealed class SystemInfo
{
	public static string operatingSystem => GetOperatingSystem();

	public static OperatingSystemFamily operatingSystemFamily => GetOperatingSystemFamily();

	public static string processorType => GetProcessorType();

	public static int processorFrequency => GetProcessorFrequencyMHz();

	public static int processorCount => GetProcessorCount();

	public static int systemMemorySize => GetPhysicalMemoryMB();

	public static string deviceUniqueIdentifier => GetDeviceUniqueIdentifier();

	public static string deviceName => GetDeviceName();

	public static string deviceModel => GetDeviceModel();

	public static string graphicsDeviceName => GetGraphicsDeviceName();

	public static GraphicsDeviceType graphicsDeviceType => GetGraphicsDeviceType();

	public static string graphicsDeviceVersion => GetGraphicsDeviceVersion();

	public static int graphicsShaderLevel => GetGraphicsShaderLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetOperatingSystem")]
	private static extern string GetOperatingSystem();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetOperatingSystemFamily")]
	private static extern OperatingSystemFamily GetOperatingSystemFamily();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorType")]
	private static extern string GetProcessorType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorFrequencyMHz")]
	private static extern int GetProcessorFrequencyMHz();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorCount")]
	private static extern int GetProcessorCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetPhysicalMemoryMB")]
	private static extern int GetPhysicalMemoryMB();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceUniqueIdentifier")]
	private static extern string GetDeviceUniqueIdentifier();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceName")]
	private static extern string GetDeviceName();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceModel")]
	private static extern string GetDeviceModel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceName")]
	private static extern string GetGraphicsDeviceName();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceType")]
	private static extern GraphicsDeviceType GetGraphicsDeviceType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVersion")]
	private static extern string GetGraphicsDeviceVersion();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsShaderLevel")]
	private static extern int GetGraphicsShaderLevel();
}
