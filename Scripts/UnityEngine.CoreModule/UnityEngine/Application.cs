using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class Application
{
	public delegate void AdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled, string errorMsg);

	public delegate void LowMemoryCallback();

	public delegate void LogCallback(string condition, string stackTrace, LogType type);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static LowMemoryCallback lowMemory__BackingField;

	private static LogCallback s_LogCallbackHandler;

	private static LogCallback s_LogCallbackHandlerThreaded;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static Func<bool> wantsToQuit;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action quitting;

	public static extern bool isPlaying
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern bool isEditor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[ThreadAndSerializationSafe]
	public static extern RuntimePlatform platform
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern string dataPath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[SecurityCritical]
	public static extern string persistentDataPath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern string temporaryCachePath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern string unityVersion
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern string installerName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern string identifier
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern ApplicationInstallMode installMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern int targetFrameRate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern SystemLanguage systemLanguage
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern NetworkReachability internetReachability
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern bool genuine
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern bool genuineCheckAvailable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static event LowMemoryCallback lowMemory
	{
		add
		{
			LowMemoryCallback lowMemoryCallback = lowMemory__BackingField;
			LowMemoryCallback lowMemoryCallback2;
			do
			{
				lowMemoryCallback2 = lowMemoryCallback;
				lowMemoryCallback = Interlocked.CompareExchange(ref lowMemory__BackingField, (LowMemoryCallback)Delegate.Combine(lowMemoryCallback2, value), lowMemoryCallback);
			}
			while ((object)lowMemoryCallback != lowMemoryCallback2);
		}
		remove
		{
			LowMemoryCallback lowMemoryCallback = lowMemory__BackingField;
			LowMemoryCallback lowMemoryCallback2;
			do
			{
				lowMemoryCallback2 = lowMemoryCallback;
				lowMemoryCallback = Interlocked.CompareExchange(ref lowMemory__BackingField, (LowMemoryCallback)Delegate.Remove(lowMemoryCallback2, value), lowMemoryCallback);
			}
			while ((object)lowMemoryCallback != lowMemoryCallback2);
		}
	}

	public static event LogCallback logMessageReceived
	{
		add
		{
			s_LogCallbackHandler = (LogCallback)Delegate.Combine(s_LogCallbackHandler, value);
			SetLogCallbackDefined(defined: true);
		}
		remove
		{
			s_LogCallbackHandler = (LogCallback)Delegate.Remove(s_LogCallbackHandler, value);
		}
	}

	[RequiredByNativeCode]
	private static void CallLowMemory()
	{
		lowMemory__BackingField?.Invoke();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void Quit();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool RequestAdvertisingIdentifierAsync(AdvertisingIdentifierCallback delegateMethod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void OpenURL(string url);

	[RequiredByNativeCode]
	private static void CallLogCallback(string logString, string stackTrace, LogType type, bool invokedOnMainThread)
	{
		if (invokedOnMainThread)
		{
			s_LogCallbackHandler?.Invoke(logString, stackTrace, type);
		}
		s_LogCallbackHandlerThreaded?.Invoke(logString, stackTrace, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetLogCallbackDefined(bool defined);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern StackTraceLogType GetStackTraceLogType(LogType logType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);

	[RequiredByNativeCode]
	private static bool Internal_ApplicationWantsToQuit()
	{
		if (wantsToQuit != null)
		{
			Delegate[] invocationList = wantsToQuit.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Func<bool> func = (Func<bool>)invocationList[i];
				try
				{
					if (!func())
					{
						return false;
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
		return true;
	}

	[RequiredByNativeCode]
	private static void Internal_ApplicationQuit()
	{
		if (quitting != null)
		{
			quitting();
		}
	}

	[RequiredByNativeCode]
	internal static void InvokeOnBeforeRender()
	{
		BeforeRenderHelper.Invoke();
	}
}
