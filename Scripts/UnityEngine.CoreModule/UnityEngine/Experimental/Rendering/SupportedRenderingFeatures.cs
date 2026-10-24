using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

public class SupportedRenderingFeatures
{
	[Flags]
	public enum ReflectionProbeSupportFlags
	{
		None = 0,
		Rotation = 1
	}

	[Flags]
	public enum LightmapMixedBakeMode
	{
		None = 0,
		IndirectOnly = 1,
		Subtractive = 2,
		Shadowmask = 4
	}

	private static SupportedRenderingFeatures s_Active = new SupportedRenderingFeatures();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ReflectionProbeSupportFlags reflectionProbeSupportFlags = ReflectionProbeSupportFlags.None;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool rendererSupportsLightProbeProxyVolumes = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool rendererSupportsMotionVectors = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool rendererSupportsReceiveShadows = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool rendererSupportsReflectionProbes = true;

	public static SupportedRenderingFeatures active
	{
		get
		{
			if (s_Active == null)
			{
				s_Active = new SupportedRenderingFeatures();
			}
			return s_Active;
		}
	}

	public LightmapMixedBakeMode defaultMixedLightingMode { get; } = LightmapMixedBakeMode.None;

	public LightmapMixedBakeMode supportedMixedLightingModes { get; } = LightmapMixedBakeMode.IndirectOnly | LightmapMixedBakeMode.Subtractive | LightmapMixedBakeMode.Shadowmask;

	public LightmapBakeType supportedLightmapBakeTypes { get; } = LightmapBakeType.Realtime | LightmapBakeType.Baked | LightmapBakeType.Mixed;

	public LightmapsMode supportedLightmapsModes { get; } = LightmapsMode.CombinedDirectional;

	[RequiredByNativeCode]
	internal unsafe static void FallbackMixedLightingModeByRef(IntPtr fallbackModePtr)
	{
		MixedLightingMode* ptr = (MixedLightingMode*)(void*)fallbackModePtr;
		if (active.defaultMixedLightingMode != LightmapMixedBakeMode.None && (active.supportedMixedLightingModes & active.defaultMixedLightingMode) == active.defaultMixedLightingMode)
		{
			switch (active.defaultMixedLightingMode)
			{
			case LightmapMixedBakeMode.Shadowmask:
				*ptr = MixedLightingMode.Shadowmask;
				break;
			case LightmapMixedBakeMode.Subtractive:
				*ptr = MixedLightingMode.Subtractive;
				break;
			default:
				*ptr = MixedLightingMode.IndirectOnly;
				break;
			}
		}
		else if (IsMixedLightingModeSupported(MixedLightingMode.Shadowmask))
		{
			*ptr = MixedLightingMode.Shadowmask;
		}
		else if (IsMixedLightingModeSupported(MixedLightingMode.Subtractive))
		{
			*ptr = MixedLightingMode.Subtractive;
		}
		else
		{
			*ptr = MixedLightingMode.IndirectOnly;
		}
	}

	internal unsafe static bool IsMixedLightingModeSupported(MixedLightingMode mixedMode)
	{
		bool result = default;
		IsMixedLightingModeSupportedByRef(mixedMode, new IntPtr(&result));
		return result;
	}

	[RequiredByNativeCode]
	internal unsafe static void IsMixedLightingModeSupportedByRef(MixedLightingMode mixedMode, IntPtr isSupportedPtr)
	{
		bool* ptr = (bool*)(void*)isSupportedPtr;
		if (!IsLightmapBakeTypeSupported(LightmapBakeType.Mixed))
		{
			*ptr = false;
		}
		else
		{
			*ptr = (mixedMode == MixedLightingMode.IndirectOnly && (active.supportedMixedLightingModes & LightmapMixedBakeMode.IndirectOnly) == LightmapMixedBakeMode.IndirectOnly) || (mixedMode == MixedLightingMode.Subtractive && (active.supportedMixedLightingModes & LightmapMixedBakeMode.Subtractive) == LightmapMixedBakeMode.Subtractive) || (mixedMode == MixedLightingMode.Shadowmask && (active.supportedMixedLightingModes & LightmapMixedBakeMode.Shadowmask) == LightmapMixedBakeMode.Shadowmask);
		}
	}

	internal unsafe static bool IsLightmapBakeTypeSupported(LightmapBakeType bakeType)
	{
		bool result = default;
		IsLightmapBakeTypeSupportedByRef(bakeType, new IntPtr(&result));
		return result;
	}

	[RequiredByNativeCode]
	internal unsafe static void IsLightmapBakeTypeSupportedByRef(LightmapBakeType bakeType, IntPtr isSupportedPtr)
	{
		bool* ptr = (bool*)(void*)isSupportedPtr;
		if (bakeType == LightmapBakeType.Mixed && (!IsLightmapBakeTypeSupported(LightmapBakeType.Baked) || active.supportedMixedLightingModes == LightmapMixedBakeMode.None))
		{
			*ptr = false;
		}
		else
		{
			*ptr = (active.supportedLightmapBakeTypes & bakeType) == bakeType;
		}
	}

	[RequiredByNativeCode]
	internal unsafe static void IsLightmapsModeSupportedByRef(LightmapsMode mode, IntPtr isSupportedPtr)
	{
		bool* ptr = (bool*)(void*)isSupportedPtr;
		*ptr = (active.supportedLightmapsModes & mode) == mode;
	}
}
