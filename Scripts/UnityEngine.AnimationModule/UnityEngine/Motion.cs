using System.Diagnostics;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Animation/Motion.h")]
public class Motion : Object
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly bool isAnimatorMotion;

	protected Motion()
	{
	}
}
