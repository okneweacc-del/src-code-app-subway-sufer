using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public class TextAsset : Object
{
	public extern string text
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern byte[] bytes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public override string ToString()
	{
		return text;
	}
}
