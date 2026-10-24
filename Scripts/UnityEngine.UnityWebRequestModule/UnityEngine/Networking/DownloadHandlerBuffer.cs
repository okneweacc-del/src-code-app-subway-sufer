using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

[StructLayout((LayoutKind)0)]
[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandlerBuffer.h")]
public sealed class DownloadHandlerBuffer : DownloadHandler
{
	public DownloadHandlerBuffer()
	{
		InternalCreateBuffer();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create(DownloadHandlerBuffer obj);

	private void InternalCreateBuffer()
	{
		m_Ptr = Create(this);
	}

	protected override byte[] GetData()
	{
		return InternalGetData();
	}

	private byte[] InternalGetData()
	{
		return DownloadHandler.InternalGetByteArray(this);
	}
}
