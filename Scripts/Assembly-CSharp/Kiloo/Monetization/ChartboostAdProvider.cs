using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.ChartboostPlugin;

namespace Kiloo.Monetization;

public class ChartboostAdProvider : AbstractAdProvider
{
	public const string ID = "chartboost";

	private bool _addedListeners;

	private bool _completedRewardedVideo;

	public override string id => "chartboost";

	public override bool supportsMulticaching => false;

	public override List<AdType> adTypesWithClickCallbackSupport
	{
		get
		{
			List<AdType> list = new List<AdType>();
			list.Add(AdType.Video);
			list.Add(AdType.Interstitial);
			return list;
		}
	}

	protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
	{
		bool flag = Chartboost.IsSupportedOnPlatform();
		bool flag2 = !string.IsNullOrEmpty(configuration.appID) && !string.IsNullOrEmpty(configuration.appSignature);
		bool flag3 = !DeviceUtility.isKindleDevice;
		_supportedAdTypes[AdType.Interstitial] = flag && flag2;
		_supportedAdTypes[AdType.Video] = flag && flag2 && flag3;
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (!_addedListeners)
		{
			Chartboost.onInitFinished += base.FinishInitialization;
			Chartboost.onClickInterstitial += (string key) =>
			{
				AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			Chartboost.onFailToLoadInterstitial += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, _cacheIDToLocation[cacheID]);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					AdSystemLog.LogError("Received cache failed for cacheID not in dictionary: " + cacheID);
					foreach (AdLocationInfo item in _cacheIDToLocation.Values.Where((AdLocationInfo location) => location.adType == AdType.Interstitial))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, item);
					}
					_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
				}
			};
			Chartboost.onCacheInterstitial += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, id, _cacheIDToLocation[cacheID], wasAlreadyCached: false);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					AdSystemLog.LogError("Received cached success for cacheID not in dictionary: " + cacheID);
				}
			};
			Chartboost.onDismissInterstitial += (string key) =>
			{
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			Chartboost.onDisplayInterstitial += (string key) =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, id, _showInterstitialLocation);
			};
			Chartboost.onCacheRewardedVideo += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, _cacheIDToLocation[cacheID], wasAlreadyCached: false);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					AdSystemLog.LogError("Received cached success for cacheID not in dictionary: " + cacheID);
				}
			};
			Chartboost.onFailToLoadRewardedVideo += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, _cacheIDToLocation[cacheID]);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					AdSystemLog.LogError("Received cache failed for cacheID not in dictionary: " + cacheID);
					foreach (AdLocationInfo item2 in _cacheIDToLocation.Values.Where((AdLocationInfo location) => location.adType == AdType.Video))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, item2);
					}
					_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Video);
				}
			};
			Chartboost.onClickRewardedVideo += (string key) =>
			{
				AbstractAdProvider.eventInvoker.ClickedAd(AdType.Video, id, _showVideoLocation);
			};
			Chartboost.onDisplayRewardedVideo += (string key) =>
			{
				_completedRewardedVideo = false;
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, _showVideoLocation);
			};
			Chartboost.onCompleteRewardedVideo += (string key) =>
			{
				_completedRewardedVideo = true;
			};
			Chartboost.onDismissRewardedVideo += (string key) =>
			{
				if (_completedRewardedVideo)
				{
					_completedRewardedVideo = false;
					AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _showVideoLocation);
				}
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, _showVideoLocation);
			};
			_addedListeners = true;
		}
		try
		{
			Chartboost.StartSession(configuration.appID, configuration.appSignature);
		}
		catch (Exception)
		{
			FinishInitialization(success: false);
		}
	}

	protected override bool HasCachedInterstitial(string cacheID)
	{
		return Chartboost.HasCachedInterstitial(cacheID);
	}

	protected override void CacheInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, adLocationInfo);
		Chartboost.CacheInterstitial(cacheID);
	}

	protected override void ShowInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		_showInterstitialLocation = adLocationInfo;
		Chartboost.ShowInterstitial(cacheID);
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
		Chartboost.CacheRewardedVideo(cacheID);
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_showVideoLocation = adLocationInfo;
		Chartboost.ShowRewardedVideo(cacheID);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return Chartboost.HasCachedRewardedVideo(cacheID);
	}
}
