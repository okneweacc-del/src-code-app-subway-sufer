using UnityEngine.Scripting;

namespace UnityEngine.XR;

[UsedByNativeCode]
public struct XRNodeState
{
	private XRNode m_Type;

	private AvailableTrackingData m_AvailableFields;

	private Vector3 m_Position;

	private Quaternion m_Rotation;

	private Vector3 m_Velocity;

	private Vector3 m_AngularVelocity;

	private Vector3 m_Acceleration;

	private Vector3 m_AngularAcceleration;

	private int m_Tracked;

	private ulong m_UniqueID;

	public ulong uniqueID
	{
		set
		{
			m_UniqueID = value;
		}
	}

	public XRNode nodeType
	{
		set
		{
			m_Type = value;
		}
	}

	public bool tracked
	{
		set
		{
			m_Tracked = (value ? 1 : 0);
		}
	}
}
