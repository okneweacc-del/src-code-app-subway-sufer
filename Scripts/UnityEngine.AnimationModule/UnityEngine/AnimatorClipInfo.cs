using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
public struct AnimatorClipInfo
{
	private int m_ClipInstanceID;

	private float m_Weight;
}
