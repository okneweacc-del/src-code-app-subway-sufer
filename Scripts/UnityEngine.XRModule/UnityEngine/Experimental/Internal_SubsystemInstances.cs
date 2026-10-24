using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental;

internal static class Internal_SubsystemInstances
{
	internal static List<Subsystem> s_SubsystemInstances = new List<Subsystem>();

	[RequiredByNativeCode]
	internal static void Internal_InitializeManagedInstance(IntPtr ptr, Subsystem inst)
	{
		inst.m_Ptr = ptr;
		inst.SetHandle(inst);
		s_SubsystemInstances.Add(inst);
	}

	[RequiredByNativeCode]
	internal static void Internal_ClearManagedInstances()
	{
		foreach (Subsystem s_SubsystemInstance in s_SubsystemInstances)
		{
			s_SubsystemInstance.m_Ptr = IntPtr.Zero;
		}
		s_SubsystemInstances.Clear();
	}

	[RequiredByNativeCode]
	internal static void Internal_RemoveInstanceByPtr(IntPtr ptr)
	{
		for (int num = s_SubsystemInstances.Count - 1; num >= 0; num--)
		{
			if (s_SubsystemInstances[num].m_Ptr == ptr)
			{
				s_SubsystemInstances[num].m_Ptr = IntPtr.Zero;
				s_SubsystemInstances.RemoveAt(num);
			}
		}
	}
}
