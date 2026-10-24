using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

[StructLayout((LayoutKind)0, Size = 1)]
[RequiredByNativeCode]
public struct PreUpdate
{
	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct PhysicsUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct Physics2DUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct CheckTexFieldInput
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct IMGUISendQueuedEvents
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct SendMouseEvents
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct AIUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct WindUpdate
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateVideo
	{
	}

	[StructLayout((LayoutKind)0, Size = 1)]
	[RequiredByNativeCode]
	public struct NewInputUpdate
	{
	}
}
