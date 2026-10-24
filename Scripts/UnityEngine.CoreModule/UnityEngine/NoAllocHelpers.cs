using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

internal sealed class NoAllocHelpers
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern Array ExtractArrayFromList(object list);

	public static int SafeLength(Array values)
	{
		return values?.Length ?? 0;
	}

	public static int SafeLength<T>(List<T> values)
	{
		return values?.Count ?? 0;
	}
}
