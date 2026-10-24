using System;
using System.Collections.Generic;

namespace Kiloo.Common.Ads;

public interface IAdSettingsProvider
{
	IList<AdProviderInfo> GetPriorityList(string waterfallID, AdType adType);

	TimeSpan GetCacheTimeoutLimit(string providerID, AdType adType);

	TimeSpan? GetTimeLimitForAdProvider(string providerID, AdType adType);

	TimeSpan GetMinimumShowInterval(AdType adType);

	uint GetGracePeriodValue();

	DateTime GetScheduledFirstInterstitialShowTime();

	TimeSpan GetMinimumShowIntervalForLocation(string locationID, AdType adType);

	bool GetAvailabilityForLocation(string adLocationID);

	Dictionary<AdLocationInfo, string> GetProviderCacheIDs(string providerId);

	string GetCacheID(string adProvider, AdLocationInfo adLocationID);

	string GetWaterfallID(AdType adType);

	string GetWaterfallID(string adLocationID);

	string GetLatestVersionForWaterfall(AdLocation adLocation);

	string[] GetAdProvidersToInitialize();

	AdProviderConfiguration GetConfigurationForProvider(string providerID);

	int GetFailCountTolerance(string adProvider, AdType adType);

	int GetMaxImpressionForProvider(string adProvider, AdType adType);

	void SetDebugPriorityListVersion(AdType adType, string version);

	int GetMaxInterstitialImpressionsPerHour();

	int GetMaxInterstitialImpressionsPerDay();

	void UpdateProviderCacheIDs();

	bool AreInterstitialsAllowed();

	int GetProviderWaitWhileCachingTimeLimit(string adLocationID, string providerID);

	bool TryGetProviderOverrideSupportedOnPlatform(AdType adType, string providerID, out string supportedDeviceVersion);
}
