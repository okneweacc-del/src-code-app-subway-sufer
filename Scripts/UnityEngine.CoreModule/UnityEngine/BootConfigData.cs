using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Export/BootConfig.bindings.h")]
internal class BootConfigData
{
	private IntPtr m_Ptr;

	private BootConfigData(IntPtr nativeHandle)
	{
		if (nativeHandle == IntPtr.Zero)
		{
			throw new ArgumentException("native handle can not be null");
		}
		m_Ptr = nativeHandle;
	}

	[RequiredByNativeCode]
	private static BootConfigData WrapBootConfigData(IntPtr nativeHandle)
	{
		return new BootConfigData(nativeHandle);
	}
}
