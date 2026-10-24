using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[NativeConditional("ENABLE_XR")]
[NativeHeader("XRScriptingClasses.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[UsedByNativeCode]
[NativeHeader("Modules/XR/Subsystems/Planes/XRBoundedPlane.h")]
public struct BoundedPlane
{
	private uint m_InstanceId;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TrackableId Id;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TrackableId SubsumedById;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Pose Pose;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Vector3 Center;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Vector2 Size;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PlaneAlignment Alignment;
}
