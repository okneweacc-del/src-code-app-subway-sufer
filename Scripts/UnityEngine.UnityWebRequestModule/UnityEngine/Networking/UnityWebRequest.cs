using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine.Networking;

[StructLayout((LayoutKind)0)]
[NativeHeader("Modules/UnityWebRequest/Public/UnityWebRequest.h")]
public class UnityWebRequest : IDisposable
{
	internal enum UnityWebRequestMethod
	{
		Get = 0,
		Post = 1,
		Put = 2,
		Head = 3,
		Custom = 4
	}

	internal enum UnityWebRequestError
	{
		OK = 0,
		Unknown = 1,
		SDKError = 2,
		UnsupportedProtocol = 3,
		MalformattedUrl = 4,
		CannotResolveProxy = 5,
		CannotResolveHost = 6,
		CannotConnectToHost = 7,
		AccessDenied = 8,
		GenericHttpError = 9,
		WriteError = 10,
		ReadError = 11,
		OutOfMemory = 12,
		Timeout = 13,
		HTTPPostError = 14,
		SSLCannotConnect = 15,
		Aborted = 16,
		TooManyRedirects = 17,
		ReceivedNoData = 18,
		SSLNotSupported = 19,
		FailedToSendData = 20,
		FailedToReceiveData = 21,
		SSLCertificateError = 22,
		SSLCipherNotAvailable = 23,
		SSLCACertError = 24,
		UnrecognizedContentEncoding = 25,
		LoginFailed = 26,
		SSLShutdownFailed = 27,
		NoInternetConnection = 28
	}

	[NonSerialized]
	internal IntPtr m_Ptr;

	[NonSerialized]
	internal DownloadHandler m_DownloadHandler;

	[NonSerialized]
	internal UploadHandler m_UploadHandler;

	[NonSerialized]
	internal CertificateHandler m_CertificateHandler;

	[NonSerialized]
	internal Uri m_Uri;

	public bool disposeCertificateHandlerOnDispose { get; set; }

	public bool disposeDownloadHandlerOnDispose { get; set; }

	public bool disposeUploadHandlerOnDispose { get; set; }

