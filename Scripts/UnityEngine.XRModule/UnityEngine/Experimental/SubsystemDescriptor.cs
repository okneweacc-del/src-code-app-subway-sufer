using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental;

[StructLayout((LayoutKind)0)]
[NativeType(Header = "Modules/XR/XRSubsystemDescriptor.h")]
[UsedByNativeCode("XRSubsystemDescriptor")]
public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptorBase where TSubsystem : Subsystem
{
}
