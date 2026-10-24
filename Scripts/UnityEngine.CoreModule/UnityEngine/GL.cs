using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StaticAccessor("GetGfxDevice()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Camera/CameraUtil.h")]
[NativeHeader("Runtime/Camera/Camera.h")]
[NativeHeader("Runtime/GfxDevice/GfxDevice.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
public sealed class GL
{
	[NativeProperty("UserBackfaceMode")]
	public static extern bool invertCulling
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[FreeFunction]
	private static void GLClear(bool clearDepth, bool clearColor, Color backgroundColor, float depth)
	{
		GLClear_Injected(clearDepth, clearColor, ref backgroundColor, depth);
	}

	public static void Clear(bool clearDepth, bool clearColor, Color backgroundColor)
	{
		GLClear(clearDepth, clearColor, backgroundColor, 1f);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GLClear_Injected(bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);
}
