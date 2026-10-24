using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[UsedByNativeCode]
[NativeType(Header = "Modules/XR/Subsystems/Camera/XRCameraSubsystemDescriptor.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[NativeConditional("ENABLE_XR")]
public class XRCameraSubsystemDescriptor : SubsystemDescriptor<XRCameraSubsystem>
{
}
