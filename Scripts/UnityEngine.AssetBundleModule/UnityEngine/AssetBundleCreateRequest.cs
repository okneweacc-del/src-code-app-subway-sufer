using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public sealed class AssetBundleCreateRequest : AsyncOperation
{
	public extern AssetBundle assetBundle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}
}
