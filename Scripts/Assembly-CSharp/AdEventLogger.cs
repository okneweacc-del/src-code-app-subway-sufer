using System;
using System.Collections.Generic;
using System.Globalization;
using Kiloo.Common.Ads;
using Kiloo.Common.Utility;

public class AdEventLogger : IAdEventListener
{
	private struct CacheAttemptInfo
	{
		public AdType adType;

		public string providerID;

		public DateTime startCacheTime;

		public TimeSpan timePaused;
	}

	private const string EVENT_INTERSTITIAL_PROVIDER_REQUEST = "AdSystem2.0_Interstitial Providers Request";

	private const string EVENT_INTERSTITIAL_PROVIDER_SHOW = "AdSystem2.0_Interstitial Providers Show";

	private const string EVENT_INTERSTITIAL_SHOW_DAILY_LIMIT_REACHED = "AdSystem2.0_Interstitials Show Daily Limit Reached";

	private const string EVENT_INTERSTITIAL_SHOW_HOURLY_LIMIT_REACHED = "AdSystem2.0_Interstitials Show Hourly Limit Reached";

	private const string EVENT_INTERSTITIAL_SHOW_NEW_USER_LIMIT_REACHED = "AdSystem2.0_Interstitials Show New User Limit Reached";

	private const string EVENT_INTERSTITIAL_SHOW_TIME_BETWEEN_ADS_LIMIT_REACHED = "AdSystem2.0_Interstitials Show Time Between Ads Limit Reached";

	private const string EVENT_VIDEOADS_CACHE_REQUEST_ENDED = "AdSystem2.0_VideoAds Cache Request Ended";

	private const string EVENT_VIDEOADS_PROVIDER_SHOW = "AdSystem2.0_VideoAds Providers Show";

	private const string EVENT_VIDEOADS_FAILED_TO_SHOW = "AdSystem2.0_Failed To Show Video Provider";

	private const string EVENT_ADS_CACHING_TIME = "AdSystem2.0_Ads Caching Time";

	private const string EVENT_FAILED_TO_INITIALIZE_PROVIDER = "AdSystem2.0_Failed To Initialize Provider";

	private const string EVENT_END_OF_WATERFALL = "AdSystem2.0_End of waterfall";

	private const string EVENT_ARGVALUE_INTERSTITIAL_REQUEST_SUCCESS = "Request Success";

	private const string EVENT_ARGVALUE_INTERSTITIAL_REQUEST_FAILED = "Request Failed";

	private const string EVENT_ARGVALUE_INTERSTITIAL_REQUEST_TIMEOUT = "Request TimeOut";

	private const string EVENT_ARGVALUE_INTERSTITIAL_REQUEST_FAILED_TO_SHOW = "Request Failed To Show";

	private const string EVENT_ARGVALUE_INTERSTITIAL_SHOW_AD = "Show Ad";

	private const string EVENT_ARGVALUE_INTERSTITIAL_CLICK = "Click";

	private const string EVENT_ARGVALUE_INTERSTITIAL_DISMISSED = "Dismissed Ad";

	private const string EVENT_ARGKEY_INTERSTITIAL_LIMIT = "Limit";

	private const string EVENT_ARGVALUE_VIDEO_REQUEST_SUCCESS = "Success";

	private const string EVENT_ARGVALUE_VIDEO_REQUEST_FAILED = "Failed";

	private const string EVENT_ARGVALUE_VIDEO_REQUEST_TIMEDOUT = "Timedout";

	private const string EVENT_ARGVALUE_VIDEO_SHOW_START = "Video starts";

	private const string EVENT_ARGVALUE_VIDEO_SHOW_COMPLETE = "Video completes";

	private const string EVENT_ARGVALUE_VIDEO_SHOW_CLICKED = "Video clicked";

	private const string EVENT_ARGVALUE_VIDEO_SHOW_FAILED = "Video failed to show";

	private const string EVENT_ARGKEY_PROVIDER_RESULT = "{0}_{1} Result";

	private const string EVENT_ARGKEY_PROVIDER_FAILED = "{0} Fails";

	private const string EVENT_ARGVALUE_PROVIDER = "Provider";

	private const string EVENT_ARGKEY_CACHE_TIME = "{0}{1}{2} Time";

	private const string EVENT_ARGVALUE_WATERFALL_INTEGER = "Integer";

	private const string EVENT_ARGKEY_WATERFALL_COUNT = "{0}_{1}";

	private const int WATERFALL_COUNT_LIMIT = 20;

	private readonly string EVENT_ARGKEY_WATERFALL_ABOVE_LIMIT = "Above " + 20;

	private Dictionary<string, int> _priorityListIterationCounter = new Dictionary<string, int>();

	private Dictionary<AdLocationInfo, CacheAttemptInfo> _cacheInfo = new Dictionary<AdLocationInfo, CacheAttemptInfo>();

	private DateTime _onPauseTime;

	public AdEventLogger()
	{
		ApplicationState.paused += OnApplicationPaused;
		ApplicationState.resumed += OnApplicationResumed;
	}

