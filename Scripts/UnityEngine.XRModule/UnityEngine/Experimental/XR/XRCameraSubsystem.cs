using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[NativeType(Header = "Modules/XR/Subsystems/Camera/XRCameraSubsystem.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[UsedByNativeCode]
[NativeConditional("ENABLE_XR")]
[NativeHeader("Runtime/Graphics/Texture2D.h")]
public class XRCameraSubsystem : Subsystem<XRCameraSubsystemDescriptor>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<FrameReceivedEventArgs> FrameReceived;

	[RequiredByNativeCode]
	private void InvokeFrameReceivedEvent()
	{
		if (FrameReceived != null)
		{
			FrameReceived(new FrameReceivedEventArgs
			{
				m_CameraSubsystem = this
			});
		}
	}
}
