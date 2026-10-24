using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.SupersonicPlugin;
using UnityEngine;

public class SupersonicAdProvider : AbstractAdProvider
{
	public const string ID = "supersonic";

	private bool _addedListeners;

	private bool _videoWatched;

	private bool _isVideoCaching;

	public override string id => "supersonic";

	public override bool supportsMulticaching => false;

	public override List<AdType> adTypesWithClickCallbackSupport
	{
		get
		{
			List<AdType> list = new List<AdType>();
			list.Add(AdType.Interstitial);
			return list;
		}
	}

	protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
	{
		bool flag = Supersonic.IsSupportedOnPlatform();
		bool flag2 = !string.IsNullOrEmpty(configuration.appID);
		_supportedAdTypes[AdType.Interstitial] = flag && flag2;
		_supportedAdTypes[AdType.Video] = flag && flag2;
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (!_addedListeners)
		{
			Supersonic.onInitFinished += base.FinishInitialization;
			Supersonic.onRewardedVideoAdOpened += () =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, _showVideoLocation);
			};
			Supersonic.onRewardedVideoAdRewarded += (string key) =>
			{
				_videoWatched = true;
			};
			Supersonic.onRewardedVideoAdClosed += () =>
			{
				if (_videoWatched)
				{
					_videoWatched = false;
					AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _showVideoLocation);
				}
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, _showVideoLocation);
			};
			Supersonic.onRewardedVideoAvailabilityChanged += (bool available) =>
			{
				if (!_cacheIDToLocation.Values.All((AdLocationInfo location) => location.adType != AdType.Video))
				{
					if (_isVideoCaching && !available)
					{
						foreach (AdLocationInfo item in _cacheIDToLocation.Values.Where((AdLocationInfo location) => location.adType == AdType.Video))
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, item);
						}
					}
					else if (available)
					{
						foreach (AdLocationInfo item2 in _cacheIDToLocation.Values.Where((AdLocationInfo location) => location.adType == AdType.Video))
						{
							AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, item2, wasAlreadyCached: false);
						}
					}
					_isVideoCaching = false;
					_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Video);
				}
			};
			Supersonic.onRewardedVideoAdShowFailed += (string key) =>
			{
				AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Video, id, _showVideoLocation);
			};
			Supersonic.onInterstitialAdClicked += () =>
			{
				AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			Supersonic.onInterstitialAdReady += () =>
			{
				foreach (AdLocationInfo item3 in _cacheIDToLocation.Values.Where((AdLocationInfo locationInfo) => locationInfo.adType == AdType.Interstitial))
				{
					AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, id, item3, wasAlreadyCached: false);
				}
				_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
			};
			Supersonic.onInterstitialAdLoadFailed += (string error) =>
			{
				foreach (AdLocationInfo item4 in _cacheIDToLocation.Values.Where((AdLocationInfo locationInfo) => locationInfo.adType == AdType.Interstitial))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, item4);
				}
				_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
			};
			Supersonic.onInterstitialAdOpened += () =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			Supersonic.onInterstitialAdShowFailed += (string key) =>
			{
				AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			Supersonic.onInterstitialAdClosed += () =>
			{
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			_addedListeners = true;
		}
		try
		{
			string userId = DeviceUtility.GetDeviceIdentifier();
			Action<string> performInit = (string uid) =>
			{
				Supersonic.Init(configuration.appID, uid);
			};
			if (Application.platform == RuntimePlatform.Android)
			{
				Application.AdvertisingIdentifierCallback delegateMethod = (string advertisingId, bool trackingEnabled, string error) =>
				{
					if (trackingEnabled && !string.IsNullOrEmpty(advertisingId))
					{
						userId = advertisingId;
					}
					performInit(userId);
				};
				if (!Application.RequestAdvertisingIdentifierAsync(delegateMethod))
				{
					performInit(userId);
				}
			}
			else
			{
				performInit(userId);
			}
		}
		catch (Exception)
		{
			FinishInitialization(success: false);
		}
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_isVideoCaching = true;
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return Supersonic.IsRewardedVideoAvailable();
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_videoWatched = false;
		_showVideoLocation = adLocationInfo;
		Supersonic.ShowRewardedVideo(cacheID);
	}

	protected override bool HasCachedInterstitial(string cacheID)
	{
		return Supersonic.IsInterstitialReady();
	}

	protected override void CacheInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, adLocationInfo);
		Supersonic.LoadInterstitial();
	}

	protected override void ShowInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		_showInterstitialLocation = adLocationInfo;
		Supersonic.ShowInterstitial(cacheID);
	}

	protected override void OnCacheTimeout(AdType adType, AdLocationInfo adLocationInfo)
	{
		base.OnCacheTimeout(adType, adLocationInfo);
		if (adType == AdType.Video)
		{
			_isVideoCaching = false;
		}
	}

	[Conditional("ENABLE_SUPERSONIC_LOGS")]
	private void Log(string message)
	{
	}
}
