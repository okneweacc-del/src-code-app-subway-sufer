using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[UsedByNativeCode]
[NativeHeader("Modules/XR/Subsystems/ReferencePoints/XRReferencePointSubsystem.h")]
[NativeConditional("ENABLE_XR")]
[NativeHeader("Modules/XR/XRPrefix.h")]
public class XRReferencePointSubsystem : Subsystem<XRReferencePointSubsystemDescriptor>
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<ReferencePointUpdatedEventArgs> ReferencePointUpdated;

	[RequiredByNativeCode]
	private void InvokeReferencePointUpdatedEvent(ReferencePoint updatedReferencePoint, TrackingState previousTrackingState, Pose previousPose)
	{
		if (ReferencePointUpdated != null)
		{
			ReferencePointUpdated(new ReferencePointUpdatedEventArgs
			{
				ReferencePoint = updatedReferencePoint,
				PreviousTrackingState = previousTrackingState,
				PreviousPose = previousPose
			});
		}
	}
}
