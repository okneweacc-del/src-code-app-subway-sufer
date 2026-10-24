using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
public struct PlayableGraph
{
	internal IntPtr m_Handle;

	internal int m_Version;
}
