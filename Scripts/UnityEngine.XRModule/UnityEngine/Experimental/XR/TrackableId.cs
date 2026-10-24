using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[NativeHeader("Modules/XR/XRManagedBindings.h")]
[UsedByNativeCode]
public struct TrackableId
{
	private static TrackableId s_InvalidId = default;

	private ulong m_SubId1;

	private ulong m_SubId2;

	public override string ToString()
	{
		return string.Format("{0}-{1}", m_SubId1.ToString("X16"), m_SubId2.ToString("X16"));
	}

	public override int GetHashCode()
	{
		return m_SubId1.GetHashCode() ^ m_SubId2.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj is TrackableId trackableId)
		{
			return m_SubId1 == trackableId.m_SubId1 && m_SubId2 == trackableId.m_SubId2;
		}
		return false;
	}
}
