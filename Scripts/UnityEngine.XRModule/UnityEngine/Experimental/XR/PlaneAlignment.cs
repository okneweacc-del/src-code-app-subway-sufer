using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[UsedByNativeCode]
[Flags]
public enum PlaneAlignment
{
	Horizontal = 1,
	Vertical = 2,
	NonAxis = 4
}
