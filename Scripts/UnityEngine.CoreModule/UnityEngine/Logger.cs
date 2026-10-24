using System;

namespace UnityEngine;

public class Logger : ILogger, ILogHandler
{
	public ILogHandler logHandler { get; set; }

	public bool logEnabled { get; set; }

	public LogType filterLogType { get; set; }

	public Logger(ILogHandler logHandler)
	{
		this.logHandler = logHandler;
		logEnabled = true;
		filterLogType = LogType.Log;
	}

	public bool IsLogTypeAllowed(LogType logType)
	{
		if (logEnabled)
		{
			if (logType == LogType.Exception)
			{
				return true;
			}
			if (filterLogType != LogType.Exception)
			{
				return logType <= filterLogType;
			}
		}
		return false;
	}

	private static string GetString(object message)
	{
		return (message == null) ? "Null" : message.ToString();
	}

	public void Log(LogType logType, object message)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, null, "{0}", GetString(message));
		}
	}

	public void Log(LogType logType, object message, Object context)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, context, "{0}", GetString(message));
		}
	}

	public void LogFormat(LogType logType, string format, params object[] args)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, null, format, args);
		}
	}

	public void LogFormat(LogType logType, Object context, string format, params object[] args)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, context, format, args);
		}
	}

	public void LogException(Exception exception, Object context)
	{
		if (logEnabled)
		{
			logHandler.LogException(exception, context);
		}
	}
}
