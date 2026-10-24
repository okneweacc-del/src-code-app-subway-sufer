using System;
using System.Collections;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Ads;

public class AdLocation : AdEventListenerAdapter
{
	private const float CHECK_TIMEOUT_INTERVAL = 0.5f;

	private const float INITIALIZE_WAIT_TIME_SECONDS = 5f;

	private AbstractAdProvider _adProvider;

	private IAdSettingsProvider _settingsProvider;

	private DateTime _timeOfLastShow;

	private IEnumerator _cacheCoroutine;

	private bool _shouldRestartIsCachingCheck;

	private IEnumerator _unavailableCoroutine;

	public readonly AdLocationInfo info;

	public readonly string waterfallID;

	private bool hasProvider => _adProvider != null;

	public bool isCaching => hasProvider && _adProvider.IsCachingLocation(info);

	public AdLocation(AdLocationInfo info, string waterfallID)
	{
		this.info = info;
		this.waterfallID = waterfallID;
	}

	public void SetSettingsProvider(IAdSettingsProvider settingsProvider)
	{
		_settingsProvider = settingsProvider;
	}

	public void SetAdProvider(AbstractAdProvider adProvider)
	{
		if (adProvider == null || adProvider.IsSupportedOnPlatform(info.adType))
		{
			_adProvider = adProvider;
		}
		else
		{
			_adProvider = null;
		}
	}

	public string GetCurrentAdProviderId()
	{
		return (_adProvider != null) ? _adProvider.id : "null";
	}

	private string GetCacheId()
	{
		return _settingsProvider.GetCacheID(_adProvider.id, info);
	}

	public bool IsCached()
	{
		return hasProvider && _adProvider.HasCachedAd(info.adType, GetCacheId());
	}

	public bool Show()
	{
		if (!hasProvider)
		{
			AdSystemLog.LogError("Provider was null for location: " + info.id);
			return false;
		}
		TimeSpan minimumShowIntervalForLocation = _settingsProvider.GetMinimumShowIntervalForLocation(info.id, info.adType);
		bool flag = info.adType == AdType.Interstitial && Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT;
		if (DateTime.Now > _timeOfLastShow + minimumShowIntervalForLocation || _timeOfLastShow > DateTime.Now || flag)
		{
			return _adProvider.ShowAd(info.adType, info, GetCacheId());
		}
		return false;
	}

	public void Cache()
	{
		if (!hasProvider)
		{
			AdSystemLog.LogWarning("Unable to cache adLocationId " + info.id + ", provider was null");
			return;
		}
		if (!_adProvider.supportsMulticaching && _adProvider.HasCachedAd(info.adType, GetCacheId()))
		{
			_adProvider.CacheAd(info.adType, info, GetCacheId());
			return;
		}
		CoroutineUtil.StopCoroutine(_unavailableCoroutine);
		_unavailableCoroutine = WaitWhileUnavailableThenCache();
		CoroutineUtil.StartCoroutine(_unavailableCoroutine);
	}

	private IEnumerator WaitWhileConditionOrUntilTimedOut(Func<bool> condition, float timeoutInSeconds)
	{
		float startTime = Time.realtimeSinceStartup;
		float elapsedTime = 0f;
		while (condition() && elapsedTime < timeoutInSeconds)
		{
			elapsedTime = Time.realtimeSinceStartup - startTime;
			yield return null;
		}
	}

