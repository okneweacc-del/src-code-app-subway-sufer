namespace UnityEngine.Experimental.XR;

public struct PlaneRemovedEventArgs
{
	internal XRPlaneSubsystem PlaneSubsystem { set; }

	internal BoundedPlane Plane { set; }
}
