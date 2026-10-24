using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

[StructLayout((LayoutKind)0, Size = 1)]
[RequiredByNativeCode]
public struct EarlyUpdate
{
	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PollPlayerConnection
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ProfilerStartFrame
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PollHtcsPlayerConnection
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct GpuTimestamp
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UnityConnectClientUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct CloudWebServicesUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UnityWebRequestUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ExecuteMainThreadJobs
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ProcessMouseInWindow
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ClearIntermediateRenderers
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ClearLines
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PresentBeforeUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ResetFrameStatsAfterPresent
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateAllUnityWebStreams
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateAsyncReadbackManager
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateTextureStreamingManager
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdatePreloading
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct RendererNotifyInvisible
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PlayerCleanupCachedData
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateMainGameViewRect
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateCanvasRectTransform
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateInputManager
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ProcessRemoteInput
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct XRUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ScriptRunDelayedStartupFrame
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateKinect
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DeliverIosPlatformEvents
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DispatchEventQueueEvents
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorSampleTime
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PhysicsResetInterpolatedTransformPosition
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct NewInputBeginFrame
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct SpriteAtlasManagerUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct TangoUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PerformanceAnalyticsUpdate
	{
	}
}
