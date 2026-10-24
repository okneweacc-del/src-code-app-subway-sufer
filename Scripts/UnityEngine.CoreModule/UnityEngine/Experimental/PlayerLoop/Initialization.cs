using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

[StructLayout((LayoutKind)0, Size = 1)]
[RequiredByNativeCode]
public struct Initialization
{
	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PlayerUpdateTime
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct AsyncUploadTimeSlicedUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct SynchronizeState
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct SynchronizeInputs
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct XREarlyUpdate
	{
	}
}
