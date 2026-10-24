using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.CSSLayout;

[NativeHeader("External/CSSLayout/CSSLayout/Native.bindings.h")]
internal static class Native
{
	private static LockDictionary<IntPtr, WeakReference> s_MeasureFunctions = new LockDictionary<IntPtr, WeakReference>();

	public static CSSMeasureFunc CSSNodeGetMeasureFunc(IntPtr node)
	{
		WeakReference cacheItem = null;
		if (s_MeasureFunctions.TryGetValue(node, out cacheItem) && cacheItem.IsAlive)
		{
			return cacheItem.Target as CSSMeasureFunc;
		}
		return null;
	}

	[RequiredByNativeCode]
	public unsafe static void CSSNodeMeasureInvoke(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode, IntPtr returnValueAddress)
	{
		CSSMeasureFunc cSSMeasureFunc = CSSNodeGetMeasureFunc(node);
		if (cSSMeasureFunc != null)
		{
			*(CSSSize*)(void*)returnValueAddress = cSSMeasureFunc(node, width, widthMode, height, heightMode);
		}
	}
}
