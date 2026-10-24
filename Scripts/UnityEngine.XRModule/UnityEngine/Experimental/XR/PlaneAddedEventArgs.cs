namespace UnityEngine.Experimental.XR;

public struct PlaneAddedEventArgs
{
	internal XRPlaneSubsystem PlaneSubsystem { set; }

	internal BoundedPlane Plane { set; }
}
