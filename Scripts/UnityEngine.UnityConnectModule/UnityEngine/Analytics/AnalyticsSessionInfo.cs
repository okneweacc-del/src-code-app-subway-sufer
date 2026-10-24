using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics;

[NativeHeader("Modules/UnityConnect/UnityConnectClient.h")]
[NativeHeader("UnityConnectScriptingClasses.h")]
[RequiredByNativeCode]
public static class AnalyticsSessionInfo
{
	public delegate void SessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged);

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static SessionStateChanged sessionStateChanged;

	[RequiredByNativeCode]
	internal static void CallSessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged)
	{
		sessionStateChanged?.Invoke(sessionState, sessionId, sessionElapsedTime, sessionChanged);
	}
}
