namespace UnityEngine;

public interface ILogger : ILogHandler
{
	void Log(LogType logType, object message);

	void Log(LogType logType, object message, Object context);

	void LogFormat(LogType logType, string format, params object[] args);
}
