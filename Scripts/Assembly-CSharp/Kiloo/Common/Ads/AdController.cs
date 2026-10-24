using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdHandling.AdConstants;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using UI.Popups;
using UnityEngine;

namespace Kiloo.Common.Ads;

public class AdController
{
	private class CacheList
	{
		public readonly List<AdLocation> list;

		public readonly int capacity;

		private int _locationToShowNext;

		public int count => list.Count;

		public CacheList(int capacity)
		{
			this.capacity = capacity;
			list = new List<AdLocation>(capacity);
		}

		public void Add(AdLocation location)
		{
			list.Add(location);
		}

		public AdLocation GetLocationToShow()
		{
			return list[_locationToShowNext];
		}

		public void AdvanceShowPointer()
		{
			_locationToShowNext++;
			if (_locationToShowNext >= count)
			{
				_locationToShowNext = 0;
			}
		}
	}

	public class EventHandler : AdEventListenerAdapter
	{
		private AdController _controller;

		public EventHandler(AdController controller)
		{
			_controller = controller;
		}

		public override void ShowedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			if (adType == AdType.Video)
			{
				_controller._timeOfLastVideo = DateTime.Now;
			}
			else
			{
				_controller._timeOfLastInterstitial = DateTime.Now;
				_controller._interstitialStats.IncrementShowCount();
				if (_controller._interstitialStats.ReachedDailyLimit(_controller._settings))
				{
					foreach (IAdEventListener eventListener in _controller._eventListeners)
					{
						eventListener.ReachedDailyInterstitialLimit(_controller._settings.GetMaxInterstitialImpressionsPerDay());
					}
				}
				else if (_controller._interstitialStats.ReachedHourlyLimit(_controller._settings))
				{
					foreach (IAdEventListener eventListener2 in _controller._eventListeners)
					{
						eventListener2.ReachedHourlyInterstitialLimit(_controller._settings.GetMaxInterstitialImpressionsPerHour());
					}
				}
			}
			AdLocation adLocationFromID = _controller.GetAdLocationFromID(adLocationInfo.id);
			if (adLocationFromID == null)
			{
				AdSystemLog.LogError("Unknown ad location " + adLocationInfo.id);
				return;
			}
			PriorityList priorityList = _controller._idToPriorityList[adLocationFromID.waterfallID];
			priorityList.IncrementNumberOfImpressions();
		}