	private IEnumerator WaitWhileUnavailableThenCache()
	{
		float startTime = Time.realtimeSinceStartup;
		Func<bool> isInitializingFunc = () => hasProvider && _adProvider.isInitializing;
		Func<bool> isCachingFunc = () => hasProvider && _adProvider.isInitialized && _adProvider.isCaching && !_adProvider.supportsMulticaching;
		int waitTimeout = (hasProvider ? _settingsProvider.GetProviderWaitWhileCachingTimeLimit(info.id, _adProvider.id) : 0);
		yield return WaitWhileConditionOrUntilTimedOut(isInitializingFunc, 5f);
		do
		{
			_shouldRestartIsCachingCheck = false;
			if (isCachingFunc())
			{
				yield return WaitWhileConditionOrUntilTimedOut(isCachingFunc, waitTimeout);
			}
		}
		while (_shouldRestartIsCachingCheck);
		bool isInitializing = isInitializingFunc();
		bool isStillCaching = isCachingFunc();
		if (hasProvider)
		{
			if (_adProvider.isInitialized)
			{
				_adProvider.CacheAd(info.adType, info, GetCacheId());
				yield break;
			}
			_adProvider.FailCaching(info.adType, info);
			AdSystemLog.LogWarning("WaitWhileUnavailableThenCache - Initialisation of provider " + _adProvider.id + " failed!");
		}
		else
		{
			AdSystemLog.LogWarning("WaitWhileUnavailableThenCache - Does not have ad provider");
		}
	}

	private IEnumerator CacheCoroutine()
	{
		float startTime = Time.realtimeSinceStartup;
		float elapsedTime = 0f;
		double cacheTimeoutLimit = _settingsProvider.GetCacheTimeoutLimit(_adProvider.id, info.adType).TotalSeconds;
		if (!isCaching)
		{
			AdSystemLog.LogWarning("isCaching was false on entering AdLocation.CacheCoroutine. Check order of events when Ad Provider triggers StartedCachingAd.");
			yield return null;
		}
		while (isCaching)
		{
			if ((double)elapsedTime > cacheTimeoutLimit)
			{
				AdSystemLog.LogWarning("Timeout for location " + info.id + " from " + _adProvider.id + ". Elapsed time: " + elapsedTime);
				_adProvider.TimeoutCaching(info.adType, info);
				break;
			}
			elapsedTime = Time.realtimeSinceStartup - startTime;
			yield return Wait.For(0.5f).UnscaledSeconds().StartCoroutine();
		}
		if (isCaching)
		{
			AdSystemLog.LogError("CacheCoroutine for location: " + info.id + " finished but the provider " + _adProvider.id + " hasn't cached it's ad!");
		}
	}

	public override void StartedCachingAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		if (hasProvider && _adProvider.id == providerID && adLocationInfo != info)
		{
			_shouldRestartIsCachingCheck = true;
		}
		if (info == adLocationInfo)
		{
			CoroutineUtil.StopCoroutine(_cacheCoroutine);
			_cacheCoroutine = CacheCoroutine();
			CoroutineUtil.StartCoroutine(_cacheCoroutine);
		}
	}

	public override void ChangedProvider(AdType adType, string newProvider, string changedWaterfallID)
	{
		if (hasProvider && info.adType == adType && waterfallID == changedWaterfallID)
		{
			SetAdProvider(AbstractAdProvider.GetAdProvider(newProvider));
		}
	}

	public override void DismissedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		MakeSureAdIsCached(providerID, adLocationInfo);
	}

	public override void FinishedWatchingVideo(string providerID, AdLocationInfo adLocationInfo)
	{
		MakeSureAdIsCached(providerID, adLocationInfo);
	}

	public override void FailedToShowAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		MakeSureAdIsCached(providerID, adLocationInfo);
	}

	public override void FailedToCacheAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		if (hasProvider && providerID == _adProvider.id && adType != info.adType)
		{
			_shouldRestartIsCachingCheck = true;
		}
	}

	private void MakeSureAdIsCached(string providerID, AdLocationInfo adLocationInfo)
	{
		if (hasProvider && providerID == _adProvider.id)
		{
			if (adLocationInfo != info)
			{
				_shouldRestartIsCachingCheck = true;
			}
			if (adLocationInfo == info && !_adProvider.HasCachedAd(info.adType, GetCacheId()))
			{
				Cache();
			}
		}
	}

	public override void ShowedAd(AdType adType, string providerID, AdLocationInfo adLocationInfo)
	{
		if (info == adLocationInfo)
		{
			_timeOfLastShow = DateTime.Now;
		}
	}
}
