using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Debug.bindings.h")]
public class Debug
{
	internal static ILogger s_Logger = new Logger(new DebugLogHandler());

	public static ILogger unityLogger => s_Logger;

	[StaticAccessor("GetBuildSettings()", StaticAccessorType.Dot)]
	[NativeProperty(TargetType = TargetType.Field)]
	public static extern bool isDebugBuild
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[ExcludeFromDocs]
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		bool depthTest = true;
		DrawLine(start, end, color, duration, depthTest);
	}

	[ExcludeFromDocs]
	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		bool depthTest = true;
		float duration = 0f;
		DrawLine(start, end, color, duration, depthTest);
	}

	[FreeFunction("DebugDrawLine")]
	public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
	{
		DrawLine_Injected(ref start, ref end, ref color, duration, depthTest);
	}

	[ExcludeFromDocs]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
		bool depthTest = true;
		DrawRay(start, dir, color, duration, depthTest);
	}

	public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
	{
		DrawLine(start, start + dir, color, duration, depthTest);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PauseEditor")]
	public static extern void Break();

	public static void Log(object message)
	{
		unityLogger.Log(LogType.Log, message);
	}

	public static void Log(object message, Object context)
	{
		unityLogger.Log(LogType.Log, message, context);
	}

	public static void LogFormat(string format, params object[] args)
	{
		unityLogger.LogFormat(LogType.Log, format, args);
	}

	public static void LogFormat(Object context, string format, params object[] args)
	{
		unityLogger.LogFormat(LogType.Log, context, format, args);
	}

	public static void LogError(object message)
	{
		unityLogger.Log(LogType.Error, message);
	}

	public static void LogError(object message, Object context)
	{
		unityLogger.Log(LogType.Error, message, context);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		unityLogger.LogFormat(LogType.Error, format, args);
	}

	public static void LogErrorFormat(Object context, string format, params object[] args)
	{
		unityLogger.LogFormat(LogType.Error, context, format, args);
	}

	public static void LogException(Exception exception)
	{
		unityLogger.LogException(exception, null);
	}

	public static void LogException(Exception exception, Object context)
	{
		unityLogger.LogException(exception, context);
	}

	public static void LogWarning(object message)
	{
		unityLogger.Log(LogType.Warning, message);
	}

	public static void LogWarning(object message, Object context)
	{
		unityLogger.Log(LogType.Warning, message, context);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		unityLogger.LogFormat(LogType.Warning, format, args);
	}

	public static void LogWarningFormat(Object context, string format, params object[] args)
	{
		unityLogger.LogFormat(LogType.Warning, context, format, args);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DrawLine_Injected(ref Vector3 start, ref Vector3 end, [DefaultValue("Color.white")] ref Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest);
}
