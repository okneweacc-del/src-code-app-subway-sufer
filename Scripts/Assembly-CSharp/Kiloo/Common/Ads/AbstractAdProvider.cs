using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace Kiloo.Common.Ads;

public abstract class AbstractAdProvider : AdEventListenerAdapter
{
	private enum InitState
	{
		Uninitialized = 0,
		Initializing = 1,
		Initialized = 2,
		Unsupported = 3
	}

	protected class EventListenerInvoker : IAdEventListener
	{
		private readonly List<IAdEventListener> _listeners = new List<IAdEventListener>();

		public void AddToStartOfList(IAdEventListener listener)
		{
			_listeners.Insert(0, listener);
		}

		public void AddListener(IAdEventListener listener)
		{
			_listeners.Add(listener);
		}

		public void RemoveListener(IAdEventListener listener)
		{
			_listeners.Remove(listener);
		}

		public void Clear()
		{
			_listeners.Clear();
		}

		public void WillShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.WillShowAd(adType, providerID, adLocationInfo);
			}
		}

		public void ShowedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.ShowedAd(adType, providerID, adLocationInfo);
			}
		}

		public void CachedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo, bool wasAlreadyCached)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.CachedAd(adType, providerID, adLocationInfo, wasAlreadyCached);
			}
		}

		public void DismissedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.DismissedAd(adType, providerID, adLocationInfo);
			}
		}

		public void StartedCachingAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.StartedCachingAd(adType, providerID, adLocationInfo);
			}
		}

		public void FinishedWatchingVideo(string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.FinishedWatchingVideo(providerID, adLocationInfo);
			}
		}

		public void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.FailedToCacheAd(adType, providerID, adLocationInfo);
			}
		}

		public void FailedToShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.FailedToShowAd(adType, providerID, adLocationInfo);
			}
		}

		public void FailedToInitAdProvider(string providerID)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.FailedToInitAdProvider(providerID);
			}
		}

		public void CachingTimedOut(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.CachingTimedOut(adType, providerID, adLocationInfo);
			}
		}

		public void ClickedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
		{
			foreach (IAdEventListener item in CloneListeners())
			{
				item.ClickedAd(adType, providerID, adLocationInfo);
			}
		}

		private List<IAdEventListener> CloneListeners()
		{
			return new List<IAdEventListener>(_listeners);
		}

		public void ReachedDailyInterstitialLimit(int limit)
		{
			throw new InvalidOperationException("This event should not be invoked by the ad provider!");
		}

		public void ReachedHourlyInterstitialLimit(int limit)
		{
			throw new InvalidOperationException("This event should not be invoked by the ad provider!");
		}

		public void ReachedNewUserInterstitialLimit(int limit)
		{
			throw new InvalidOperationException("This event should not be invoked by the ad provider!");
		}

		public void ReachedTimeBetweenAdsInterstitialLimit(int limit)
		{
			throw new InvalidOperationException("This event should not be invoked by the ad provider!");
		}

		public void ChangedProvider(AdType adType, string newProvider, string changedWaterfallID)
		{
			throw new InvalidOperationException("This event should not be invoked by the ad provider!");
		}

		public void PriorityListWrapped(string waterfallID)
		{
			throw new InvalidOperationException("This event should not be invoked by the ad provider!");
		}
	}

	protected static readonly EventListenerInvoker eventInvoker = new EventListenerInvoker();

	protected static readonly AdLocationInfo DEFAULT_SHOW_LOCATION = AdLocations.UNDETERMINED_LOCATION;

	protected AdLocationInfo _showInterstitialLocation = DEFAULT_SHOW_LOCATION;

	protected AdLocationInfo _showVideoLocation = DEFAULT_SHOW_LOCATION;

	protected AdLocationInfo _showLocation = DEFAULT_SHOW_LOCATION;

	protected readonly Dictionary<string, AdLocationInfo> _cacheIDToLocation = new Dictionary<string, AdLocationInfo>();

	private static readonly Dictionary<string, AbstractAdProvider> _idToAbstractAdProvider = new Dictionary<string, AbstractAdProvider>();

	private static readonly Dictionary<string, AbstractAdProvider> _idToDebugAdProvider = new Dictionary<string, AbstractAdProvider>();

	private InitState _initState;

	private bool? _isLoadedAdConstantsRestricted;

	private static IAdSettingsProvider _settingsProvider;

	private bool _hasCheckedPlatformSupport;

	protected readonly Dictionary<AdType, bool> _supportedAdTypes = new Dictionary<AdType, bool>();

	private AdType[] _allAdTypes = new AdType[2]
	{
		AdType.Interstitial,
		AdType.Video
	};

	private static Dictionary<string, AbstractAdProvider> idToAdProvider
	{
		get
		{
			if (Globals.DEBUG_USE_DEBUG_AD_PROVIDER)
			{
				return _idToDebugAdProvider;
			}
			return _idToAbstractAdProvider;
		}
	}

	public abstract string id { get; }

	public abstract bool supportsMulticaching { get; }

	public virtual List<AdType> adTypesWithClickCallbackSupport => new List<AdType>();

	public bool isCaching { get; private set; }

	public bool isInitializing => _initState == InitState.Initializing;

	public bool isInitialized => _initState == InitState.Initialized;

	protected abstract void CheckPlatformSupport(AdProviderConfiguration configuration);

	private void Initialize(AdProviderConfiguration configuration)
	{
		if (_initState == InitState.Uninitialized)
		{
			if (!IsAnyTypeSupportedOnPlatform())
			{
				_initState = InitState.Unsupported;
				eventInvoker.FailedToInitAdProvider(id);
			}
			else
			{
				_initState = InitState.Initializing;
				_isLoadedAdConstantsRestricted = configuration.isRestricted;
				OnInitialize(configuration);
			}
		}
	}

	public bool IsSupportedOnPlatform(AdType adType)
	{
		if (!_hasCheckedPlatformSupport)
		{
			CheckPlatformSupport(_settingsProvider.GetConfigurationForProvider(id));
			CheckHasCacheIDs(_settingsProvider.GetProviderCacheIDs(id));
			CheckOnlineSettingRestriction();
			_hasCheckedPlatformSupport = true;
		}
		bool value;
		return _supportedAdTypes.TryGetValue(adType, out value) && value;
	}

	private void CheckHasCacheIDs(Dictionary<AdLocationInfo, string> cacheIDs)
	{
		Dictionary<AdType, bool> dictionary = new Dictionary<AdType, bool>();
		foreach (KeyValuePair<AdLocationInfo, string> cacheID in cacheIDs)
		{
			if (!string.IsNullOrEmpty(cacheID.Key.id) && !string.IsNullOrEmpty(cacheID.Value))
			{
				dictionary[cacheID.Key.adType] = true;
			}
		}
		AdType[] allAdTypes = _allAdTypes;
		foreach (AdType key in allAdTypes)
		{
			if (!dictionary.TryGetValue(key, out var value) || !value)
			{
				_supportedAdTypes[key] = false;
			}
		}
	}

	private void CheckOnlineSettingRestriction()
	{
		AdType[] allAdTypes = _allAdTypes;
		foreach (AdType adType in allAdTypes)
		{
			if (!_supportedAdTypes.TryGetValue(adType, out var value) || !value || !_settingsProvider.TryGetProviderOverrideSupportedOnPlatform(adType, id, out var supportedDeviceVersion))
			{
				continue;
			}
			string oSVersion = DeviceUtility.GetOSVersion();
			try
			{
				if (global::Utility.CompareVersions(oSVersion, supportedDeviceVersion) < 0)
				{
					_supportedAdTypes[adType] = false;
				}
			}
			catch (FormatException ex)
			{
				AdSystemLog.LogError("CheckOnlineSettingRestriction: Failed to parse versions for comparison: " + oSVersion + " and " + supportedDeviceVersion + "  : " + ex);
			}
		}
	}

	private bool IsAnyTypeSupportedOnPlatform()
	{
		AdType[] allAdTypes = _allAdTypes;
		foreach (AdType adType in allAdTypes)
		{
			if (IsSupportedOnPlatform(adType))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void OnInitialize(AdProviderConfiguration configuration)
	{
	}

	protected void FinishInitialization(bool success)
	{
		if (success)
		{
			_initState = InitState.Initialized;
			eventInvoker.AddToStartOfList(this);
		}
		else
		{
			_initState = InitState.Uninitialized;
			eventInvoker.FailedToInitAdProvider(id);
		}
	}

	public static void ClearStaticCollections()
	{
		idToAdProvider.Clear();
		eventInvoker.Clear();
	}

	public static void MapAdProviders(IAdSettingsProvider settingsProvider)
	{
		_settingsProvider = settingsProvider;
		if (_idToDebugAdProvider.Count <= 0)
		{
			MapDebugAdProvider();
		}
		if (_idToAbstractAdProvider.Count <= 0)
		{
			MapAbstractAdProviders(settingsProvider);
		}
	}

	private static void MapDebugAdProvider()
	{
		foreach (Type abstractAdProviderSubType in GetAbstractAdProviderSubTypes())
		{
			AbstractAdProvider abstractAdProvider = (AbstractAdProvider)Activator.CreateInstance(abstractAdProviderSubType);
			DebugAdProvider value = new DebugAdProvider(abstractAdProvider.id, abstractAdProvider.supportsMulticaching);
			_idToDebugAdProvider.Add(abstractAdProvider.id, value);
		}
	}

	private static void MapAbstractAdProviders(IAdSettingsProvider settingsProvider)
	{
		string[] adProvidersToInitialize = settingsProvider.GetAdProvidersToInitialize();
		foreach (Type abstractAdProviderSubType in GetAbstractAdProviderSubTypes())
		{
			AbstractAdProvider abstractAdProvider = (AbstractAdProvider)Activator.CreateInstance(abstractAdProviderSubType);
			if (adProvidersToInitialize.Contains(abstractAdProvider.id))
			{
				AdProviderConfiguration configurationForProvider = settingsProvider.GetConfigurationForProvider(abstractAdProvider.id);
				abstractAdProvider.Initialize(configurationForProvider);
			}
			_idToAbstractAdProvider.Add(abstractAdProvider.id, abstractAdProvider);
		}
	}

	private static IEnumerable<Type> GetAbstractAdProviderSubTypes()
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			IEnumerable<Type> subTypes = from t in assembly.GetTypes()
				where t.IsSubclassOf(typeof(AbstractAdProvider))
				select t;
			foreach (Type subType in subTypes)
			{
				if (subType != typeof(DebugAdProvider))
				{
					yield return subType;
				}
			}
		}
	}

	public static void HandleAdConstantsChanged()
	{
		foreach (KeyValuePair<string, AbstractAdProvider> item in idToAdProvider)
		{
			if (item.Value.isInitialized || item.Value.isInitializing)
			{
				AdProviderConfiguration configurationForProvider = _settingsProvider.GetConfigurationForProvider(item.Key);
				bool? isLoadedAdConstantsRestricted = item.Value._isLoadedAdConstantsRestricted;
				if (isLoadedAdConstantsRestricted != true && isLoadedAdConstantsRestricted.HasValue && configurationForProvider.isRestricted)
				{
					item.Value._initState = InitState.Unsupported;
				}
			}
		}
	}

	public static bool IsProviderSupportedOnPlatform(string id, AdType adType)
	{
		if (idToAdProvider.TryGetValue(id, out var value))
		{
			return value.IsSupportedOnPlatform(adType);
		}
		AdSystemLog.LogError("Unknown provider ID " + id);
		return false;
	}

	public static AbstractAdProvider GetAdProvider(string id)
	{
		AbstractAdProvider abstractAdProvider;
		if (idToAdProvider.ContainsKey(id))
		{
			abstractAdProvider = idToAdProvider[id];
			AdProviderConfiguration configurationForProvider = _settingsProvider.GetConfigurationForProvider(id);
			abstractAdProvider.Initialize(configurationForProvider);
		}
		else if (Application.isEditor)
		{
			abstractAdProvider = new DebugAdProvider(id, supportsMulticahing: false);
			abstractAdProvider.Initialize(new AdProviderConfiguration());
			idToAdProvider[id] = abstractAdProvider;
		}
		else
		{
			AdSystemLog.LogError("Unknown provider ID " + id);
			abstractAdProvider = null;
		}
		return abstractAdProvider;
	}

	public static void AddEventListener(IAdEventListener listener)
	{
		eventInvoker.AddListener(listener);
	}

	public static void RemoveEventListener(IAdEventListener listener)
	{
		eventInvoker.RemoveListener(listener);
	}

	public bool HasCachedAd(AdType adType, string cacheID)
	{
		if (!isInitialized || string.IsNullOrEmpty(cacheID))
		{
			return false;
		}
		return (adType != AdType.Video) ? HasCachedInterstitial(cacheID) : HasCachedVideo(cacheID);
	}

	public bool IsCachingLocation(AdLocationInfo info)
	{
		return isCaching && _cacheIDToLocation.ContainsValue(info);
	}

	public void CacheAd(AdType adType, AdLocationInfo adLocationInfo, string cacheID)
	{
		if (!isInitialized || string.IsNullOrEmpty(cacheID))
		{
			AdSystemLog.LogError("Unable to cache ad, provider is not initialized or cacheID is null");
			eventInvoker.FailedToCacheAd(adType, id, adLocationInfo);
		}
		else
		{
			if (!supportsMulticaching && isCaching)
			{
				return;
			}
			if (HasCachedAd(adType, cacheID))
			{
				eventInvoker.CachedAd(adType, id, adLocationInfo, wasAlreadyCached: true);
				return;
			}
			isCaching = true;
			if (adType == AdType.Video)
			{
				CacheVideo(adLocationInfo, cacheID);
			}
			else
			{
				CacheInterstitial(adLocationInfo, cacheID);
			}
		}
	}

	public bool ShowAd(AdType adType, AdLocationInfo adLocationInfo, string cacheID)
	{
		if (!isInitialized || string.IsNullOrEmpty(cacheID))
		{
			AdSystemLog.LogError("Unable to show ad, provider is not initialized or cacheID is null");
			eventInvoker.FailedToShowAd(adType, id, adLocationInfo);
			return false;
		}
		if (!HasCachedAd(adType, cacheID))
		{
			eventInvoker.FailedToShowAd(adType, id, adLocationInfo);
			return false;
		}
		eventInvoker.WillShowAd(adType, id, adLocationInfo);
		if (adType == AdType.Video)
		{
			ShowVideo(adLocationInfo, cacheID);
		}
		else
		{
			ShowInterstitial(adLocationInfo, cacheID);
		}
		return true;
	}

	public void TimeoutCaching(AdType adType, AdLocationInfo adLocationInfo)
	{
		eventInvoker.CachingTimedOut(adType, id, adLocationInfo);
		OnCacheTimeout(adType, adLocationInfo);
	}

	public void FailCaching(AdType adType, AdLocationInfo adLocationInfo)
	{
		eventInvoker.FailedToCacheAd(adType, id, adLocationInfo);
	}

	protected virtual void OnCacheTimeout(AdType adType, AdLocationInfo adLocationInfo)
	{
	}

	protected virtual void ShowInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		throw new NotSupportedException("Ad provider " + id + ", does not support interstitials");
	}

	protected virtual void CacheInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		throw new NotSupportedException("Ad provider " + id + ", does not support interstitials");
	}

	protected virtual void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		throw new NotSupportedException("Ad provider " + id + ", does not support videos");
	}

	protected virtual void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		throw new NotSupportedException("Ad provider " + id + ", does not support videos");
	}

	protected virtual bool HasCachedVideo(string cacheID)
	{
		throw new NotSupportedException("Ad provider " + id + ", does not support videos");
	}

	protected virtual bool HasCachedInterstitial(string cacheID)
	{
		throw new NotSupportedException("Ad provider " + id + ", does not support interstitials");
	}

	public override void CachedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo, bool wasAlreadyCached)
	{
		OnFinishedCaching(providerID);
	}

	public override void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		OnFinishedCaching(providerID);
	}

	public override void CachingTimedOut(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		OnFinishedCaching(providerID);
	}

	private void OnFinishedCaching(string providerID)
	{
		if (providerID == id)
		{
			isCaching = false;
		}
	}
}
