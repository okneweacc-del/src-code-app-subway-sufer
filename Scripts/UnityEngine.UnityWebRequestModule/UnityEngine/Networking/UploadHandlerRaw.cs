using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

[StructLayout((LayoutKind)0)]
[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandlerRaw.h")]
public sealed class UploadHandlerRaw : UploadHandler
{
	public UploadHandlerRaw(byte[] data)
	{
		if (data != null && data.Length == 0)
		{
			throw new ArgumentException("Cannot create a data handler without payload data");
		}
		m_Ptr = Create(this, data);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create(UploadHandlerRaw self, byte[] data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetContentType")]
	private extern void InternalSetContentType(string newContentType);

	internal override void SetContentType(string newContentType)
	{
		InternalSetContentType(newContentType);
	}
}
