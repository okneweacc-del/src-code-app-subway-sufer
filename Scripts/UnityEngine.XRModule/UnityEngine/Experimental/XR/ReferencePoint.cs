using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
[NativeHeader("Modules/XR/Subsystems/ReferencePoints/XRManagedReferencePoint.h")]
[UsedByNativeCode]
public struct ReferencePoint
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TrackableId Id;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TrackingState TrackingState;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Pose Pose;
}
