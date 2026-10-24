using System;
using Kiloo.Common.Ads;
using Kiloo.Monetization.InmobiPlugin;

namespace Kiloo.Monetization;

public class InMobiAdProvider : AbstractAdProvider
{
	public const string ID = "inmobi";

	private bool _completedVideo;

	private bool _addedListeners;

	public override string id => "inmobi";

	public override bool supportsMulticaching => false;

	protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
	{
		bool flag = Inmobi.IsSupportedOnPlatform();
		bool flag2 = !string.IsNullOrEmpty(configuration.appID);
		_supportedAdTypes[AdType.Interstitial] = flag && flag2;
		_supportedAdTypes[AdType.Video] = flag && flag2;
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (!_addedListeners)
		{
			Inmobi.willShowAd += (string cacheId) =>
			{
				_completedVideo = false;
				AbstractAdProvider.eventInvoker.ShowedAd(_showLocation.adType, id, _showLocation);
			};
			Inmobi.onCacheAd += (string cacheId) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheId))
				{
					AbstractAdProvider.eventInvoker.CachedAd(_cacheIDToLocation[cacheId].adType, id, _cacheIDToLocation[cacheId], wasAlreadyCached: false);
					_cacheIDToLocation.Remove(cacheId);
				}
				else
				{
					LogError("Received Cached Ad callback for unrecognised cache ID: " + cacheId);
				}
			};
			Inmobi.onFailToLoadAd += (string cacheId) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheId))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(_cacheIDToLocation[cacheId].adType, id, _cacheIDToLocation[cacheId]);
				}
				else
				{
					LogError("Received Fail To Load Ad callback for unrecognised cache ID: " + cacheId);
					foreach (AdLocationInfo value in _cacheIDToLocation.Values)
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(value.adType, id, value);
					}
					_cacheIDToLocation.Clear();
				}
			};
			Inmobi.onCompleteIncentive += (string cacheId) =>
			{
				_completedVideo = true;
			};
			Inmobi.onDismissAd += (string cacheId) =>
			{
				if (_completedVideo)
				{
					_completedVideo = false;
					AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _showLocation);
				}
				AbstractAdProvider.eventInvoker.DismissedAd(_showLocation.adType, id, _showLocation);
			};
			Inmobi.onInitSuccessfully += (string key) =>
			{
				FinishInitialization(success: true);
			};
			Inmobi.onFailToDisplayAd += (string key) =>
			{
				AbstractAdProvider.eventInvoker.FailedToShowAd(_showLocation.adType, id, _showLocation);
			};
			_addedListeners = true;
		}
		try
		{
			Inmobi.Initialize(configuration.appID);
		}
		catch (Exception)
		{
			FinishInitialization(success: false);
		}
	}

	protected override bool HasCachedInterstitial(string cacheID)
	{
		return Inmobi.HasCachedAd(cacheID);
	}

	protected override void CacheInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, adLocationInfo);
		Inmobi.CacheAd(cacheID);
	}

	protected override void ShowInterstitial(AdLocationInfo adLocationInfo, string cacheID)
	{
		_showLocation = adLocationInfo;
		Inmobi.ShowAd(cacheID);
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
		Inmobi.CacheAd(cacheID);
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_showLocation = adLocationInfo;
		Inmobi.ShowAd(cacheID);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return Inmobi.HasCachedAd(cacheID);
	}

	private void LogError(string message)
	{
		AdSystemLog.LogError("[InMobi] " + message);
	}
}
