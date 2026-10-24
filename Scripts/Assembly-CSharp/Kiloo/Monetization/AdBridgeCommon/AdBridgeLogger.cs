using UnityEngine;

namespace Kiloo.Monetization.AdBridgeCommon;

public class AdBridgeLogger
{
	private string _platform;

	private string _providerId;

	private IPlatformEventInvoker _platformEventInvoker;

	private bool _isLoggingEnabled;

	public AdBridgeLogger(string platform, string providerId, bool isLoggingEnabled, IPlatformEventInvoker platformEventInvoker)
	{
		_isLoggingEnabled = isLoggingEnabled;
		_platform = platform;
		_providerId = providerId;
		_platformEventInvoker = platformEventInvoker;
	}

	public void UpdateDebugLogsState()
	{
		if (_isLoggingEnabled)
		{
			_platformEventInvoker.EnableDebugLogs();
		}
		else
		{
			_platformEventInvoker.DisableDebugLogs();
		}
	}

	public void LogError(string msg, Object context = null)
	{
		SDebug.LogError($"{_providerId}{_platform}: {msg}", context);
	}

	public void LogWarning(string msg, Object context = null)
	{
	}

	public void Log(string msg, Object context = null)
	{
		if (!_isLoggingEnabled)
		{
		}
	}
}
