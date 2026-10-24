using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

[StructLayout((LayoutKind)0)]
[NativeHeader("Modules/UnityWebRequest/Public/UploadHandler/UploadHandler.h")]
public class UploadHandler : IDisposable
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	public string contentType
	{
		set
		{
			SetContentType(value);
		}
	}

	internal UploadHandler()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern void Release();

	~UploadHandler()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Release();
			m_Ptr = IntPtr.Zero;
		}
	}

	internal virtual void SetContentType(string newContentType)
	{
	}
}
