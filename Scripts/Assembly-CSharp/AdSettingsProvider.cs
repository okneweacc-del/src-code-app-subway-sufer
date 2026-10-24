using System;
using System.Collections.Generic;
using AdHandling.AdConstants;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using UnityEngine;

public class AdSettingsProvider : IAdSettingsProvider
{
	private const string AGE_RESTRICTION_PREFIX = "_restricted";

	private const char PRIORITY_LIST_PROVIDER_SEPARATOR = ',';

	private const char PRIORITY_LIST_PROVIDER_COMPONENTS_SEPARATOR = ':';

	private const char PRIORITY_LIST_PROVIDER_PRIORITY_WEIGHT_SEPARATOR = '_';

	private const string INTERSTITIAL_GRACE_PERIOD_KEY = "ads_grace_period";

	private const string NUMBER_OF_FAILS_FOR_EACH_PROVIDER = "number_of_fails_for_interstitial";

	private const string GLOBAL_TIME_BETWEEN_INTERSTITIALS_KEY = "global_time_between_interstitials";

	private const string LIMIT_PER_HOUR = "interstitials_limit_per_hour";

	private const string LIMIT_PER_DAY = "interstitials_limit_per_day";

	private const string LOCK_ON_TIME_LIMIT_KEY = "lock_on_time_limit_{0}";

	private const string CACHE_TIMEOUT_LIMIT_ADTYPE_KEY = "cache_timeout_limit_{0}";

	private const string CACHE_TIMEOUT_LIMIT_PROVIDER_KEY = "cache_timeout_limit_{0}_{1}";

	private const string AD_LOCATION_OVERRIDE_CACHE_ID_KEY = "ad_location_{0}_{1}{2}_cacheid";

	private const string AD_LOCATION_TIME_LIMIT_KEY = "ad_location_{0}_time_between_interstitials";

	private const string AD_LOCATION_AVAILABILITY_KEY = "ad_location_{0}_enabled";

	private const string AD_LOCATION_WAIT_WHILE_CACHING_TIME_LIMIT_KEY = "ad_location_{0}_{1}_wait_caching_limit";

	private const string PRIORITY_LIST_VERSION_KEY = "prio_list_{0}_version";

	private const string PROVIDER_MINIMUM_DEVICE_VERSION_KEY = "provider_{0}_{1}_minimum_device_version";

	private const string WATERFALL_LOCATION_LIST_KEY = "{0}_list";

	private const string WATERFALL_ADTYPE_LIST_KEY = "default_{0}_list";

	private const string IMPRESSION_LIMIT_KEY = "impression_limit_{0}_{1}";

	private const uint DEFAULT_INTERSTITIAL_GRACE_PERIOD = 72000u;

	private const int DEFAULT_NUMBER_OF_FAILS_FOR_PROVIDER = 1;

	private const int DEFAULT_GLOBAL_TIME_BETWEEN_INTERSTITIALS = 10;

	private const int DEFAULT_LIMIT_PER_HOUR = 5;

	private const int DEFAULT_LIMIT_PER_DAY = 20;

	private const int DEFAULT_LOCK_ON_TIME_LIMIT_INTERSTITIAL = 72000;

	private const int DEFAULT_LOCK_ON_TIME_LIMIT_VIDEO = 72000;

	private const int DEFAULT_CACHE_TIMEOUT_LIMIT = 10;

	private const bool DEFAULT_AD_LOCATION_AVAILABILITY = false;

	private const int DEFAULT_AD_LOCATION_TIME_LIMIT = 10;

	private const int DEFAULT_AD_LOCATION_WAIT_WHILE_CACHING_TIME_LIMIT = 10;

	private const string DEFAULT_PRIORITY_LIST_VERSION = "notset";

	private const int DEFAULT_IMPRESSION_LIMIT = int.MaxValue;

	private Dictionary<AdType, string> _debugPriorityListVersion = new Dictionary<AdType, string>();

