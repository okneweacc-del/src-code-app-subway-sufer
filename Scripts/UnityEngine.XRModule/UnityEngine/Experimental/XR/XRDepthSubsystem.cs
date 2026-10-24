using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[NativeHeader("Modules/XR/XRPrefix.h")]
[UsedByNativeCode]
[NativeHeader("Modules/XR/Subsystems/Depth/XRDepthSubsystem.h")]
[NativeConditional("ENABLE_XR")]
public class XRDepthSubsystem : Subsystem<XRDepthSubsystemDescriptor>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private Action<PointCloudUpdatedEventArgs> PointCloudUpdated;

	[RequiredByNativeCode]
	private void InvokePointCloudUpdatedEvent()
	{
		if (PointCloudUpdated != null)
		{
			PointCloudUpdated(new PointCloudUpdatedEventArgs
			{
				m_DepthSubsystem = this
			});
		}
	}
}
