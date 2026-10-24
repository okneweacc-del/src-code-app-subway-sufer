using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Modules/ScreenCapture/Public/CaptureScreenshot.h")]
public static class ScreenCapture
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void CaptureScreenshot(string filename, [DefaultValue("1")] int superSize);
}