	private static List<AdProviderInfo> _allInterstitialProviders = new List<AdProviderInfo>
	{
		new AdProviderInfo
		{
			providerID = "supersonic",
			priority = 1,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "inmobi",
			priority = 2,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "chartboost",
			priority = 3,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "facebook",
			priority = 4,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "mobvista",
			priority = 6,
			weight = 100f
		}
	};

	private static List<AdProviderInfo> _allVideoProviders = new List<AdProviderInfo>
	{
		new AdProviderInfo
		{
			providerID = "supersonic",
			priority = 1,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "adcolony",
			priority = 2,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "tapjoy",
			priority = 3,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "inmobi",
			priority = 4,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "vungle",
			priority = 5,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "chartboost",
			priority = 6,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "facebook",
			priority = 7,
			weight = 100f
		},
		new AdProviderInfo
		{
			providerID = "mobvista",
			priority = 8,
			weight = 100f
		}
	};

	private static List<string> _interstitialProviderIDs = new List<string>();

	private static List<string> _videoProviderIDs = new List<string>();

	private static Dictionary<string, AdType> _cachedAdTypesForLocationIDs = new Dictionary<string, AdType>();

	public virtual IList<AdProviderInfo> GetPriorityList(string waterfallID, AdType adType)
	{
		if ((Globals.DEBUG_INTERSTITIAL_PROVIDERS && adType == AdType.Interstitial) || (Globals.DEBUG_VIDEOADS_PROVIDERS && adType == AdType.Video))
		{
			return GetDebugPriorityList(adType);
		}
		CacheProviderIDs();
		List<AdProviderInfo> list = new List<AdProviderInfo>();
		if (!OnlineSettings.instance.TryGetValue(waterfallID, out var valueString))
		{
			waterfallID = GetWaterfallID(adType);
			OnlineSettings.instance.TryGetValue(waterfallID, out valueString);
		}
		if (!string.IsNullOrEmpty(valueString))
		{
			valueString = valueString.Replace("\r\n", string.Empty).Replace("\n", string.Empty);
			string[] array = valueString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split(':');
					if (array3.Length >= 2)
					{
						string text2 = array3[0];
						if ((adType == AdType.Interstitial && _interstitialProviderIDs.Contains(text2)) || (adType == AdType.Video && _videoProviderIDs.Contains(text2)))
						{
							string[] array4 = array3[1].Split('_');
							if (array4.Length >= 2)
							{
								if (int.TryParse(array4[0], out var result) && int.TryParse(array4[1], out var result2))
								{
									if (AbstractAdProvider.IsProviderSupportedOnPlatform(text2, adType))
									{
										AdProviderInfo adProviderInfo = new AdProviderInfo();
										adProviderInfo.providerID = text2;
										adProviderInfo.priority = result;
										adProviderInfo.weight = result2;
										AdProviderInfo item = adProviderInfo;
										list.Add(item);
									}
									else
									{
										AdSystemLog.LogError(string.Concat("The provider with ID: ", text2, " and adType: ", adType, " is not supported on this platform."));
									}
								}
								else
								{
									AdSystemLog.LogError("The priority or weight of the provider: " + text2 + " in the list: " + waterfallID + " is not the expected data type!");
								}
							}
							else
							{
								AdSystemLog.LogError("The priority or weight of the provider: " + text2 + " in the list: " + waterfallID + " is not formatted correctly!");
							}
						}
						else
						{
							AdSystemLog.LogError("The provider with ID: " + text2 + " in the list: " + waterfallID + " is not registered as a valid provider for ad type: " + adType);
						}
					}
					else
					{
						AdSystemLog.LogError("The provider information: '" + array3.ToContentString() + "' is not formatted correctly in online settings");
					}
				}
			}
			else
			{
				AdSystemLog.LogError("Priority list ID: " + waterfallID + " has not content!");
			}
		}
		else
		{
			AdSystemLog.LogError("Priority list ID: " + waterfallID + " was not found in online settings!");
		}
		return list;
	}

	public virtual TimeSpan GetCacheTimeoutLimit(string providerID, AdType adType)
	{
		string key = $"cache_timeout_limit_{adType}".ToLower();
		int value = OnlineSettings.instance.GetValue(key, 10);
		string key2 = $"cache_timeout_limit_{adType}_{providerID}".ToLower();
		int value2 = OnlineSettings.instance.GetValue(key2, value);
		return TimeSpan.FromSeconds(value2);
	}

	public virtual TimeSpan? GetTimeLimitForAdProvider(string providerID, AdType adType)
	{
		string key = $"lock_on_time_limit_{adType}".ToLower();
		int defaultValue;
		switch (adType)
		{
		case AdType.Interstitial:
			defaultValue = 72000;
			break;
		case AdType.Video:
			defaultValue = 72000;
			break;
		default:
			return null;
		}
		return TimeSpan.FromSeconds(OnlineSettings.instance.GetValue(key, defaultValue));
	}

	public virtual TimeSpan GetMinimumShowInterval(AdType adType)
	{
		switch (adType)
		{
		case AdType.Interstitial:
		{
			int value = OnlineSettings.instance.GetValue("global_time_between_interstitials", 10);
			return TimeSpan.FromSeconds(value);
		}
		case AdType.Video:
			return TimeSpan.Zero;
		default:
			AdSystemLog.LogError("AdSettingsProvider.cs The minimum show interval has not been set for the adType: " + adType);
			return TimeSpan.Zero;
		}
	}

	public virtual uint GetGracePeriodValue()
	{
		return OnlineSettings.instance.GetValue("ads_grace_period", 72000u);
	}

	public virtual DateTime GetScheduledFirstInterstitialShowTime()
	{
		DateTime result = DateTime.MinValue;
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT || Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_GRACE_PERIOD)
		{
			return result;
		}
		if (AdsSave.scheduledFirstInterstitialShowTime != DateTime.MinValue)
		{
			if (CDateTime.UtcNow > UserSave.firstInstallDate)
			{
				result = AdsSave.scheduledFirstInterstitialShowTime;
			}
		}
		else
		{
			uint gracePeriodValue = GetGracePeriodValue();
			result = (AdsSave.scheduledFirstInterstitialShowTime = UserSave.firstInstallDate + TimeSpan.FromSeconds(gracePeriodValue));
		}
		return result;
	}

	public virtual bool GetAvailabilityForLocation(string locationID)
	{
		bool flag = false;
		AdType? adType;
		if (!_cachedAdTypesForLocationIDs.ContainsKey(locationID))
		{
			adType = GetAdTypeForLocationID(locationID);
			if (adType.HasValue)
			{
				_cachedAdTypesForLocationIDs.Add(locationID, adType.Value);
			}
			else
			{
				AdSystemLog.LogError("The location with id: " + locationID + " does NOT have an AdType assigned to it. Please assign an AdType!");
			}
		}
		else
		{
			adType = _cachedAdTypesForLocationIDs[locationID];
		}
		if (adType.HasValue && ((Globals.DEBUG_VIDEOADS_PROVIDERS && adType == AdType.Video) || (Globals.DEBUG_INTERSTITIAL_PROVIDERS && adType.GetValueOrDefault() == AdType.Interstitial && adType.HasValue)))
		{
			foreach (AdLocationInfo adLocation in AdLocations.adLocations)
			{
				if (adLocation.id == locationID)
				{
					flag = true;
				}
			}
		}
		else
		{
			string key = $"ad_location_{locationID}_enabled";
			flag = OnlineSettings.instance.GetValue(key, defaultValue: false);
			if (adType.HasValue && adType.Value == AdType.Interstitial)
			{
				if (Application.systemLanguage == SystemLanguage.Chinese)
				{
					return true;
				}
				if (flag)
				{
					flag = AreInterstitialsAllowed();
				}
			}
		}
		return flag;
	}

	public virtual bool AreInterstitialsAllowed()
	{
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES)
		{
			return true;
		}
		bool hasPurchasedOrRestored = UserSave.hasPurchasedOrRestored;
		return !hasPurchasedOrRestored;
	}

	public virtual int GetProviderWaitWhileCachingTimeLimit(string adLocationID, string providerID)
	{
		string key = $"ad_location_{adLocationID}_{providerID}_wait_caching_limit";
		return OnlineSettings.instance.GetValue(key, 10);
	}

	public virtual bool TryGetProviderOverrideSupportedOnPlatform(AdType adType, string providerID, out string supportedDeviceVersion)
	{
		string key = $"provider_{providerID}_{adType}_minimum_device_version".ToLower();
		return OnlineSettings.instance.TryGetValue(key, out supportedDeviceVersion);
	}

	public virtual TimeSpan GetMinimumShowIntervalForLocation(string locationID, AdType adType)
	{
		TimeSpan result = TimeSpan.Zero;
		if (adType == AdType.Interstitial)
		{
			string key = $"ad_location_{locationID}_time_between_interstitials";
			result = TimeSpan.FromSeconds(OnlineSettings.instance.GetValue(key, 10));
		}
		return result;
	}

	public virtual void UpdateProviderCacheIDs()
	{
		foreach (AdLocationInfo adLocation in AdLocations.adLocations)
		{
			IEnumerable<AdProviderInfo> allProvidersForAdType = GetAllProvidersForAdType(adLocation.adType);
			if (allProvidersForAdType == null)
			{
				continue;
			}
			string arg = ((!AgeRestriction.isRestricted) ? string.Empty : "_restricted");
			foreach (AdProviderInfo item in allProvidersForAdType)
			{
				string key = $"ad_location_{adLocation.id}_{item.providerID}{arg}_cacheid";
				if (OnlineSettings.instance.TryGetValue(key, out var valueString))
				{
					Dictionary<AdLocationInfo, string> providerCacheIDs = GetProviderCacheIDs(item.providerID);
					if (providerCacheIDs != null)
					{
						providerCacheIDs[adLocation] = valueString;
					}
				}
			}
		}
	}

	private static IEnumerable<AdProviderInfo> GetAllProvidersForAdType(AdType adType)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			return _allInterstitialProviders;
		case AdType.Video:
			return _allVideoProviders;
		default:
			AdSystemLog.LogError("GetAllProvidersForAdType: Unhandled AdType " + adType);
			return null;
		}
	}

	public virtual Dictionary<AdLocationInfo, string> GetProviderCacheIDs(string providerId)
	{
		return providerId switch
		{
			"adcolony" => AdConstants.instance.AdColonyCacheIDs, 
			"inmobi" => AdConstants.instance.InmobiCacheIDs, 
			"chartboost" => AdConstants.instance.ChartboostCacheIDs, 
			"supersonic" => AdConstants.instance.SupersonicCacheIDs, 
			"tapjoy" => AdConstants.instance.TapJoyCacheIDs, 
			"vungle" => AdConstants.instance.VungleCacheIDs, 
			"facebook" => AdConstants.instance.FacebookCacheIDs, 
			"mobvista" => AdConstants.instance.MobVistaCacheIDs, 
			_ => null, 
		};
	}

	public virtual string GetCacheID(string adProvider, AdLocationInfo adLocation)
	{
		Dictionary<AdLocationInfo, string> providerCacheIDs = GetProviderCacheIDs(adProvider);
		if (providerCacheIDs != null && providerCacheIDs.TryGetValue(adLocation, out var value))
		{
			return value;
		}
		AdSystemLog.LogError(string.Format("Cache ID not found in AdSettingsProvider.GetCacheID for provider: {0} and location: {1} dict size: {2}", adProvider, adLocation.id, (providerCacheIDs != null) ? providerCacheIDs.Count.ToString() : "null"));
		return string.Empty;
	}

	public virtual string GetWaterfallID(string adLocationID)
	{
		string text = string.Empty;
		foreach (AdLocationInfo adLocation in AdLocations.adLocations)
		{
			if (adLocationID == adLocation.id)
			{
				text = $"{adLocationID}_list";
				break;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			AdSystemLog.LogError("WaterfallID is not set for locationID: " + adLocationID);
		}
		return text;
	}

	public virtual string GetWaterfallID(AdType adType)
	{
		return $"default_{adType}_list".ToLower();
	}

	public virtual string GetLatestVersionForWaterfall(AdLocation adLocation)
	{
		if ((Globals.DEBUG_INTERSTITIAL_PROVIDERS && adLocation.info.adType == AdType.Interstitial) || (Globals.DEBUG_VIDEOADS_PROVIDERS && adLocation.info.adType == AdType.Video))
		{
			_debugPriorityListVersion.TryGetValue(adLocation.info.adType, out var value);
			return value;
		}
		string key = $"prio_list_{adLocation.waterfallID}_version";
		return OnlineSettings.instance.GetValue(key, "notset");
	}

	public virtual string[] GetAdProvidersToInitialize()
	{
		return new string[0];
	}

	private string[] GetAdColonyZoneIDs()
	{
		Dictionary<AdLocationInfo, string> adColonyCacheIDs = AdConstants.instance.AdColonyCacheIDs;
		List<string> list = new List<string>();
		foreach (AdLocationInfo adLocation in AdLocations.adLocations)
		{
			if (adColonyCacheIDs.TryGetValueNotNullOrEmpty(adLocation, out var obj))
			{
				list.Add(obj);
			}
		}
		return list.ToArray();
	}

	public virtual AdProviderConfiguration GetConfigurationForProvider(string providerID)
	{
		AdProviderConfiguration adProviderConfiguration = new AdProviderConfiguration();
		adProviderConfiguration.isRestricted = AdConstants.instance.isRestricted;
		AdProviderConfiguration adProviderConfiguration2 = adProviderConfiguration;
		switch (providerID)
		{
		case "inmobi":
			adProviderConfiguration2.appID = AdConstants.instance.InmobiAccountId;
			break;
		case "adcolony":
			adProviderConfiguration2.appID = AdConstants.instance.AdColonyAppId;
			adProviderConfiguration2.zoneIDs = GetAdColonyZoneIDs();
			break;
		case "chartboost":
			adProviderConfiguration2.appID = AdConstants.instance.ChartboostAppId;
			adProviderConfiguration2.appSignature = AdConstants.instance.ChartboostAppSignature;
			break;
		case "tapjoy":
			adProviderConfiguration2.appID = AdConstants.instance.TapJoyAppId;
			adProviderConfiguration2.appSDKKey = AdConstants.instance.TapJoyAppKey;
			break;
		case "supersonic":
			adProviderConfiguration2.appID = AdConstants.instance.SupersonicAppId;
			break;
		case "vungle":
			adProviderConfiguration2.appID = AdConstants.instance.VungleAppId;
			break;
		case "mobvista":
			adProviderConfiguration2.appID = AdConstants.instance.MobVistaAppId;
			adProviderConfiguration2.appSDKKey = AdConstants.instance.MobVistaAppSdkKey;
			break;
		default:
			AdSystemLog.LogError("Configuration is not defined for " + providerID);
			break;
		case "facebook":
			break;
		}
		return adProviderConfiguration2;
	}

	public virtual int GetFailCountTolerance(string adProvider, AdType adType)
	{
		return OnlineSettings.instance.GetValue("number_of_fails_for_interstitial", 1);
	}

	public virtual int GetMaxImpressionForProvider(string adProvider, AdType adType)
	{
		if (Globals.DEBUG_AD_SYSTEM_SINGLE_IMPRESSION)
		{
			return 1;
		}
		string key = $"impression_limit_{adProvider}_{adType}".ToLower();
		return OnlineSettings.instance.GetValue(key, int.MaxValue);
	}

	public virtual int GetMaxInterstitialImpressionsPerHour()
	{
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IMPRESSION_LIMIT)
		{
			return int.MaxValue;
		}
		return OnlineSettings.instance.GetValue("interstitials_limit_per_hour", 5);
	}

	public virtual int GetMaxInterstitialImpressionsPerDay()
	{
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IMPRESSION_LIMIT)
		{
			return int.MaxValue;
		}
		return OnlineSettings.instance.GetValue("interstitials_limit_per_day", 20);
	}

	private static void CacheProviderIDs()
	{
		if (_videoProviderIDs.Count == 0)
		{
			for (int i = 0; i < _allVideoProviders.Count; i++)
			{
				if (!_videoProviderIDs.Contains(_allVideoProviders[i].providerID))
				{
					_videoProviderIDs.Add(_allVideoProviders[i].providerID);
				}
			}
		}
		if (_interstitialProviderIDs.Count != 0)
		{
			return;
		}
		for (int j = 0; j < _allInterstitialProviders.Count; j++)
		{
			if (!_interstitialProviderIDs.Contains(_allInterstitialProviders[j].providerID))
			{
				_interstitialProviderIDs.Add(_allInterstitialProviders[j].providerID);
			}
		}
	}

	private static List<AdProviderInfo> GetDebugPriorityList(AdType adType)
	{
		List<AdProviderInfo> list = new List<AdProviderInfo>();
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS && adType == AdType.Interstitial)
		{
			foreach (AdProviderInfo allInterstitialProvider in _allInterstitialProviders)
			{
				bool flag = false;
				switch (allInterstitialProvider.providerID)
				{
				case "inmobi":
					flag = Globals.DEBUG_INTERSTITIAL_PROVIDER_INMOBI;
					break;
				case "supersonic":
					flag = Globals.DEBUG_INTERSTITIAL_PROVIDER_SUPERSONIC;
					break;
				case "chartboost":
					flag = Globals.DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST;
					break;
				case "facebook":
					flag = Globals.DEBUG_INTERSTITIAL_PROVIDER_FACEBOOK;
					break;
				case "mobvista":
					flag = Globals.DEBUG_INTERSTITIAL_PROVIDER_MOBVISTA;
					break;
				default:
					AdSystemLog.LogError("The interstitial provider with ID " + allInterstitialProvider.providerID + " is not accounted for in the debug settings!");
					break;
				}
				if (flag)
				{
					if (AbstractAdProvider.IsProviderSupportedOnPlatform(allInterstitialProvider.providerID, adType))
					{
						list.Add(allInterstitialProvider);
						continue;
					}
					AdSystemLog.LogError(string.Concat("The provider with ID: ", allInterstitialProvider.providerID, " and adType: ", adType, " is not supported on this platform."));
				}
			}
		}
		if (Globals.DEBUG_VIDEOADS_PROVIDERS && adType == AdType.Video)
		{
			foreach (AdProviderInfo allVideoProvider in _allVideoProviders)
			{
				bool flag2 = false;
				switch (allVideoProvider.providerID)
				{
				case "adcolony":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_ADCOLONY;
					break;
				case "supersonic":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_SUPERSONIC;
					break;
				case "chartboost":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST;
					break;
				case "inmobi":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_INMOBI;
					break;
				case "tapjoy":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_TAPJOY;
					break;
				case "vungle":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_VUNGLE;
					break;
				case "facebook":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_FACEBOOK;
					break;
				case "mobvista":
					flag2 = Globals.DEBUG_VIDEOADS_PROVIDERS_MOBVISTA;
					break;
				default:
					AdSystemLog.LogError("The video provider with ID " + allVideoProvider.providerID + " is not accounted for in the debug settings!");
					break;
				}
				if (flag2)
				{
					if (AbstractAdProvider.IsProviderSupportedOnPlatform(allVideoProvider.providerID, adType))
					{
						list.Add(allVideoProvider);
						continue;
					}
					AdSystemLog.LogError(string.Concat("The provider with ID: ", allVideoProvider.providerID, " and adType: ", adType, " is not supported on this platform."));
				}
			}
		}
		return list;
	}

	public virtual void SetDebugPriorityListVersion(AdType adType, string value)
	{
		_debugPriorityListVersion[adType] = value;
	}

	private AdType? GetAdTypeForLocationID(string locationID)
	{
		AdType? result = null;
		foreach (AdLocationInfo adLocation in AdLocations.adLocations)
		{
			if (adLocation.id == locationID)
			{
				result = adLocation.adType;
			}
		}
		return result;
	}
}
