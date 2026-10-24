using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/UnityConnect/RemoteSettings.h")]
[NativeHeader("UnityConnectScriptingClasses.h")]
public static class RemoteSettings
{
	public delegate void UpdatedEventHandler();

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static UpdatedEventHandler Updated;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static Action BeforeFetchFromServer;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<bool, bool, int> Completed;

	[RequiredByNativeCode]
	internal static void RemoteSettingsUpdated(bool wasLastUpdatedFromServer)
	{
		Updated?.Invoke();
	}

	[RequiredByNativeCode]
	internal static void RemoteSettingsBeforeFetchFromServer()
	{
		BeforeFetchFromServer?.Invoke();
	}

	[RequiredByNativeCode]
	internal static void RemoteSettingsUpdateCompleted(bool wasLastUpdatedFromServer, bool settingsChanged, int response)
	{
		Completed?.Invoke(wasLastUpdatedFromServer, settingsChanged, response);
	}
}
