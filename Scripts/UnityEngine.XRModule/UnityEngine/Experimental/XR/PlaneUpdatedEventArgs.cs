namespace UnityEngine.Experimental.XR;

public struct PlaneUpdatedEventArgs
{
	internal XRPlaneSubsystem PlaneSubsystem { set; }

	internal BoundedPlane Plane { set; }
}
