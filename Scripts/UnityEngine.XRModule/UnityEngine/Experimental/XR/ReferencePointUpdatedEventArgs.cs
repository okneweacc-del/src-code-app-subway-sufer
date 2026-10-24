using UnityEngine.Bindings;

namespace UnityEngine.Experimental.XR;

[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
public struct ReferencePointUpdatedEventArgs
{
	internal ReferencePoint ReferencePoint { set; }

	internal TrackingState PreviousTrackingState { set; }

	internal Pose PreviousPose { set; }
}
