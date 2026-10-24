using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[UsedByNativeCode]
[NativeHeader("Modules/XR/Subsystems/Planes/XRPlaneSubsystem.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[NativeConditional("ENABLE_XR")]
public class XRPlaneSubsystem : Subsystem<XRPlaneSubsystemDescriptor>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<PlaneAddedEventArgs> PlaneAdded;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<PlaneUpdatedEventArgs> PlaneUpdated;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<PlaneRemovedEventArgs> PlaneRemoved;

	[RequiredByNativeCode]
	private void InvokePlaneAddedEvent(BoundedPlane plane)
	{
		if (PlaneAdded != null)
		{
			PlaneAdded(new PlaneAddedEventArgs
			{
				PlaneSubsystem = this,
				Plane = plane
			});
		}
	}

	[RequiredByNativeCode]
	private void InvokePlaneUpdatedEvent(BoundedPlane plane)
	{
		if (PlaneUpdated != null)
		{
			PlaneUpdated(new PlaneUpdatedEventArgs
			{
				PlaneSubsystem = this,
				Plane = plane
			});
		}
	}

	[RequiredByNativeCode]
	private void InvokePlaneRemovedEvent(BoundedPlane removedPlane)
	{
		if (PlaneRemoved != null)
		{
			PlaneRemoved(new PlaneRemovedEventArgs
			{
				PlaneSubsystem = this,
				Plane = removedPlane
			});
		}
	}
}