	public void WillShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
	}

	public void ShowedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Interstitial Providers Show", "Show Ad");
			break;
		case AdType.Video:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_VideoAds Providers Show", "Video starts");
			break;
		}
	}

	public void CachedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo, bool wasAlreadyCached)
	{
		if (!wasAlreadyCached)
		{
			LogCacheTime(adType, providerID, adLocationInfo, "Success");
			switch (adType)
			{
			case AdType.Interstitial:
				LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Interstitial Providers Request", "Request Success");
				break;
			case AdType.Video:
				LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_VideoAds Cache Request Ended", "Success");
				break;
			}
		}
	}

	public void DismissedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		if (adType == AdType.Interstitial)
		{
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Interstitial Providers Show", "Dismissed Ad");
		}
	}

	public void StartedCachingAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		_cacheInfo[adLocationInfo] = new CacheAttemptInfo
		{
			adType = adType,
			providerID = providerID,
			startCacheTime = DateTime.Now
		};
	}

	private void OnApplicationPaused()
	{
		_onPauseTime = DateTime.Now;
	}

	private void OnApplicationResumed()
	{
		TimeSpan timeSpan = DateTime.Now - _onPauseTime;
		List<AdLocationInfo> list = new List<AdLocationInfo>(_cacheInfo.Keys);
		foreach (AdLocationInfo item in list)
		{
			CacheAttemptInfo value = _cacheInfo[item];
			value.timePaused += timeSpan;
			_cacheInfo[item] = value;
		}
	}

	public void FinishedWatchingVideo(string providerID, AdLocationInfo adLocationInfo)
	{
		LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_VideoAds Providers Show", "Video completes");
	}

	public void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		LogCacheTime(adType, providerID, adLocationInfo, "Failed");
		if (adType == AdType.Interstitial)
		{
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Interstitial Providers Request", "Request Failed");
		}
		else
		{
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_VideoAds Cache Request Ended", "Failed");
		}
	}

	private void LogCacheTime(AdType adType, string providerID, AdLocationInfo adLocationInfo, string status)
	{
		if (_cacheInfo.TryGetValue(adLocationInfo, out var value))
		{
			if (value.adType == adType && value.providerID == providerID)
			{
				double num = Math.Round((DateTime.Now - value.startCacheTime - value.timePaused).TotalSeconds * 2.0) / 2.0;
				Flurry.LogEventWithAParameter("AdSystem2.0_Ads Caching Time", $"{providerID}{adType}{status} Time", num.ToString(CultureInfo.InvariantCulture));
				_cacheInfo.Remove(adLocationInfo);
			}
		}
		else
		{
			AdSystemLog.LogWarning($"[CACHETIME] Did not find info about location '{adLocationInfo.id}' | currentInfo: {adType} | {providerID} | {status}");
		}
	}

	public void FailedToShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Interstitial Providers Request", "Request Failed To Show");
			break;
		case AdType.Video:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Failed To Show Video Provider", "Video failed to show");
			break;
		}
	}

	public void FailedToInitAdProvider(string providerID)
	{
		string argKey = $"{providerID} Fails";
		Flurry.LogEventWithAParameter("AdSystem2.0_Failed To Initialize Provider", argKey, "Provider");
	}

	public void CachingTimedOut(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		LogCacheTime(adType, providerID, adLocationInfo, "Timeout");
		switch (adType)
		{
		case AdType.Interstitial:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Interstitial Providers Request", "Request TimeOut");
			break;
		case AdType.Video:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_VideoAds Cache Request Ended", "Timedout");
			break;
		}
	}

	public void ClickedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_Interstitial Providers Show", "Click");
			break;
		case AdType.Video:
			LogProviderResult(providerID, adLocationInfo, "AdSystem2.0_VideoAds Providers Show", "Video clicked");
			break;
		}
	}

	public void ReachedDailyInterstitialLimit(int limit)
	{
		Flurry.LogEventWithAParameter("AdSystem2.0_Interstitials Show Daily Limit Reached", "Limit", limit.ToString());
	}

	public void ReachedHourlyInterstitialLimit(int limit)
	{
		Flurry.LogEventWithAParameter("AdSystem2.0_Interstitials Show Hourly Limit Reached", "Limit", limit.ToString());
	}

	public void ReachedNewUserInterstitialLimit(int limit)
	{
		Flurry.LogEventWithAParameter("AdSystem2.0_Interstitials Show New User Limit Reached", "Limit", limit.ToString());
	}

	public void ReachedTimeBetweenAdsInterstitialLimit(int limit)
	{
		Flurry.LogEventWithAParameter("AdSystem2.0_Interstitials Show Time Between Ads Limit Reached", "Limit", limit.ToString());
	}

	public void ChangedProvider(AdType adType, string newProvider, string changedWaterfallID)
	{
	}

	public void PriorityListWrapped(string waterfallID)
	{
		_priorityListIterationCounter.TryGetValue(waterfallID, out var value);
		value++;
		_priorityListIterationCounter[waterfallID] = value;
		string arg = ((value > 20) ? EVENT_ARGKEY_WATERFALL_ABOVE_LIMIT : value.ToString());
		string argKey = $"{waterfallID}_{arg}";
		Flurry.LogEventWithAParameter("AdSystem2.0_End of waterfall", argKey, "Integer");
	}

	private void LogProviderResult(string providerID, AdLocationInfo adLocationInfo, string eventName, string eventValue)
	{
		string argKey = $"{adLocationInfo.id}_{providerID} Result";
		Flurry.LogEventWithAParameter(eventName, argKey, eventValue);
	}
}
