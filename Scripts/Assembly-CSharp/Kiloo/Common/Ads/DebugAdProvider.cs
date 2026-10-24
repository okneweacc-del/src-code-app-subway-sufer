using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Ads;

public class DebugAdProvider : AbstractAdProvider
{
	private const int DEVICE_CLOSE_BUTTON_SIZE = 150;

	private const int DEVICE_TEXT_FONT_SIZE = 60;

	private const float SIMULATED_FAIL_CHANCE = 0.15f;

	private const float SIMULATED_CACHE_TIME = 1f;

	private readonly string _id;

	private readonly bool _supportsMulticaching;

	private DebugAdGUI _adGUI;

	private List<string> _cachedVideos = new List<string>();

	private List<string> _cachedInterstitials = new List<string>();

	private MonoBehaviour _interstitialRunner;

	private bool _isCachingInterstitial;

	private bool _isCachingVideo;

	private MonoBehaviour _videoRunner;

	public static bool forceNextCacheToFail;

	public static bool forceCacheToFail;

	private bool errorHappened
	{
		get
		{
			if (id.Contains("fail-provider"))
			{
				return true;
			}
			if (id.Contains("success-provider"))
			{
				return false;
			}
			return simulateErrors && Random.value <= 0.15f;
		}
	}

	public override string id => _id;

	private static bool simulateErrors => false;

	public override bool supportsMulticaching => _supportsMulticaching;

	public DebugAdProvider(string id, bool supportsMulticahing)
	{
		_id = id;
		_supportsMulticaching = supportsMulticahing;
	}

	protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
	{
		_supportedAdTypes[AdType.Interstitial] = true;
		_supportedAdTypes[AdType.Video] = true;
	}

	protected override void CacheInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		if (supportsMulticaching || (!_isCachingInterstitial && !_cachedInterstitials.Contains(cacheID)))
		{
			_isCachingInterstitial = true;
			_cacheIDToLocation[cacheID] = adLocationInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, adLocationInfo);
			_interstitialRunner.StartCoroutine(CacheInterstitialCoroutine(adLocationInfo, cacheID));
		}
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		if (supportsMulticaching || (!_isCachingVideo && !_cachedVideos.Contains(cacheID)))
		{
			_isCachingVideo = true;
			_cacheIDToLocation[cacheID] = adLocationInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
			AdSystemManager.SetProviderForAdLocation(adLocationInfo, this);
			_videoRunner.StartCoroutine(CacheVideoCoroutine(adLocationInfo, cacheID));
		}
	}

	protected override bool HasCachedInterstitial(string cacheID)
	{
		return _cachedInterstitials.Contains(cacheID);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return _cachedVideos.Contains(cacheID);
	}

	protected override void OnCacheTimeout(AdType adType, AdLocationInfo adLocationInfo)
	{
		if (adType == AdType.Interstitial)
		{
			_isCachingInterstitial = false;
			_interstitialRunner.StopAllCoroutines();
		}
		else
		{
			_isCachingVideo = false;
			_videoRunner.StopAllCoroutines();
		}
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (errorHappened)
		{
			FinishInitialization(success: false);
			return;
		}
		_adGUI = new GameObject(_id).AddComponent<DebugAdGUI>();
		UpdateAdGuiProperties();
		_adGUI.finishedWatchingVideo += () =>
		{
			AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _adGUI.adLocationInfo);
		};
		_adGUI.dismissedAd += (AdType adType) =>
		{
			AbstractAdProvider.eventInvoker.DismissedAd(adType, id, _adGUI.adLocationInfo);
		};
		Object.DontDestroyOnLoad(_adGUI.gameObject);
		_videoRunner = new GameObject("VideoRunner").AddComponent<NewMonoBehaviour>();
		_interstitialRunner = new GameObject("InterstitialRunner").AddComponent<NewMonoBehaviour>();
		_videoRunner.transform.parent = _adGUI.transform;
		_interstitialRunner.transform.parent = _adGUI.transform;
		FinishInitialization(success: true);
	}

	private void UpdateAdGuiProperties()
	{
		if (!Application.isEditor)
		{
			_adGUI.SetGuiProperties(150, 60);
		}
	}

	protected override void ShowInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		if (errorHappened)
		{
			_cachedInterstitials.Remove(cacheID);
			AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Interstitial, id, adLocationInfo);
		}
		else
		{
			_adGUI.ShowInterstitial(adLocationInfo, id);
			_cachedInterstitials.Remove(cacheID);
			AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, id, adLocationInfo);
		}
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		if (errorHappened)
		{
			_cachedVideos.Remove(cacheID);
			AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Video, id, adLocationInfo);
		}
		else
		{
			_adGUI.ShowVideo(adLocationInfo, id);
			_cachedVideos.Remove(cacheID);
			AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, adLocationInfo);
		}
	}

	private IEnumerator CacheInterstitialCoroutine(AdLocationInfo adLocationInfo, string cacheID)
	{
		yield return Wait.For(1f).UnscaledSeconds().StartCoroutineOn(_interstitialRunner);
		_isCachingInterstitial = false;
		if (forceNextCacheToFail || errorHappened)
		{
			forceNextCacheToFail = false;
			AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, adLocationInfo);
		}
		else
		{
			_cachedInterstitials.Add(cacheID);
			AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, id, adLocationInfo, wasAlreadyCached: false);
		}
	}

	private IEnumerator CacheVideoCoroutine(AdLocationInfo adLocationInfo, string cacheID)
	{
		yield return Wait.For(1f).UnscaledSeconds().StartCoroutineOn(_videoRunner);
		_isCachingVideo = false;
		if (forceNextCacheToFail || errorHappened || forceCacheToFail)
		{
			forceNextCacheToFail = false;
			AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, adLocationInfo);
		}
		else
		{
			_cachedVideos.Add(cacheID);
			AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, adLocationInfo, wasAlreadyCached: false);
		}
	}
}
