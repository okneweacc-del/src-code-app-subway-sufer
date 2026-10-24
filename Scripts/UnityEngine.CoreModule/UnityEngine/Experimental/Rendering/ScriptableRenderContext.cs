using System;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Rendering;

[NativeType("Runtime/Graphics/ScriptableRenderLoop/ScriptableRenderContext.h")]
public struct ScriptableRenderContext
{
	private IntPtr m_Ptr;

	internal ScriptableRenderContext(IntPtr ptr)
	{
		m_Ptr = ptr;
	}
}
