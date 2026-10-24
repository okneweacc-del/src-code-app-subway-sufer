using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Camera/RenderSettings.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Camera/RenderSettings.h")]
[StaticAccessor("GetRenderSettings()", StaticAccessorType.Dot)]
public sealed class RenderSettings : Object
{
	[NativeProperty("UseFog")]
	public static extern bool fog
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}
}
