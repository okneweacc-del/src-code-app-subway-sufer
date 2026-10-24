using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[StaticAccessor("GetScreenManager()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Graphics/ScreenManager.h")]
public sealed class Screen
{
	public static extern int width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetWidth", IsThreadSafe = true)]
		get;
	}

	public static extern int height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetHeight", IsThreadSafe = true)]
		get;
	}

	public static extern float dpi
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDPI")]
		get;
	}

	public static ScreenOrientation orientation
	{
		set
		{
			if (value == ScreenOrientation.Unknown)
			{
				Debug.Log("ScreenOrientation.Unknown is deprecated. Please use ScreenOrientation.AutoRotation");
				value = ScreenOrientation.AutoRotation;
			}
			RequestOrientation(value);
		}
	}

	[NativeProperty("ScreenTimeout")]
	public static extern int sleepTimeout
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static bool autorotateToPortrait
	{
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToPortrait, value);
		}
	}

	public static bool autorotateToPortraitUpsideDown
	{
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToPortraitUpsideDown, value);
		}
	}

	public static bool autorotateToLandscapeLeft
	{
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeLeft, value);
		}
	}

	public static bool autorotateToLandscapeRight
	{
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeRight, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RequestOrientation(ScreenOrientation orient);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetIsOrientationEnabled")]
	private static extern void SetOrientationEnabled(EnabledOrientation orient, bool enabled);
}
