using System;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[Serializable]
[RequiredByNativeCode]
public abstract class PlayableBehaviour : IPlayableBehaviour, ICloneable
{
	public PlayableBehaviour()
	{
	}

	public virtual object Clone()
	{
		return MemberwiseClone();
	}
}
