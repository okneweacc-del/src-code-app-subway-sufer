using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[NativeHeader("UnityConnectScriptingClasses.h")]
[NativeHeader("Modules/UnityConnect/RemoteSettings.h")]
[ExcludeFromDocs]
public class RemoteConfigSettings : IDisposable
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<bool> Updated;

	~RemoteConfigSettings()
	{
		Destroy();
	}

	private void Destroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	public void Dispose()
	{
		Destroy();
		GC.SuppressFinalize(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	internal static extern void Internal_Destroy(IntPtr ptr);

	[RequiredByNativeCode]
	internal static void RemoteConfigSettingsUpdated(RemoteConfigSettings rcs, bool wasLastUpdatedFromServer)
	{
		rcs.Updated?.Invoke(wasLastUpdatedFromServer);
	}
}
