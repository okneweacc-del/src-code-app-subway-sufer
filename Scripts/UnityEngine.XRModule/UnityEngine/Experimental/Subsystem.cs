using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental;

[StructLayout((LayoutKind)0)]
[NativeType(Header = "Modules/XR/XRSubsystem.h")]
[UsedByNativeCode]
public class Subsystem
{
	internal IntPtr m_Ptr;

	internal ISubsystemDescriptor m_subsystemDescriptor;

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SetHandle(Subsystem inst);
}
[UsedByNativeCode("XRSubsystem_TXRSubsystemDescriptor")]
public class Subsystem<TSubsystemDescriptor> : Subsystem where TSubsystemDescriptor : ISubsystemDescriptor
{
}
