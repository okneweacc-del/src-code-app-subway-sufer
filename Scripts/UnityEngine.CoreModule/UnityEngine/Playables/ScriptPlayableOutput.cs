using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[RequiredByNativeCode]
public struct ScriptPlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;
}