		public override void DismissedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			if (adType == AdType.Interstitial)
			{
				_controller._timeOfLastInterstitial = DateTime.Now;
			}
			CoroutineUtil.StopCoroutine(_controller._checkCachesCoroutine);
			_controller._checkCachesCoroutine = CoroutineUtil.StartCoroutine(_controller.CheckCacheExpiredCoroutine());
		}

		public override void FailedToInitAdProvider(string providerID)
		{
			AdSystemLog.LogError("Failed to initialize ad provider " + providerID);
		}

		public override void CachingTimedOut(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			AdSystemLog.LogError("Location: " + adLocationInfo.id + " | provider:" + providerID + " timed out!");
			HandleFail(providerID, adLocationInfo);
		}

		public override void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			AdSystemLog.LogError("Failed to cache " + adLocationInfo.id + ", " + providerID);
			HandleFail(providerID, adLocationInfo);
		}

		public override void CachedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo, bool wasAlreadyCached)
		{
		}

		private bool IncrementFailCount(string waterfallID)
		{
			return _controller._idToPriorityList[waterfallID].IncrementNumberOfFails();
		}

		public override void FailedToShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			AdSystemLog.LogError("Failed to show " + adLocationInfo.id + ", " + providerID);
			CoroutineUtil.StopCoroutine(_controller._checkCachesCoroutine);
			_controller._checkCachesCoroutine = CoroutineUtil.StartCoroutine(_controller.CheckCacheExpiredCoroutine());
			HandleFail(providerID, adLocationInfo);
		}

		private void HandleFail(string providerID, AdLocationInfo adLocationInfo)
		{
			string currentProviderIDForLocation = _controller.GetCurrentProviderIDForLocation(adLocationInfo);
			if (currentProviderIDForLocation != providerID)
			{
				return;
			}
			AdLocation adLocationFromID = _controller.GetAdLocationFromID(adLocationInfo.id);
			if (adLocationFromID == null)
			{
				AdSystemLog.LogError("Unknown ad location " + adLocationInfo.id);
				return;
			}
			PriorityList priorityList = _controller._idToPriorityList[adLocationFromID.waterfallID];
			bool reachedLastProvider = priorityList.reachedLastProvider;
			if (IncrementFailCount(adLocationFromID.waterfallID) && !reachedLastProvider && _controller.allowReCacheOnFail)
			{
				_controller.Cache(adLocationFromID);
			}
		}
	}

	private readonly IAdSettingsProvider _settings;

	private readonly List<AdLocation> _adLocations;

	private readonly Dictionary<string, PriorityList> _idToPriorityList = new Dictionary<string, PriorityList>();

	private DateTime _timeOfLastVideo = DateTime.MinValue;

	private DateTime _timeOfLastInterstitial = DateTime.MinValue;

	private InterstitialStats _interstitialStats;

	private List<IAdEventListener> _eventListeners = new List<IAdEventListener>();

	private const int DEFAULT_NUMBER_OF_VIDEO_ADS_TO_CACHE = 2;

	private const int DEFAULT_NUMBER_OF_INTERSTITIALS_TO_CACHE = 1;

	private const string ONLINE_SETTINGS_KEY_NUMBER_OF_VIDEO_ADS_TO_CACHE = "video_ads_cache_pool_size";

	private const string ONLINE_SETTINGS_KEY_NUMBER_OF_INTERSTITIALS_TO_CACHE = "interstitial_ads_cache_pool_size";

	private const float DEFAULT_AD_EXPIRY_MIN_CHECK_INTERVAL = 10f;

	private const float DEFAULT_AD_EXPIRY_MAX_CHECK_INTERVAL = 30f;

	private const string ONLINE_SETTINGS_KEY_AD_MIN_EXPIRY_CHECK_INTERVAL = "ad_expiry_min_check_interval";

	private const string ONLINE_SETTINGS_KEY_AD_MAX_EXPIRY_CHECK_INTERVAL = "ad_expiry_max_check_interval";

	private readonly Dictionary<AdType, CacheList> _cacheLists;

	private Coroutine _checkCachesCoroutine;

	private static float adExpiryMaxCheckInterval => OnlineSettings.instance.GetValue("ad_expiry_max_check_interval", 30f);

	private static float adExpiryMinCheckInterval => OnlineSettings.instance.GetValue("ad_expiry_min_check_interval", 10f);

	public bool allowReCacheOnFail { get; set; }

	public AdController(IAdSettingsProvider settings, ICollection<AdLocationInfo> adLocations, InterstitialStats interstitialStats)
	{
		_settings = settings;
		_adLocations = new List<AdLocation>(adLocations.Count);
		int value = OnlineSettings.instance.GetValue("video_ads_cache_pool_size", 2);
		int value2 = OnlineSettings.instance.GetValue("interstitial_ads_cache_pool_size", 1);
		_cacheLists = new Dictionary<AdType, CacheList>
		{
			{
				AdType.Video,
				new CacheList(value)
			},
			{
				AdType.Interstitial,
				new CacheList(value2)
			}
		};
		_interstitialStats = interstitialStats;
		allowReCacheOnFail = true;
		AbstractAdProvider.MapAdProviders(settings);
		AdConstants.constantsChanged += AbstractAdProvider.HandleAdConstantsChanged;
		foreach (AdLocationInfo adLocation2 in adLocations)
		{
			string waterfallID = GetWaterfallID(adLocation2);
			AdType adType = adLocation2.adType;
			if (waterfallID != null)
			{
				if (!_idToPriorityList.ContainsKey(waterfallID))
				{
					PriorityList value3 = CreatePriorityListForID(waterfallID, adType, settings);
					_idToPriorityList.Add(waterfallID, value3);
				}
				AdLocation adLocation = new AdLocation(adLocation2, waterfallID);
				AbstractAdProvider currentProvider = GetCurrentProvider(adLocation);
				adLocation.SetSettingsProvider(settings);
				adLocation.SetAdProvider(currentProvider);
				_adLocations.Add(adLocation);
				CacheList cacheList = _cacheLists[adType];
				if (cacheList.count < cacheList.capacity && adLocation.info.isPoolLocation)
				{
					cacheList.Add(adLocation);
				}
				AddEventListener(adLocation);
			}
		}
		foreach (KeyValuePair<AdType, CacheList> cacheList2 in _cacheLists)
		{
			if (cacheList2.Value.count < cacheList2.Value.capacity)
			{
				AdSystemLog.LogError($"{cacheList2.Key} Cache Pool Size set to {cacheList2.Value.capacity} but only {cacheList2.Value.count} locations found!");
			}
		}
		_checkCachesCoroutine = CoroutineUtil.StartCoroutine(CheckCacheExpiredCoroutine());
	}

	public void UpdateInterstitialStats(InterstitialStats stats)
	{
		_interstitialStats = stats;
	}

	public void AddEventListener(IAdEventListener eventListener)
	{
		AbstractAdProvider.AddEventListener(eventListener);
		_eventListeners.Add(eventListener);
	}

	public void RemoveEventListener(IAdEventListener eventListener)
	{
		AbstractAdProvider.RemoveEventListener(eventListener);
		_eventListeners.Remove(eventListener);
	}

	public bool IsCachedDebug(AdLocationInfo adLocationInfo)
	{
		return GetAdLocationFromID(adLocationInfo.id)?.IsCached() ?? false;
	}

	public bool IsCached(AdLocationInfo adLocationInfo)
	{
		if (!_settings.GetAvailabilityForLocation(adLocationInfo.id))
		{
			return false;
		}
		return GetCachedPoolLocations(adLocationInfo).Count > 0;
	}

	private List<AdLocation> GetCachedPoolLocations(AdLocationInfo adLocationInfo)
	{
		return _cacheLists[adLocationInfo.adType].list.FindAll((AdLocation location) => location.IsCached());
	}

	public bool IsCaching(AdLocationInfo adLocationInfo)
	{
		bool result = false;
		AdLocation adLocationFromID = GetAdLocationFromID(adLocationInfo.id);
		if (adLocationFromID != null)
		{
			if (_settings.GetAvailabilityForLocation(adLocationInfo.id))
			{
				result = adLocationFromID.isCaching;
			}
		}
		else
		{
			AdSystemLog.LogError("Unknown ad location ID " + adLocationInfo.id);
		}
		return result;
	}

	private IEnumerator CheckCacheExpiredCoroutine()
	{
		float adExpiryCheckInterval = adExpiryMinCheckInterval;
		while (true)
		{
			yield return new WaitForSeconds(adExpiryCheckInterval);
			bool foundExpiredAd = false;
			foreach (CacheList value in _cacheLists.Values)
			{
				foreach (AdLocation item in value.list)
				{
					if (_settings.GetAvailabilityForLocation(item.info.id) && !item.isCaching && !item.IsCached())
					{
						item.Cache();
						foundExpiredAd = true;
						break;
					}
				}
			}
			adExpiryCheckInterval = ((!foundExpiredAd) ? adExpiryMaxCheckInterval : adExpiryMinCheckInterval);
		}
	}

	public void Cache(AdLocation specificLocation = null)
	{
		foreach (KeyValuePair<AdType, CacheList> cacheList in _cacheLists)
		{
			foreach (AdLocation item in cacheList.Value.list)
			{
				if ((specificLocation == null || item == specificLocation) && _settings.GetAvailabilityForLocation(item.info.id) && (DebugSettingsPopup.forceRefreshAdSystem || !item.IsCached()))
				{
					_idToPriorityList[item.waterfallID].wrapped = false;
					if (HasPriorityListChanged(item))
					{
						UpdatePriorityListForID(item.waterfallID, item.info.adType);
					}
					AbstractAdProvider currentProvider = GetCurrentProvider(item);
					if (currentProvider == null && !Globals.DEBUG_VIDEOADS_PROVIDERS && !Globals.DEBUG_INTERSTITIAL_PROVIDERS)
					{
						return;
					}
					item.SetAdProvider(currentProvider);
					item.Cache();
				}
			}
		}
	}

	public bool Show(AdLocationInfo adLocationInfo)
	{
		AdLocation adLocationFromID = GetAdLocationFromID(adLocationInfo.id);
		if (adLocationFromID == null)
		{
			AdSystemLog.LogError("Unknown ad location ID: " + adLocationInfo.id);
			return false;
		}
		if (!_settings.GetAvailabilityForLocation(adLocationInfo.id))
		{
			AdSystemLog.LogWarning("Show called on location: " + adLocationInfo.id + " but it has been disabled.");
			return false;
		}
		if (adLocationFromID.info.adType == AdType.Interstitial)
		{
			if (Application.isEditor && !Globals.DEBUG_INTERSTITIAL_IN_EDITOR)
			{
				return false;
			}
			if (!_settings.AreInterstitialsAllowed())
			{
				return false;
			}
			if (_interstitialStats.ReachedLimit(_settings))
			{
				return false;
			}
			if (!_interstitialStats.hasSeenFirstInterstitial && CDateTime.UtcNow < _settings.GetScheduledFirstInterstitialShowTime())
			{
				foreach (IAdEventListener eventListener in _eventListeners)
				{
					eventListener.ReachedNewUserInterstitialLimit((int)_settings.GetGracePeriodValue());
				}
				return false;
			}
		}
		TimeSpan minimumShowInterval = _settings.GetMinimumShowInterval(adLocationFromID.info.adType);
		DateTime dateTime = ((adLocationFromID.info.adType != AdType.Video) ? _timeOfLastInterstitial : _timeOfLastVideo);
		bool flag = adLocationFromID.info.adType != AdType.Video && Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT;
		if (DateTime.Now > dateTime + minimumShowInterval || dateTime > DateTime.Now || flag)
		{
			AdLocation adLocation = null;
			CacheList cacheList = _cacheLists[adLocationFromID.info.adType];
			if (!cacheList.list.Exists((AdLocation location) => location.IsCached()))
			{
				string eventName = $"Ad Location: {adLocationInfo.id}";
				string argValue = string.Format("{0};{1}", "null", "No cached ad");
				Flurry.LogEventWithAParameter(eventName, "Outcome", argValue);
				AdSystemLog.LogError("Show called from location: " + adLocationInfo.id + ", but no video ads are cached!");
				Cache();
				return false;
			}
			for (int num = 0; num < cacheList.count; num++)
			{
				if (adLocation != null && adLocation.IsCached())
				{
					break;
				}
				adLocation = cacheList.GetLocationToShow();
				cacheList.AdvanceShowPointer();
			}
			if (adLocation == null)
			{
				AdSystemLog.LogError("!!! Show called for location: " + adLocationInfo.id + " and though it wasn't caught by the no ad cached checks, no cached ad was found!");
				return false;
			}
			string eventName2 = $"Ad Location: {adLocationInfo.id}";
			string argValue2 = string.Format("{0};{1}", adLocation.GetCurrentAdProviderId(), "Success");
			Flurry.LogEventWithAParameter(eventName2, "Outcome", argValue2);
			if (adLocation.Show())
			{
				CoroutineUtil.StopCoroutine(_checkCachesCoroutine);
				return true;
			}
			return false;
		}
		foreach (IAdEventListener eventListener2 in _eventListeners)
		{
			eventListener2.ReachedTimeBetweenAdsInterstitialLimit((int)minimumShowInterval.TotalSeconds);
		}
		return false;
	}

	public void ChangeToNextProvider()
	{
		foreach (PriorityList value in _idToPriorityList.Values)
		{
			value.ChangeToNextProvider();
		}
	}

	public string GetInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (AdLocation adLocation in _adLocations)
		{
			AbstractAdProvider currentProvider = GetCurrentProvider(adLocation);
			stringBuilder.AppendLine("Loc: " + adLocation.info.id + " Provider:" + ((currentProvider == null) ? "none" : currentProvider.id));
			stringBuilder.AppendLine("Cache status: " + adLocation.IsCached());
		}
		return stringBuilder.ToString();
	}

	public void SetDebugPriorityListVersion(AdType adtype, string version)
	{
		_settings.SetDebugPriorityListVersion(adtype, version);
	}

	public string GetCurrentProviderIDForLocation(AdLocationInfo locationInfo)
	{
		string result = string.Empty;
		AdLocation adLocationFromID = GetAdLocationFromID(locationInfo.id);
		if (adLocationFromID != null)
		{
			AbstractAdProvider currentProvider = GetCurrentProvider(adLocationFromID);
			if (currentProvider != null)
			{
				result = currentProvider.id;
			}
		}
		return result;
	}

	public string GetHighestPriorityProviderWithCachedAd(AdLocationInfo adLocationInfo)
	{
		AdLocation adLocation = GetCachedPoolLocations(adLocationInfo).FirstOrDefault();
		if (adLocation != null)
		{
			return adLocation.GetCurrentAdProviderId();
		}
		return string.Empty;
	}

	public void SetProviderForAdLocation(AdLocationInfo adLocationInfo, AbstractAdProvider provider)
	{
		AdLocation adLocation = _adLocations.Find((AdLocation item) => item.info.id == adLocationInfo.id);
		adLocation.SetAdProvider(provider);
	}

	public uint GetGracePeriodValue()
	{
		return _settings.GetGracePeriodValue();
	}

	public bool GetAvailabilityForLocation(string locationID)
	{
		AdLocation adLocationFromID = GetAdLocationFromID(locationID);
		if (adLocationFromID == null)
		{
			return false;
		}
		return _settings.GetAvailabilityForLocation(adLocationFromID.info.id);
	}

	public bool AreInterstitialsAllowed()
	{
		return _settings.AreInterstitialsAllowed();
	}

	public bool HasPriorityListWrapped(string locationID)
	{
		AdLocation adLocationFromID = GetAdLocationFromID(locationID);
		return adLocationFromID != null && _idToPriorityList[adLocationFromID.waterfallID].wrapped;
	}

	private bool HasPriorityListChanged(AdLocation adLocation)
	{
		string latestVersionForWaterfall = _settings.GetLatestVersionForWaterfall(adLocation);
		return _interstitialStats.HasVersionChangedForList(adLocation.waterfallID, latestVersionForWaterfall);
	}

	private void UpdatePriorityListForID(string waterfallID, AdType adType)
	{
		if (_idToPriorityList.ContainsKey(waterfallID))
		{
			_idToPriorityList[waterfallID] = CreatePriorityListForID(waterfallID, adType, _settings);
		}
	}

	public void UpdateCacheIdsFromOnlineSettings()
	{
		_settings.UpdateProviderCacheIDs();
	}

	private PriorityList CreatePriorityListForID(string waterfallID, AdType adType, IAdSettingsProvider settings)
	{
		PriorityList list = new PriorityList(waterfallID, adType, settings);
		list.changedProvider += () =>
		{
			foreach (IAdEventListener eventListener in _eventListeners)
			{
				eventListener.ChangedProvider(adType, list.currentProvider, waterfallID);
			}
		};
		list.resetToTop += () =>
		{
			foreach (IAdEventListener eventListener2 in _eventListeners)
			{
				eventListener2.PriorityListWrapped(waterfallID);
			}
		};
		return list;
	}

	[CanBeNull]
	public string GetWaterfallID(AdLocationInfo adLocation)
	{
		string waterfallID = _settings.GetWaterfallID(adLocation.id);
		if (!string.IsNullOrEmpty(waterfallID))
		{
			return waterfallID;
		}
		return _settings.GetWaterfallID(adLocation.adType);
	}

	[CanBeNull]
	private AbstractAdProvider GetCurrentProvider(AdLocation adLocation)
	{
		if (!_idToPriorityList.TryGetValue(adLocation.waterfallID, out var value) || !value.hasProviders)
		{
			return null;
		}
		value.CheckIfProviderExpired();
		return AbstractAdProvider.GetAdProvider(value.currentProvider);
	}

	[CanBeNull]
	public AdLocation GetAdLocationFromID(string adLocationID)
	{
		AdLocation adLocation = _adLocations.FirstOrDefault((AdLocation l) => l.info.id == adLocationID);
		if (adLocation == null)
		{
			AdSystemLog.LogError("Could not find an ad location with the specified ID: " + adLocationID);
		}
		return adLocation;
	}
}
