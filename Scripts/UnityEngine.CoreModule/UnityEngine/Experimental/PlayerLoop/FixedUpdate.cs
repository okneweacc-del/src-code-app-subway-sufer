using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

[StructLayout((LayoutKind)0, Size = 1)]
[RequiredByNativeCode]
public struct FixedUpdate
{
	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ClearLines
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorFixedSampleTime
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct AudioFixedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ScriptRunBehaviourFixedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorFixedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct LegacyFixedAnimationUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct XRFixedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PhysicsFixedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct Physics2DFixedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorFixedUpdatePostPhysics
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ScriptRunDelayedFixedFrameRate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct ScriptRunDelayedTasks
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct NewInputBeginFixedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct NewInputEndFixedUpdate
	{
	}
}
