using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Events;

[Serializable]
internal class PersistentCallGroup
{
	[SerializeField]
	[FormerlySerializedAs("m_Listeners")]
	private List<PersistentCall> m_Calls;

	public PersistentCallGroup()
	{
		m_Calls = new List<PersistentCall>();
	}

	public void Initialize(InvokableCallList invokableList, UnityEventBase unityEventBase)
	{
		foreach (PersistentCall call in m_Calls)
		{
			if (call.IsValid())
			{
				BaseInvokableCall runtimeCall = call.GetRuntimeCall(unityEventBase);
				if (runtimeCall != null)
				{
					invokableList.AddPersistentInvokableCall(runtimeCall);
				}
			}
		}
	}
}
