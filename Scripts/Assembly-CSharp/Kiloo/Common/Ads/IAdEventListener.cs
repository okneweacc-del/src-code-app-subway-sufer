namespace Kiloo.Common.Ads;

public interface IAdEventListener
{
	void WillShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void ShowedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void CachedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo, bool wasAlreadyCached);

	void DismissedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void StartedCachingAd(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void FinishedWatchingVideo(string providerID, AdLocationInfo adLocationInfo);

	void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void FailedToShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void FailedToInitAdProvider(string providerID);

	void CachingTimedOut(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void ClickedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo);

	void ReachedDailyInterstitialLimit(int limit);

	void ReachedHourlyInterstitialLimit(int limit);

	void ReachedNewUserInterstitialLimit(int limit);

	void ReachedTimeBetweenAdsInterstitialLimit(int limit);

	void ChangedProvider(AdType adType, string newProvider, string changedWaterfallID);

	void PriorityListWrapped(string waterfallID);
}
