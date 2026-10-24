using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common.Ads;
using UnityEngine;

namespace Kiloo.Monetization.MobVista;

public class MobVistaAdProvider : AbstractAdProvider
{
	public const string ID = "mobvista";

	public override string id => "mobvista";

	public override bool supportsMulticaching => false;

	public override List<AdType> adTypesWithClickCallbackSupport
	{
		get
		{
			List<AdType> list = new List<AdType>();
			list.Add(AdType.Video);
			return list;
		}
	}

	protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
	{
		bool isSupportedOnPlatform = MobVista.isSupportedOnPlatform;
		bool flag = !string.IsNullOrEmpty(configuration.appID) && !string.IsNullOrEmpty(configuration.appSDKKey);
		_supportedAdTypes[AdType.Video] = isSupportedOnPlatform && flag;
		_supportedAdTypes[AdType.Interstitial] = MobVista.isInterstitialSupportedOnPlatform && flag;
	}

	protected override void OnInitialize(AdProviderConfiguration adProviderConfiguration)
	{
		MobVista.Initialize(adProviderConfiguration.appID, adProviderConfiguration.appSDKKey);
		SetupHooks();
		FinishInitialization(success: true);
	}

	private void SetupHooks()
	{
		MobVista.onVideoLoaded += OnVideoLoaded;
		MobVista.onVideoAdClose += OnVideoAdClose;
		MobVista.onVideoLoadFailed += OnVideoLoadFailed;
		MobVista.onVideoShow += OnVideoShow;
		MobVista.onVideoAdWatched += OnVideoAdWatched;
		MobVista.onVideoAdShowFailed += OnVideoAdShowFailed;
		MobVista.onVideoAdClicked += OnVideoAdClicked;
		MobVista.onInterstitialLoaded += OnInterstitialLoaded;
		MobVista.onInterstitialAdClose += OnInterstitialAdClose;
		MobVista.onInterstitialLoadFailed += OnInterstitialLoadFailed;
		MobVista.onInterstitialShow += OnInterstitialShow;
		MobVista.onInterstitialAdWatched += OnInterstitialAdWatched;
		MobVista.onInterstitialAdShowFailed += OnInterstitialAdShowFailed;
		MobVista.onInterstitialAdClicked += OnInterstitialAdClicked;
	}

	private bool IsFullScreenAdLoaded(string cacheId)
	{
		return MobVista.IsFullScreenAdLoaded(cacheId);
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
		MobVista.LoadFullScreenAd(cacheID);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return IsFullScreenAdLoaded(cacheID);
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_showLocation = adLocationInfo;
		MobVista.ShowFullScreenAd(cacheID);
	}

	protected override void CacheInterstitial(AdLocationInfo adLocation, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocation;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, adLocation);
		MobVista.LoadInterstitialAd(cacheID);
	}

	protected override bool HasCachedInterstitial(string cacheID)
	{
		return MobVista.IsInterstitialAdLoaded(cacheID);
	}

	protected override void ShowInterstitial(AdLocationInfo adLocation, string cacheID)
	{
		_showInterstitialLocation = adLocation;
		_showLocation = adLocation;
		MobVista.ShowInterstitialAd(cacheID);
		AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, "mobvista", _showInterstitialLocation);
	}

	private void OnVideoLoaded(string cacheID)
	{
		if (_cacheIDToLocation.ContainsKey(cacheID))
		{
			AbstractAdProvider.eventInvoker.CachedAd(_cacheIDToLocation[cacheID].adType, "mobvista", _cacheIDToLocation[cacheID], wasAlreadyCached: false);
			_cacheIDToLocation.Remove(cacheID);
		}
	}

	private void OnVideoShow(string cacheID)
	{
		AbstractAdProvider.eventInvoker.ShowedAd(_showLocation.adType, "mobvista", _showLocation);
	}

	private void OnVideoAdClose(string cacheID)
	{
		AbstractAdProvider.eventInvoker.DismissedAd(_showLocation.adType, "mobvista", _showLocation);
	}

	private void OnVideoAdWatched(string cacheID)
	{
		AbstractAdProvider.eventInvoker.FinishedWatchingVideo("mobvista", _showLocation);
	}

	private void OnVideoLoadFailed(string cacheID)
	{
		if (_cacheIDToLocation.ContainsKey(cacheID))
		{
			AbstractAdProvider.eventInvoker.FailedToCacheAd(_cacheIDToLocation[cacheID].adType, "mobvista", _cacheIDToLocation[cacheID]);
			_cacheIDToLocation.Remove(cacheID);
		}
	}

	private void OnVideoAdClicked(string cacheID)
	{
		AbstractAdProvider.eventInvoker.ClickedAd(_showLocation.adType, "mobvista", _showLocation);
	}

	private void OnVideoAdShowFailed(string error)
	{
		AbstractAdProvider.eventInvoker.FailedToShowAd(_showLocation.adType, "mobvista", _showLocation);
	}

	private void OnInterstitialLoaded(string cacheID)
	{
		if (_cacheIDToLocation.ContainsKey(cacheID))
		{
			AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, "mobvista", _cacheIDToLocation[cacheID], wasAlreadyCached: false);
			_cacheIDToLocation.Remove(cacheID);
		}
	}

	private void OnInterstitialShow(string cacheID)
	{
		AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, "mobvista", _showInterstitialLocation);
	}

	private void OnInterstitialAdClose(string cacheID)
	{
		AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, "mobvista", _showInterstitialLocation);
	}

	private void OnInterstitialAdWatched(string cacheID)
	{
		AbstractAdProvider.eventInvoker.FinishedWatchingVideo("mobvista", _showInterstitialLocation);
	}

	private void OnInterstitialLoadFailed(string cacheID)
	{
		AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, "mobvista", _cacheIDToLocation[cacheID]);
		_cacheIDToLocation.Remove(cacheID);
	}

	private void OnInterstitialAdClicked(string unitId)
	{
		AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, "mobvista", _showInterstitialLocation);
	}

	private void OnInterstitialAdShowFailed(string error)
	{
		AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Interstitial, "mobvista", _showInterstitialLocation);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		SDebug.LogError("[MobvistaVideoAdProvider] " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
	}

	[Conditional("ENABLE_MOBVISTA_LOGS")]
	private static void Log(string msg, Object context = null)
	{
	}
}
