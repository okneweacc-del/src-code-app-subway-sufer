using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[NativeHeader("Modules/XR/XRPrefix.h")]
[NativeConditional("ENABLE_XR")]
[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
[UsedByNativeCode]
public class XRSessionSubsystem : Subsystem<XRSessionSubsystemDescriptor>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<SessionTrackingStateChangedEventArgs> TrackingStateChanged;

	[RequiredByNativeCode]
	private void InvokeTrackingStateChangedEvent(TrackingState newState)
	{
		if (TrackingStateChanged != null)
		{
			TrackingStateChanged(new SessionTrackingStateChangedEventArgs
			{
				m_Session = this,
				NewState = newState
			});
		}
	}
}
