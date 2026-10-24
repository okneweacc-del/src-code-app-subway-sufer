using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

[StructLayout((LayoutKind)0, Size = 1)]
[RequiredByNativeCode]
public struct PreLateUpdate
{
	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct AIUpdatePostScript
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorUpdateAnimationBegin
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct LegacyAnimationUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorUpdateAnimationEnd
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorDeferredEvaluate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateNetworkManager
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateMasterServerInterface
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UNetUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct EndGraphicsJobsLate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ParticleSystemBeginUpdateAll
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ScriptRunBehaviourLateUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ConstraintManagerUpdate
	{
	}
}
