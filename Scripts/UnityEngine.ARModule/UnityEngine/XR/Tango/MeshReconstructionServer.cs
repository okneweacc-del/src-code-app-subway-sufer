using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango;

[NativeConditional("PLATFORM_ANDROID")]
[NativeHeader("Runtime/AR/Tango/TangoScriptApi.h")]
[NativeHeader("PhysicsScriptingClasses.h")]
[UsedByNativeCode]
internal class MeshReconstructionServer
{
	internal delegate void SegmentChangedDelegate(GridIndex gridIndex, SegmentChange changeType, double updateTime);

	internal delegate void SegmentReadyDelegate(SegmentGenerationResult generatedSegmentData);

	internal IntPtr m_ServerPtr;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	internal static extern void DestroyThreaded(IntPtr server);

	[RequiredByNativeCode]
	private static void InvokeSegmentChangedEvent(SegmentChangedDelegate onSegmentChanged, GridIndex gridIndex, SegmentChange changeType, double updateTime)
	{
		onSegmentChanged?.Invoke(gridIndex, changeType, updateTime);
	}

	[RequiredByNativeCode]
	private static void InvokeSegmentReadyEvent(SegmentReadyDelegate onSegmentReady, GridIndex gridIndex, MeshFilter meshFilter, MeshCollider meshCollider, bool success, double elapsedTimeSeconds)
	{
		if (onSegmentReady != null)
		{
			SegmentGenerationResult generatedSegmentData = new SegmentGenerationResult
			{
				gridIndex = gridIndex,
				meshFilter = meshFilter,
				meshCollider = meshCollider,
				success = success,
				elapsedTimeSeconds = elapsedTimeSeconds
			};
			onSegmentReady(generatedSegmentData);
		}
	}

	~MeshReconstructionServer()
	{
		if (m_ServerPtr != IntPtr.Zero)
		{
			DestroyThreaded(m_ServerPtr);
			m_ServerPtr = IntPtr.Zero;
			GC.SuppressFinalize(this);
		}
	}
}