	public string method
	{
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Cannot set a UnityWebRequest's method to an empty or null string");
			}
			switch (value.ToUpper())
			{
			case "GET":
				InternalSetMethod(UnityWebRequestMethod.Get);
				break;
			case "POST":
				InternalSetMethod(UnityWebRequestMethod.Post);
				break;
			case "PUT":
				InternalSetMethod(UnityWebRequestMethod.Put);
				break;
			case "HEAD":
				InternalSetMethod(UnityWebRequestMethod.Head);
				break;
			default:
				InternalSetCustomMethod(value.ToUpper());
				break;
			}
		}
	}

	public string error
	{
		get
		{
			if (!isNetworkError && !isHttpError)
			{
				return null;
			}
			return GetWebErrorString(GetError());
		}
	}

	public string url
	{
		get
		{
			return GetUrl();
		}
		set
		{
			string localUrl = "http://localhost/";
			InternalSetUrl(WebRequestUtils.MakeInitialUrl(value, localUrl));
		}
	}

	public extern long responseCode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern bool isModifiable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsModifiable")]
		get;
	}

	public extern bool isDone
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsDone")]
		get;
	}

	public extern bool isNetworkError
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsNetworkError")]
		get;
	}

	public extern bool isHttpError
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsHttpError")]
		get;
	}

	public int redirectLimit
	{
		set
		{
			SetRedirectLimitFromScripting(value);
		}
	}

	public bool chunkedTransfer
	{
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its chunked transfer encoding setting cannot be altered");
			}
			UnityWebRequestError unityWebRequestError = SetChunked(value);
			if (unityWebRequestError != UnityWebRequestError.OK)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
		}
	}

	public UploadHandler uploadHandler
	{
		get
		{
			return m_UploadHandler;
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the upload handler");
			}
			UnityWebRequestError unityWebRequestError = SetUploadHandler(value);
			if (unityWebRequestError != UnityWebRequestError.OK)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
			m_UploadHandler = value;
		}
	}

	public DownloadHandler downloadHandler
	{
		get
		{
			return m_DownloadHandler;
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the download handler");
			}
			UnityWebRequestError unityWebRequestError = SetDownloadHandler(value);
			if (unityWebRequestError != UnityWebRequestError.OK)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
			m_DownloadHandler = value;
		}
	}

	public CertificateHandler certificateHandler => m_CertificateHandler;

	public UnityWebRequest(string url, string method)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.url = url;
		this.method = method;
	}

	public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.url = url;
		this.method = method;
		this.downloadHandler = downloadHandler;
		this.uploadHandler = uploadHandler;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("ENABLE_UNITYWEBREQUEST")]
	[NativeMethod(IsThreadSafe = true)]
	private static extern string GetWebErrorString(UnityWebRequestError err);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern IntPtr Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern void Release();

	internal void InternalDestroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Abort();
			Release();
			m_Ptr = IntPtr.Zero;
		}
	}

	private void InternalSetDefaults()
	{
		disposeDownloadHandlerOnDispose = true;
		disposeUploadHandlerOnDispose = true;
		disposeCertificateHandlerOnDispose = true;
	}

	~UnityWebRequest()
	{
		DisposeHandlers();
		InternalDestroy();
	}

	public void Dispose()
	{
		DisposeHandlers();
		InternalDestroy();
		GC.SuppressFinalize(this);
	}

	private void DisposeHandlers()
	{
		if (disposeDownloadHandlerOnDispose)
		{
			downloadHandler?.Dispose();
		}
		if (disposeUploadHandlerOnDispose)
		{
			uploadHandler?.Dispose();
		}
		if (disposeCertificateHandlerOnDispose)
		{
			certificateHandler?.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal extern UnityWebRequestAsyncOperation BeginWebRequest();

	[Obsolete("Use SendWebRequest.  It returns a UnityWebRequestAsyncOperation which contains a reference to the WebRequest object.", false)]
	public AsyncOperation Send()
	{
		return SendWebRequest();
	}

	public UnityWebRequestAsyncOperation SendWebRequest()
	{
		UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = BeginWebRequest();
		if (unityWebRequestAsyncOperation != null)
		{
			unityWebRequestAsyncOperation.webRequest = this;
		}
		return unityWebRequestAsyncOperation;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	public extern void Abort();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetMethod(UnityWebRequestMethod methodType);

	internal void InternalSetMethod(UnityWebRequestMethod methodType)
	{
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
		}
		UnityWebRequestError unityWebRequestError = SetMethod(methodType);
		if (unityWebRequestError != UnityWebRequestError.OK)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetCustomMethod(string customMethodName);

	internal void InternalSetCustomMethod(string customMethodName)
	{
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
		}
		UnityWebRequestError unityWebRequestError = SetCustomMethod(customMethodName);
		if (unityWebRequestError != UnityWebRequestError.OK)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError GetError();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern string GetUrl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetUrl(string url);

	private void InternalSetUrl(string url)
	{
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its URL cannot be altered");
		}
		UnityWebRequestError unityWebRequestError = SetUrl(url);
		if (unityWebRequestError != UnityWebRequestError.OK)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private extern void SetRedirectLimitFromScripting(int limit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetChunked(bool chunked);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetRequestHeader")]
	internal extern UnityWebRequestError InternalSetRequestHeader(string name, string value);

	public void SetRequestHeader(string name, string value)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Cannot set a Request Header with a null or empty name");
		}
		if (value == null)
		{
			throw new ArgumentException("Cannot set a Request header with a null");
		}
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its request headers cannot be altered");
		}
		UnityWebRequestError unityWebRequestError = InternalSetRequestHeader(name, value);
		if (unityWebRequestError != UnityWebRequestError.OK)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern string GetResponseHeader(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern string[] GetResponseHeaderKeys();

	public Dictionary<string, string> GetResponseHeaders()
	{
		string[] responseHeaderKeys = GetResponseHeaderKeys();
		if (responseHeaderKeys == null || responseHeaderKeys.Length == 0)
		{
			return null;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>(responseHeaderKeys.Length, StringComparer.OrdinalIgnoreCase);
		for (int i = 0; i < responseHeaderKeys.Length; i++)
		{
			string responseHeader = GetResponseHeader(responseHeaderKeys[i]);
			dictionary.Add(responseHeaderKeys[i], responseHeader);
		}
		return dictionary;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetUploadHandler(UploadHandler uh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetDownloadHandler(DownloadHandler dh);

	public static UnityWebRequest Get(string uri)
	{
		return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
	}

	public static UnityWebRequest Post(string uri, WWWForm formData)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, formData);
		return unityWebRequest;
	}

	private static void SetupPost(UnityWebRequest request, WWWForm formData)
	{
		byte[] array = null;
		if (formData != null)
		{
			array = formData.data;
			if (array.Length == 0)
			{
				array = null;
			}
		}
		request.uploadHandler = new UploadHandlerRaw(array);
		request.downloadHandler = new DownloadHandlerBuffer();
		if (formData == null)
		{
			return;
		}
		Dictionary<string, string> headers = formData.headers;
		foreach (KeyValuePair<string, string> item in headers)
		{
			request.SetRequestHeader(item.Key, item.Value);
		}
	}

	public static string EscapeURL(string s, Encoding e)
	{
		if (s == null)
		{
			return null;
		}
		if (s == "")
		{
			return "";
		}
		if (e == null)
		{
			return null;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] bytes2 = WWWTranscoder.URLEncode(bytes);
		return e.GetString(bytes2);
	}
}
