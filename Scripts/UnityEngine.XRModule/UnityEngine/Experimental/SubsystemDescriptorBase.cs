using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental;

[StructLayout((LayoutKind)0)]
[UsedByNativeCode("XRSubsystemDescriptorBase")]
public class SubsystemDescriptorBase : ISubsystemDescriptor, ISubsystemDescriptorImpl
{
	internal IntPtr m_Ptr;

	IntPtr ISubsystemDescriptorImpl.ptr
	{
		set
		{
			m_Ptr = value;
		}
	}
}
