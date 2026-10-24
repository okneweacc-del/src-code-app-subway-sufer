using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;

namespace Kiloo.Monetization.FacebookAds;

public class FacebookAdProvider : AbstractAdProvider
{
	public const string ID = "facebook";

	private bool _addedListeners;

	private bool _videoWatched;

	private readonly Dictionary<AdLocationInfo, string> _adLocationToCacheId = new Dictionary<AdLocationInfo, string>();

	public override string id => "facebook";

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
		bool value = FacebookNativeAds.IsSupportedOnPlatform();
		_supportedAdTypes[AdType.Interstitial] = value;
		_supportedAdTypes[AdType.Video] = value;
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (!_addedListeners)
		{
			FacebookNativeAds.onNativeInitialize += base.FinishInitialization;
			FacebookNativeAds.onNativeDidLoad += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, id, _cacheIDToLocation[cacheID], wasAlreadyCached: false);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					LogError("Received cache success for cacheId not in dictionary: " + cacheID);
				}
			};
			FacebookNativeAds.onNativeAdError += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, _cacheIDToLocation[cacheID]);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					LogError("Received cache failed for cacheId not in dictionary: " + cacheID);
					foreach (AdLocationInfo item in _cacheIDToLocation.Values.Where((AdLocationInfo location) => location.adType == AdType.Interstitial))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(item.adType, id, item);
					}
					_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
				}
			};
			FacebookNativeAds.onNativeAdShown += (string placement) =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			FacebookNativeAds.onNativeAdClosed += (string placement) =>
			{
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			FacebookNativeAds.onNativeAdClicked += (string placement) =>
			{
				AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, "facebook", _showInterstitialLocation);
			};
			FacebookNativeAds.onVideoDidLoad += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, _cacheIDToLocation[cacheID], wasAlreadyCached: false);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					LogError("Received cache success for cacheId not in dictionary: " + cacheID);
				}
			};
			FacebookNativeAds.onVideoError += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, _cacheIDToLocation[cacheID]);
					_cacheIDToLocation.Remove(cacheID);
				}
				else if (_showVideoLocation != null && !_showVideoLocation.id.Equals(AbstractAdProvider.DEFAULT_SHOW_LOCATION.id))
				{
					if (_videoWatched)
					{
						_videoWatched = false;
						AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _showVideoLocation);
					}
					AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Video, id, _showVideoLocation);
					_showVideoLocation = AbstractAdProvider.DEFAULT_SHOW_LOCATION;
				}
				else
				{
					LogError("Received video error but no show location set and cache ID not found as waiting. CacheID: " + cacheID);
					foreach (AdLocationInfo item2 in _cacheIDToLocation.Values.Where((AdLocationInfo location) => location.adType == AdType.Video))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, item2);
					}
					_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Video);
				}
			};
			FacebookNativeAds.onVideoViewStarted += (string cacheID) =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, _showVideoLocation);
			};
			FacebookNativeAds.onVideoViewFinished += (string cacheID) =>
			{
				_videoWatched = true;
			};
			FacebookNativeAds.onVideoViewClosed += (string cacheID) =>
			{
				if (_videoWatched)
				{
					_videoWatched = false;
					AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _showVideoLocation);
				}
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, _showVideoLocation);
				_showVideoLocation = AbstractAdProvider.DEFAULT_SHOW_LOCATION;
			};
			_addedListeners = true;
		}
		try
		{
			FacebookNativeAds.Initialize();
		}
		catch (Exception)
		{
			FinishInitialization(success: false);
		}
	}

	protected override void CacheInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		_adLocationToCacheId[adLocationInfo] = cacheID;
		_cacheIDToLocation[cacheID] = adLocationInfo;
		FacebookNativeAds.CacheNativeAd(cacheID);
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, adLocationInfo);
	}

	protected override void ShowInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		FacebookNativeAds.ShowInterstitial(cacheID);
		_showInterstitialLocation = adLocationInfo;
	}

	protected override bool HasCachedInterstitial(string cacheID)
	{
		return FacebookNativeAds.IsNativeAdLoaded(cacheID);
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_adLocationToCacheId[adLocationInfo] = cacheID;
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
		FacebookNativeAds.LoadVideoAd(cacheID);
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheId)
	{
		_showVideoLocation = adLocationInfo;
		FacebookNativeAds.ShowVideo(cacheId);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return FacebookNativeAds.IsCachedVideoAvailable(cacheID);
	}

	[Conditional("ENABLE_FACEBOOKADS_LOGS")]
	private void Log(string message)
	{
	}

	private void LogWarning(string message)
	{
	}

	private void LogError(string message)
	{
		SDebug.LogError("[FacebookAdProvider] " + message);
	}
}
