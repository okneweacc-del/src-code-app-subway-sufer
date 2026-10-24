using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

[StructLayout((LayoutKind)0)]
[NativeHeader("Modules/UnityWebRequest/Public/DownloadHandler/DownloadHandler.h")]
public class DownloadHandler : IDisposable
{
	[NonSerialized]
	[VisibleToOtherModules]
	internal IntPtr m_Ptr;

	public byte[] data => GetData();

	public string text => GetText();

	[VisibleToOtherModules]
	internal DownloadHandler()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern void Release();

	~DownloadHandler()
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

	protected virtual byte[] GetData()
	{
		return null;
	}

	protected virtual string GetText()
	{
		byte[] array = GetData();
		if (array != null && array.Length > 0)
		{
			return GetTextEncoder().GetString(array, 0, array.Length);
		}
		return "";
	}

	private Encoding GetTextEncoder()
	{
		string contentType = GetContentType();
		if (!string.IsNullOrEmpty(contentType))
		{
			int num = contentType.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
			if (num > -1)
			{
				int num2 = contentType.IndexOf('=', num);
				if (num2 > -1)
				{
					string text = contentType.Substring(num2 + 1).Trim().Trim('\'', '"')
						.Trim();
					int num3 = text.IndexOf(';');
					if (num3 > -1)
					{
						text = text.Substring(0, num3);
					}
					try
					{
						return Encoding.GetEncoding(text);
					}
					catch (ArgumentException ex)
					{
						Debug.LogWarning($"Unsupported encoding '{text}': {ex.Message}");
					}
				}
			}
		}
		return Encoding.UTF8;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern string GetContentType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules]
	[NativeThrows]
	internal static extern byte[] InternalGetByteArray(DownloadHandler dh);
}
