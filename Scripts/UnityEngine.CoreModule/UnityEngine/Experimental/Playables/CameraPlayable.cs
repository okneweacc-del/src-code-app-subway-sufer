using System;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables;

[StaticAccessor("CameraPlayableBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Camera//Director/CameraPlayable.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Export/Director/CameraPlayable.bindings.h")]
public struct CameraPlayable : IPlayable, IEquatable<CameraPlayable>
{
	private PlayableHandle m_Handle;

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool Equals(CameraPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
