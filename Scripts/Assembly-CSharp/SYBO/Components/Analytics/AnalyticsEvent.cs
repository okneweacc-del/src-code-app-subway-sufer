using System;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics;

public abstract class AnalyticsEvent
{
	[SerializeField]
	[SerializedSizeLimit(36, false)]
	[SerializedAs("uid")]
	protected string _userID;

	[SerializeField]
	[SerializedSizeLimit(36, false)]
	[SerializedAs("id")]
	protected string _eventID;

	[SerializeField]
	[SerializedAs("client_version")]
	[SerializedSizeLimit(12, true)]
	protected string _gameVersion;

	[SerializeField]
	[SerializedAs("created_at")]
	private string _eventTime;

	public static Func<string> GetGameVersion;

	public static Func<DateTime> GetDeviceStartupTime;

	public static Func<string> GetUserID;

	public static Func<float> GetSamplingKey;

	public string UserId => _userID;

	public string EventId => _eventID;

	public string GameVersion => _gameVersion;

	public string EventTime => _eventTime;

	public abstract string EventName { get; }

	public AnalyticsEvent()
		: this(initializeValues: true, GetEventTime())
	{
	}

	protected AnalyticsEvent(bool initializeValues)
		: this(initializeValues, GetEventTime())
	{
	}

	protected AnalyticsEvent(bool initializeValues, DateTime eventTime)
	{
		if (initializeValues)
		{
			_eventID = Guid.NewGuid().ToString();
			_userID = GetUserID();
			_gameVersion = GetGameVersion();
			_eventTime = GetTimeText(eventTime);
		}
	}

	public static DateTime GetEventTime()
	{
		return GetDeviceStartupTime().AddMilliseconds(AnalyticsManager.GetMsTicksSinceDeviceStartup());
	}

	public static string GetTimeText(DateTime time)
	{
		return $"{time.Year:0000}-{time.Month:00}-{time.Day:00}T{time.Hour:00}:{time.Minute:00}:{time.Second:00}.{time.Millisecond:000}Z";
	}

	public override string ToString()
	{
		return $"{GetType().Name} for user {_userID}";
	}
}
