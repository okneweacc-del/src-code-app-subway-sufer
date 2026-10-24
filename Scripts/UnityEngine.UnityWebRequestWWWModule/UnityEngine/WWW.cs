using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace UnityEngine;

public class WWW : CustomYieldInstruction, IDisposable
{
	private UnityWebRequest _uwr;

	private Dictionary<string, string> _responseHeaders;

	public string error
	{
		get
		{
			if (!_uwr.isDone)
			{
				return null;
			}
			if (_uwr.isNetworkError)
			{
				return _uwr.error;
			}
			if (_uwr.responseCode >= 400)
			{
				return $"{_uwr.responseCode} {GetStatusCodeName(_uwr.responseCode)}";
			}
			return null;
		}
	}

	public bool isDone => _uwr.isDone;

	public Dictionary<string, string> responseHeaders
	{
		get
		{
			if (!isDone)
			{
				return new Dictionary<string, string>();
			}
			if (_responseHeaders == null)
			{
				_responseHeaders = _uwr.GetResponseHeaders();
				if (_responseHeaders != null)
				{
					_responseHeaders["STATUS"] = $"HTTP/1.1 {_uwr.responseCode} {GetStatusCodeName(_uwr.responseCode)}";
				}
				else
				{
					_responseHeaders = new Dictionary<string, string>();
				}
			}
			return _responseHeaders;
		}
	}

	public string text
	{
		get
		{
			if (!WaitUntilDoneIfPossible())
			{
				return "";
			}
			if (_uwr.isNetworkError)
			{
				return "";
			}
			DownloadHandler downloadHandler = _uwr.downloadHandler;
			if (downloadHandler == null)
			{
				return "";
			}
			return downloadHandler.text;
		}
	}

	public Texture2D texture => CreateTextureFromDownloadedData(markNonReadable: false);

	public string url => _uwr.url;

	public override bool keepWaiting => !_uwr.isDone;

	public WWW(string url)
	{
		_uwr = UnityWebRequest.Get(url);
		_uwr.SendWebRequest();
	}

	public WWW(string url, WWWForm form)
	{
		_uwr = UnityWebRequest.Post(url, form);
		_uwr.chunkedTransfer = false;
		_uwr.SendWebRequest();
	}

	public WWW(string url, byte[] postData, Dictionary<string, string> headers)
	{
		_uwr = new UnityWebRequest(url, (postData != null) ? "POST" : "GET");
		_uwr.chunkedTransfer = false;
		UploadHandler uploadHandler = new UploadHandlerRaw(postData)
		{
			contentType = "application/x-www-form-urlencoded"
		};
		_uwr.uploadHandler = uploadHandler;
		_uwr.downloadHandler = new DownloadHandlerBuffer();
		foreach (KeyValuePair<string, string> header in headers)
		{
			_uwr.SetRequestHeader(header.Key, header.Value);
		}
		_uwr.SendWebRequest();
	}

	public static string EscapeURL(string s)
	{
		return EscapeURL(s, Encoding.UTF8);
	}

	public static string EscapeURL(string s, Encoding e)
	{
		return UnityWebRequest.EscapeURL(s, e);
	}

	private Texture2D CreateTextureFromDownloadedData(bool markNonReadable)
	{
		if (!WaitUntilDoneIfPossible())
		{
			return new Texture2D(2, 2);
		}
		if (_uwr.isNetworkError)
		{
			return null;
		}
		DownloadHandler downloadHandler = _uwr.downloadHandler;
		if (downloadHandler == null)
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(downloadHandler.data, markNonReadable);
		return texture2D;
	}

	public void LoadImageIntoTexture(Texture2D texture)
	{
		if (!WaitUntilDoneIfPossible())
		{
			return;
		}
		if (_uwr.isNetworkError)
		{
			Debug.LogError("Cannot load image: download failed");
			return;
		}
		DownloadHandler downloadHandler = _uwr.downloadHandler;
		if (downloadHandler == null)
		{
			Debug.LogError("Cannot load image: internal error");
		}
		else
		{
			texture.LoadImage(downloadHandler.data, markNonReadable: false);
		}
	}

	public void Dispose()
	{
		_uwr.Dispose();
	}

	private bool WaitUntilDoneIfPossible()
	{
		if (_uwr.isDone)
		{
			return true;
		}
		if (url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
		{
			while (!_uwr.isDone)
			{
			}
			return true;
		}
		Debug.LogError("You are trying to load data from a www stream which has not completed the download yet.\nYou need to yield the download or wait until isDone returns true.");
		return false;
	}

	private string GetStatusCodeName(long statusCode)
	{
		if (statusCode >= 400 && statusCode <= 417)
		{
			switch ((int)(statusCode - 400))
			{
			case 0:
				return "Bad Request";
			case 1:
				return "Unauthorized";
			case 2:
				return "Payment Required";
			case 3:
				return "Forbidden";
			case 4:
				return "Not Found";
			case 5:
				return "Method Not Allowed";
			case 6:
				return "Not Acceptable";
			case 7:
				return "Proxy Authentication Required";
			case 8:
				return "Request Timeout";
			case 9:
				return "Conflict";
			case 10:
				return "Gone";
			case 11:
				return "Length Required";
			case 12:
				return "Precondition Failed";
			case 13:
				return "Request Entity Too Large";
			case 14:
				return "Request-URI Too Long";
			case 15:
				return "Unsupported Media Type";
			case 16:
				return "Requested Range Not Satisfiable";
			case 17:
				return "Expectation Failed";
			}
		}
		if (statusCode >= 200 && statusCode <= 206)
		{
			switch ((int)(statusCode - 200))
			{
			case 0:
				return "OK";
			case 1:
				return "Created";
			case 2:
				return "Accepted";
			case 3:
				return "Non-Authoritative Information";
			case 4:
				return "No Content";
			case 5:
				return "Reset Content";
			case 6:
				return "Partial Content";
			}
		}
		if (statusCode >= 300 && statusCode <= 307)
		{
			switch ((int)(statusCode - 300))
			{
			case 0:
				return "Multiple Choices";
			case 1:
				return "Moved Permanently";
			case 2:
				return "Found";
			case 3:
				return "See Other";
			case 4:
				return "Not Modified";
			case 5:
				return "Use Proxy";
			case 7:
				return "Temporary Redirect";
			}
		}
		if (statusCode >= 500 && statusCode <= 505)
		{
			switch ((int)(statusCode - 500))
			{
			case 0:
				return "Internal Server Error";
			case 1:
				return "Not Implemented";
			case 2:
				return "Bad Gateway";
			case 3:
				return "Service Unavailable";
			case 4:
				return "Gateway Timeout";
			case 5:
				return "HTTP Version Not Supported";
			}
		}
		return "";
	}
}
