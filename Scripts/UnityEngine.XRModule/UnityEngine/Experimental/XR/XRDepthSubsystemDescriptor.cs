using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[NativeType(Header = "Modules/XR/Subsystems/Depth/XRDepthSubsystemDescriptor.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[UsedByNativeCode]
[NativeConditional("ENABLE_XR")]
public class XRDepthSubsystemDescriptor : SubsystemDescriptor<XRDepthSubsystem>
{
}
