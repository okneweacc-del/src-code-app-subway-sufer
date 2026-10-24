using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[StaticAccessor("GetQualitySettings()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Graphics/QualitySettings.h")]
[NativeHeader("Runtime/Misc/PlayerSettings.h")]
public sealed class QualitySettings : Object
{
	public static extern int pixelLightCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("QualitySettingsNames")]
	public static extern string[] names
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern ColorSpace activeColorSpace
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
		[NativeName("GetColorSpace")]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetCurrentIndex")]
	public static extern int GetQualityLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetCurrentIndex")]
	public static extern void SetQualityLevel(int index, [DefaultValue("true")] bool applyExpensiveChanges);
}
