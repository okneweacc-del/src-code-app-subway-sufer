using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables;

[NativeHeader("Runtime/Graphics/RenderTexture.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Export/Director/TexturePlayableOutput.bindings.h")]
[StaticAccessor("TexturePlayableOutputBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Graphics/Director/TexturePlayableOutput.h")]
public struct TexturePlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;
}
