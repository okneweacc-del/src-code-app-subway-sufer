using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using UnityEngine;

public class AdSystemManager : AdEventListenerAdapter
{
	public const string LAST_SHOWED_PROVIDER_SAVED_SUFFIX = "_(saved)";

	private const string ONLINE_SETTINGS_ALLOW_RECACHE_DURING_RUN_KEY = "allow_recache_during_run";

	private bool _isInitialized;

	private bool _showAllowed = true;

	private AdController _adController;

	private AdLocationInfo _currentlyShowingAdAtLocation;

	private readonly Dictionary<string, string> _lastShowedProviderPerLocation = new Dictionary<string, string>();

	private Dictionary<AdType, string> _lastShowedLocationsPerAdType;

	private ScreenOrientation _currentScreenOrientation = ScreenOrientation.Portrait;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onAdSystemInitialized__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onWillShowVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onShowedVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onFailedToShowVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onDismissedVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo, bool> onCachedVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onFailedToCacheVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onFinishedWatchingVideo__BackingField;

	private Action _videoReward;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onWillShowInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onCachedInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onFailedToCacheInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onShowedInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onFailedToShowInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdLocationInfo> onDismissedInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onPriorityListWrapped__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onChangedProvider__BackingField;

	private static AdSystemManager _instance;

	public static bool isInitialized => _instance != null && _instance._isInitialized;

