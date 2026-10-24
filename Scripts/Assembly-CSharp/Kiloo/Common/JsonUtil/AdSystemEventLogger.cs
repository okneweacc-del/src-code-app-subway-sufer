using Kiloo.Common.Ads;

namespace Kiloo.Common.JsonUtil;

public class AdSystemEventLogger : AdEventListenerAdapter
{
	public bool clicked { get; private set; }

	public CacheFailSource cacheFailSource { get; private set; }

	public AdState adState { get; private set; }

	public AdSystemEventLogger()
	{
		AdTrackingSystemLogger.SetAdSystemEventLogger(this);
	}

	public override void ShowedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		AdTrackingSystemLogger.LogFlow(adType, GetIdOfAdlocationBeingShown(), providerID, "Attempt to show ad", "ShowedAd");
		adState = AdState.ShownButNotFinished;
	}

	public override void FailedToShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		AdTrackingSystemLogger.LogFlow(adType, GetIdOfAdlocationBeingShown(), providerID, "Attempt to show ad", "FailedShowingAd");
	}

	public override void CachedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo, bool wasAlreadyCached)
	{
		string text = ((!wasAlreadyCached) ? "Success" : "AlreadyCached");
		if (GetCurrentProviderIDForLocation(adLocationInfo) != providerID && !wasAlreadyCached)
		{
			text += "NotCurrent";
		}
		AdTrackingSystemLogger.LogAttemptToCacheResult(adType, providerID, text);
		cacheFailSource = CacheFailSource.NoFail;
	}

	public override void DismissedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		AdTrackingSystemLogger.LogFlow(adType, adLocationInfo.id, providerID, "Result of ad shown", GetResultOfVideoShown(adType, providerID));
		clicked = false;
		adState = AdState.NotShown;
	}

	public override void FinishedWatchingVideo(string providerID, AdLocationInfo adLocationInfo)
	{
		adState = AdState.ShownAndFinished;
	}

	public override void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		string text = "Failed";
		string currentProviderIDForLocation = GetCurrentProviderIDForLocation(adLocationInfo);
		if (currentProviderIDForLocation != providerID)
		{
			text += "NotCurrent";
		}
		AdTrackingSystemLogger.LogAttemptToCacheResult(adType, providerID, text);
		cacheFailSource = CacheFailSource.CacheFail;
	}

	public override void CachingTimedOut(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		AdTrackingSystemLogger.LogAttemptToCacheResult(adType, providerID, "Timedout");
		cacheFailSource = CacheFailSource.Timeout;
	}

	public override void PriorityListWrapped(string waterfallID)
	{
		if (cacheFailSource == CacheFailSource.Timeout || cacheFailSource == CacheFailSource.CacheFail)
		{
			cacheFailSource = CacheFailSource.Waterfall;
		}
	}

	public override void ClickedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		clicked = true;
	}

	private string GetResultOfVideoShown(AdType adType, string providerId)
	{
		AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(providerId);
		if (clicked)
		{
			return string.Format("{0}{1}", GetAdShownPrefix(adType), "Clicked");
		}
		if (adProvider.adTypesWithClickCallbackSupport.Contains(adType))
		{
			return string.Format("{0}{1}", GetAdShownPrefix(adType), "Closed");
		}
		return string.Format("{0}{1}", GetAdShownPrefix(adType), "NoClickInfo");
	}

	private string GetAdShownPrefix(AdType adType)
	{
		if (adType == AdType.Video)
		{
			return GetVideoShownPrefix();
		}
		return adType.ToString();
	}

	private string GetVideoShownPrefix()
	{
		return adState switch
		{
			AdState.ShownButNotFinished => string.Format("{0}{1}", AdType.Video, "Early"), 
			AdState.ShownAndFinished => string.Format("{0}{1}", AdType.Video, "Finished"), 
			_ => string.Empty, 
		};
	}

	private string GetIdOfAdlocationBeingShown()
	{
		return AdSystemManager.GetIdOfAdlocationBeingShown();
	}

	private string GetCurrentProviderIDForLocation(AdLocationInfo adLocationInfo)
	{
		return AdSystemManager.GetCurrentProviderIDForLocation(adLocationInfo);
	}
}
