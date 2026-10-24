using Kiloo.Common.Ads;

namespace Kiloo.Common.JsonUtil;

public class AdTrackingSystemLogger
{
	private static AdSystemEventLogger _adSystemEventLogger;

	public AdTrackingSystemLogger()
	{
		new WaterFallReach();
	}

	public static void SetAdSystemEventLogger(AdSystemEventLogger adSystemEventLogger)
	{
		_adSystemEventLogger = adSystemEventLogger;
	}

	public static void LogFlow(AdType adType, AdLocationInfo adLocation, string flowId, string result)
	{
		LogFlow(adType, adLocation.id, AdSystemManager.GetHighestPriorityProviderWithCachedAd(adLocation), flowId, result);
	}

	public static void LogFlow(AdType adType, string adLocationId, string providerId, string flowId, string result)
	{
		string eventName = $"{GetAdPrefix(adType)}{adLocationId};{flowId}";
		string argValue = $"{providerId};{result}";
		Flurry.LogEventWithAParameter(eventName, "Outcome", argValue);
	}

	public static void LogAttemptToShowButtonFlow(AdLocationInfo adLocation)
	{
		LogAttemptToShowButtonFlow(adLocation, AdSystemManager.IsCached(adLocation));
	}

	public static void LogAttemptToShowButtonFlow(AdLocationInfo adLocation, bool isCached)
	{
		if (_adSystemEventLogger != null)
		{
			string result = "Fill";
			CacheFailSource cacheFailSource = _adSystemEventLogger.cacheFailSource;
			if (!isCached)
			{
				result = cacheFailSource switch
				{
					CacheFailSource.Timeout => "NoFillCacheTimeout", 
					CacheFailSource.Waterfall => "NoFillEndOfWaterfall", 
					_ => "NoFill", 
				};
			}
			LogAttemptToShowButtonFlow(adLocation, result);
		}
	}

	public static void LogAttemptToShowButtonFlow(AdLocationInfo adLocation, string result)
	{
		LogFlow(AdType.Video, adLocation, "Attempt to show button", result);
	}

	public static void LogVideoAdButtonClick(AdLocationInfo adLocation)
	{
		Flurry.LogEventWithAParameter($"{adLocation.id}_generic", AdSystemManager.GetHighestPriorityProviderWithCachedAd(adLocation), "Click");
	}

	public static void LogWaterFallReach(string screenName, int waterfallReach)
	{
		string argValue = $"{screenName};{waterfallReach}";
		Flurry.LogEventWithAParameter("WaterfallReach", "screen_name;waterfall reach", argValue);
	}

	public static void LogFillRequest(AdLocationInfo adLocation)
	{
		LogFillRequest(adLocation, AdSystemManager.IsCached(adLocation));
	}

	public static void LogFillRequest(AdLocationInfo adLocation, bool adAvailable)
	{
		string result = ((!adAvailable) ? "NoAdAvailable" : "AdAvailable");
		LogFlow(AdType.Video, adLocation, "Fill Request", result);
	}

	public static void LogAttemptToCacheResult(AdType adType, string providerId, string result)
	{
		string eventName = string.Format("{0}{1}", GetAdPrefix(adType), "AttemptToCacheResult");
		string argValue = $"{providerId};{result}";
		Flurry.LogEventWithAParameter(eventName, "Outcome", argValue);
	}

	private static string GetAdPrefix(AdType adType)
	{
		return adType switch
		{
			AdType.Interstitial => "int_", 
			AdType.Video => "vid_", 
			_ => string.Empty, 
		};
	}
}