	public static event Action onAdSystemInitialized
	{
		add
		{
			Action action = onAdSystemInitialized__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onAdSystemInitialized__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onAdSystemInitialized__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onAdSystemInitialized__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onWillShowVideo
	{
		add
		{
			Action<AdLocationInfo> action = onWillShowVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowVideo__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onWillShowVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowVideo__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onShowedVideo
	{
		add
		{
			Action<AdLocationInfo> action = onShowedVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedVideo__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onShowedVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedVideo__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onFailedToShowVideo
	{
		add
		{
			Action<AdLocationInfo> action = onFailedToShowVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowVideo__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onFailedToShowVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowVideo__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onDismissedVideo
	{
		add
		{
			Action<AdLocationInfo> action = onDismissedVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedVideo__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onDismissedVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedVideo__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo, bool> onCachedVideo
	{
		add
		{
			Action<AdLocationInfo, bool> action = onCachedVideo__BackingField;
			Action<AdLocationInfo, bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedVideo__BackingField, (Action<AdLocationInfo, bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo, bool> action = onCachedVideo__BackingField;
			Action<AdLocationInfo, bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedVideo__BackingField, (Action<AdLocationInfo, bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onFailedToCacheVideo
	{
		add
		{
			Action<AdLocationInfo> action = onFailedToCacheVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheVideo__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onFailedToCacheVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheVideo__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onFinishedWatchingVideo
	{
		add
		{
			Action<AdLocationInfo> action = onFinishedWatchingVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFinishedWatchingVideo__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onFinishedWatchingVideo__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFinishedWatchingVideo__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onWillShowInterstitial
	{
		add
		{
			Action<AdLocationInfo> action = onWillShowInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onWillShowInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onCachedInterstitial
	{
		add
		{
			Action<AdLocationInfo> action = onCachedInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onCachedInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onFailedToCacheInterstitial
	{
		add
		{
			Action<AdLocationInfo> action = onFailedToCacheInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onFailedToCacheInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onShowedInterstitial
	{
		add
		{
			Action<AdLocationInfo> action = onShowedInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onShowedInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onFailedToShowInterstitial
	{
		add
		{
			Action<AdLocationInfo> action = onFailedToShowInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onFailedToShowInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdLocationInfo> onDismissedInterstitial
	{
		add
		{
			Action<AdLocationInfo> action = onDismissedInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdLocationInfo> action = onDismissedInterstitial__BackingField;
			Action<AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedInterstitial__BackingField, (Action<AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onPriorityListWrapped
	{
		add
		{
			Action<string> action = onPriorityListWrapped__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPriorityListWrapped__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onPriorityListWrapped__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPriorityListWrapped__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onChangedProvider
	{
		add
		{
			Action<string> action = onChangedProvider__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onChangedProvider__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onChangedProvider__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onChangedProvider__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private AdSystemManager()
	{
		Initialize();
	}

	public static void CreateAdSystemManager()
	{
		if (_instance == null)
		{
			_instance = new AdSystemManager();
		}
		else
		{
			AdSystemLog.LogWarning("AdSystemManager.CreateAdSystemManager was called, but an instance had already been created! Ignoring.");
		}
	}

	private void Initialize()
	{
		if (AgeRestriction.waitForAgeRestriction)
		{
			AdSystemLog.LogError("AdSystemManager initialized before AgeRestriction has been setup!\n" + Environment.StackTrace);
		}
		_adController = new AdController(new AdSettingsProvider(), AdLocations.adLocations, AdsSave.interstitialStats);
		_adController.AddEventListener(this);
		_adController.AddEventListener(new AdSystemEventLogger());
		_adController.AddEventListener(new AdController.EventHandler(_adController));
		_adController.AddEventListener(new AdEventLogger());
		UIScreenController.OnWillShowScreen += HandleOnWillShowScreen;
		OnlineSettings instance = OnlineSettings.instance;
		instance.onOnlineSettingsDownloadResult = (Action<bool>)Delegate.Combine(instance.onOnlineSettingsDownloadResult, new Action<bool>(OnOnlineSettingsDownloadResult));
		UpdateCacheIdsFromOnlineSettings();
		LoadLastShowedProviders();
		AppRefreshHandler.hasRefreshed += AppHasRefreshed;
		_isInitialized = true;
		CoroutineUtil.StartCoroutine(CoroutineUtil.ExecuteActionNextFrame(() =>
		{
			onAdSystemInitialized__BackingField.SafeInvoke();
		}));
	}

	private void AppHasRefreshed(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.Data))
		{
			_adController.UpdateInterstitialStats(AdsSave.interstitialStats);
		}
	}

	private void OnOnlineSettingsDownloadResult(bool b)
	{
		UpdateCacheIdsFromOnlineSettings();
	}

	private void UpdateCacheIdsFromOnlineSettings()
	{
		_adController.UpdateCacheIdsFromOnlineSettings();
	}

	public static bool IsCachedDebug(AdLocationInfo adLocationInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsCachedDebug(adLocationInfo);
	}

	public static bool IsCached(AdLocationInfo info)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsCached(info);
	}

	public static bool IsCaching(AdLocationInfo adLocationInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsCaching(adLocationInfo);
	}

	public static void Cache()
	{
		if (_instance != null && _instance._isInitialized)
		{
			_instance._adController.Cache();
		}
	}

	public static void Show(AdLocationInfo adLocationInfo, Action videoPayout = null)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			AdSystemLog.LogError("Show called, but Ad System has not been initialised!");
		}
		else
		{
			if (!_instance._showAllowed)
			{
				return;
			}
			_instance._showAllowed = false;
			if (adLocationInfo.adType == AdType.Video)
			{
				_instance.SaveCurrentOrientation();
				_instance._videoReward = videoPayout;
			}
			if (Globals.DEBUG_SKIP_VIDEO_ADS && adLocationInfo.adType == AdType.Video)
			{
				_instance.SkipVideoAd();
				_instance._showAllowed = true;
				return;
			}
			_instance.ShowAd(adLocationInfo);
			CoroutineUtil.StartCoroutine(CoroutineUtil.ExecuteActionNextFrame(() =>
			{
				_instance._showAllowed = true;
			}));
		}
	}

	private void SkipVideoAd()
	{
		FinishedWatchingVideo(string.Empty, AdLocations.UNDETERMINED_LOCATION);
		DismissedAd(AdType.Video, string.Empty, AdLocations.UNDETERMINED_LOCATION);
	}

	private void ShowAd(AdLocationInfo adLocationInfo)
	{
		_currentlyShowingAdAtLocation = adLocationInfo;
		if (_adController.Show(adLocationInfo))
		{
			ShowTransitionOverlay(adLocationInfo.adType, adLocationInfo);
		}
	}

	public static string GetLastShowedProviderForLocation(AdLocationInfo locationInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			AdSystemLog.LogError("GetLastShowedProviderForLocation called, but Ad System has not been initialised!");
			return string.Empty;
		}
		_instance._lastShowedProviderPerLocation.TryGetValue(locationInfo.id, out var value);
		return value ?? string.Empty;
	}

	public static string GetLastPlayedLocation(AdType adType)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			AdSystemLog.LogError("GetLastPlayedLocation called, but Ad System has not been initialised!");
			return string.Empty;
		}
		_instance._lastShowedLocationsPerAdType.TryGetValue(adType, out var value);
		return value ?? string.Empty;
	}

	public static string GetCurrentProviderIDForLocation(AdLocationInfo locationInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return _instance._adController.GetCurrentProviderIDForLocation(locationInfo);
	}

	public static string GetHighestPriorityProviderWithCachedAd(AdLocationInfo adLocationInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return _instance._adController.GetHighestPriorityProviderWithCachedAd(adLocationInfo);
	}

	public static AdLocationInfo GetAdLocationInfoFromID(string adLocationID)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return AdLocations.UNDETERMINED_LOCATION;
		}
		AdLocation adLocationFromID = _instance._adController.GetAdLocationFromID(adLocationID);
		if (adLocationFromID == null)
		{
			AdSystemLog.LogError("Couldn't find ad location for ID: " + adLocationID);
			return AdLocations.UNDETERMINED_LOCATION;
		}
		return adLocationFromID.info;
	}

	public static bool GetAvailabilityForLocation(string locationID)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.GetAvailabilityForLocation(locationID);
	}

	public static bool AreInterstitialsAllowed()
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.AreInterstitialsAllowed();
	}

	public static bool HasPriorityListWrapped(string locationID)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.HasPriorityListWrapped(locationID);
	}

	public static bool HaveAllCachePoolPriorityListsWrapped(string locationID)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		AdLocationInfo adLocationInfoFromID = GetAdLocationInfoFromID(locationID);
		foreach (AdLocationInfo adLocation in AdLocations.adLocations)
		{
			if (adLocationInfoFromID.adType == adLocation.adType && GetAvailabilityForLocation(adLocation.id) && adLocation.isPoolLocation && !_instance._adController.HasPriorityListWrapped(adLocation.id))
			{
				return false;
			}
		}
		return true;
	}

	public override void WillShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		UpdateLastShowedProviders(adLocationInfo, providerID, shouldSave: false);
		UpdateLastShowedLocations(adType, adLocationInfo, shouldSave: true);
		switch (adType)
		{
		case AdType.Interstitial:
			onWillShowInterstitial__BackingField.SafeInvoke(adLocationInfo);
			break;
		case AdType.Video:
			onWillShowVideo__BackingField.SafeInvoke(adLocationInfo);
			break;
		}
	}

	public override void CachedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo, bool wasAlreadyCached)
	{
		switch (adType)
		{
		case AdType.Video:
			onCachedVideo__BackingField.SafeInvoke(adLocationInfo, wasAlreadyCached);
			break;
		case AdType.Interstitial:
			onCachedInterstitial__BackingField.SafeInvoke(adLocationInfo);
			break;
		}
	}

	public override void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		if (!(_adController.GetCurrentProviderIDForLocation(adLocationInfo) != providerID))
		{
			switch (adType)
			{
			case AdType.Video:
				onFailedToCacheVideo__BackingField.SafeInvoke(adLocationInfo);
				break;
			case AdType.Interstitial:
				onFailedToCacheInterstitial__BackingField.SafeInvoke(adLocationInfo);
				break;
			}
		}
	}

	public override void ShowedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		UpdateLastShowedProviders(adLocationInfo, providerID, shouldSave: false);
		UpdateLastShowedLocations(adType, adLocationInfo, shouldSave: false);
		ShowOverlay(adType, adLocationInfo);
		switch (adType)
		{
		case AdType.Video:
			DisableAudio();
			onShowedVideo__BackingField.SafeInvoke(adLocationInfo);
			break;
		case AdType.Interstitial:
			onShowedInterstitial__BackingField.SafeInvoke(adLocationInfo);
			break;
		}
	}

	public override void FinishedWatchingVideo(string providerID, AdLocationInfo adLocationInfo)
	{
		HideOverlay(AdType.Video);
		EnableAudio();
		GiveReward();
		onFinishedWatchingVideo__BackingField.SafeInvoke(adLocationInfo);
		if (adLocationInfo != AdLocations.UNDETERMINED_LOCATION)
		{
			Cache();
		}
	}

	public override void FailedToShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		HideOverlay(adType);
		_currentlyShowingAdAtLocation = null;
		switch (adType)
		{
		case AdType.Video:
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("SLIDEIN_VIDEO_PLAY_ERROR"));
			onFailedToShowVideo__BackingField.SafeInvoke(adLocationInfo);
			if (adLocationInfo != AdLocations.UNDETERMINED_LOCATION)
			{
				Cache();
			}
			break;
		case AdType.Interstitial:
			onFailedToShowInterstitial__BackingField.SafeInvoke(adLocationInfo);
			break;
		}
	}

	public override void DismissedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		HideOverlay(adType);
		_currentlyShowingAdAtLocation = null;
		switch (adType)
		{
		case AdType.Video:
			EnableAudio();
			ResetOrientationIfNeeded();
			onDismissedVideo__BackingField.SafeInvoke(adLocationInfo);
			break;
		case AdType.Interstitial:
			onDismissedInterstitial__BackingField.SafeInvoke(adLocationInfo);
			break;
		}
	}

	public override void ChangedProvider(AdType adType, string newProvider, string changedWaterfallID)
	{
		onChangedProvider__BackingField.SafeInvoke(changedWaterfallID);
	}

	public override void PriorityListWrapped(string waterfallID)
	{
		onPriorityListWrapped__BackingField.SafeInvoke(waterfallID);
	}

	private void HandleOnWillShowScreen(string screenName)
	{
		bool value = OnlineSettings.instance.GetValue("allow_recache_during_run", defaultValue: false);
		_adController.allowReCacheOnFail = screenName != "IngameUI" || value;
	}

	private void GiveReward()
	{
		if (_videoReward != null)
		{
			_videoReward.SafeInvoke();
			_videoReward = null;
		}
		else
		{
			SDebug.LogError("Tried to GiveReward but _videoReward is null!");
		}
	}

	private void ShowTransitionOverlay(AdType adType, AdLocationInfo adLocationInfo)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			UIScreenController.Instance.ShowTransitionAdsOverlay();
			break;
		case AdType.Video:
			UIScreenController.Instance.ShowVideoAdsOverlay(adLocationInfo);
			break;
		default:
			SDebug.LogError("Unable to find AdsOverlay for adType: " + adType);
			break;
		}
	}

	private void ShowOverlay(AdType adType, AdLocationInfo adLocationInfo)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			UIScreenController.Instance.ShowAdsOverlay();
			break;
		case AdType.Video:
			UIScreenController.Instance.ShowVideoAdsOverlay(adLocationInfo);
			break;
		default:
			SDebug.LogError("Unable to find AdsOverlay for adType: " + adType);
			break;
		}
	}

	private void HideOverlay(AdType adType)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			UIScreenController.Instance.HideAdsOverlay();
			break;
		case AdType.Video:
			UIScreenController.Instance.HideVideoAdsOverlay(forceHide: true);
			break;
		default:
			SDebug.LogError("Unable to find AdsOverlay for adType: " + adType);
			break;
		}
	}

	private void DisableAudio()
	{
		HouseKeeper.SaveAudioOptionsBeforeVideoAds(Settings.optionSfx, Settings.optionMusic);
		Settings.DisableAllSoundOptions();
	}

	private void EnableAudio()
	{
		HouseKeeper.TryRestoreSoundOptionAfterLostFocusIfNeeded();
	}

	private void SaveCurrentOrientation()
	{
		_currentScreenOrientation = ScreenOrientation.Portrait;
		if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
		{
			_currentScreenOrientation = ScreenOrientation.PortraitUpsideDown;
		}
	}

	private void ResetOrientationIfNeeded()
	{
		if (Screen.width > Screen.height)
		{
			Screen.orientation = _currentScreenOrientation;
			Kiloo.Common.Timer.Timer.ExecuteWithDelay(1f, () =>
			{
				Screen.autorotateToLandscapeLeft = false;
				Screen.autorotateToLandscapeRight = false;
				Screen.autorotateToPortrait = true;
				Screen.autorotateToPortraitUpsideDown = true;
				Screen.orientation = ScreenOrientation.AutoRotation;
			});
		}
	}

	private void LoadLastShowedProviders()
	{
		Dictionary<string, string> lastShowedProviders = AdsSave.lastShowedProviders;
		if (lastShowedProviders != null)
		{
			foreach (KeyValuePair<string, string> item in lastShowedProviders)
			{
				_lastShowedProviderPerLocation[item.Key] = item.Value + "_(saved)";
			}
		}
		_lastShowedLocationsPerAdType = AdsSave.lastShowedLocations;
	}

	private void UpdateLastShowedProviders(AdLocationInfo adLocationInfo, string providerID, bool shouldSave)
	{
		_lastShowedProviderPerLocation[adLocationInfo.id] = providerID;
		AdsSave.lastShowedProviders = _lastShowedProviderPerLocation;
		if (shouldSave)
		{
			SaveManager.Save();
		}
	}

	private void UpdateLastShowedLocations(AdType adType, AdLocationInfo adLocationInfo, bool shouldSave)
	{
		_lastShowedLocationsPerAdType[adType] = adLocationInfo.id;
		AdsSave.lastShowedLocations = _lastShowedLocationsPerAdType;
		if (shouldSave)
		{
			SaveManager.Save();
		}
	}

	public static void ChangeToNextProvider()
	{
		if (_instance != null && _instance._isInitialized)
		{
			_instance._adController.ChangeToNextProvider();
			_instance._adController.Cache();
		}
	}

	public static void UpdateDebugPriorityListVersion(AdType adType)
	{
		if (_instance != null && _instance._isInitialized)
		{
			_instance._adController.SetDebugPriorityListVersion(adType, Guid.NewGuid().ToString());
		}
	}

	public static void SetProviderForAdLocation(AdLocationInfo adLocationInfo, AbstractAdProvider provider)
	{
		if (_instance != null && _instance._isInitialized)
		{
			_instance._adController.SetProviderForAdLocation(adLocationInfo, provider);
		}
	}

	public static string GetIdOfAdlocationBeingShown()
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return (_instance._currentlyShowingAdAtLocation == null) ? string.Empty : _instance._currentlyShowingAdAtLocation.id;
	}
}
