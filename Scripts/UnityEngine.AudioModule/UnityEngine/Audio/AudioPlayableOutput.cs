using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[RequiredByNativeCode]
[StaticAccessor("AudioPlayableOutputBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Modules/Audio/Public/AudioSource.h")]
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioPlayableOutput.bindings.h")]
[NativeHeader("Modules/Audio/Public/Director/AudioPlayableOutput.h")]
public struct AudioPlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;
}
