using System.Collections.Generic;
using Info;
using Kiloo.Common.Ads;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace Kiloo.Monetization.Vungle;

public class VungleAdProvider : AbstractAdProvider
{
	public const string ID = "vungle";

	private const int MIN_SUPPORTED_IOS_VERSION = 8;

	private const int MIN_SUPPORTED_KINDLE_API = 22;

	private bool _addedListeners;

	private VungleAdBridgeWrapper _vungleAdBridgeWrapper;

	public override string id => "vungle";

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
		bool flag = !string.IsNullOrEmpty(configuration.appID);
		_supportedAdTypes[AdType.Video] = flag && IsSupportedOnPlatform();
	}

	private bool IsSupportedOnPlatform()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
		{
			bool result = true;
			string oSVersion = DeviceUtility.GetOSVersion();
			if (int.TryParse(oSVersion.Split('.')[0], out var result2) && result2 < 8)
			{
				result = false;
			}
			return result;
		}
		case RuntimePlatform.Android:
			if (DeviceUtility.isKindleDevice && DeviceUtility.GetAndroidApiLevel() < 22)
			{
				return false;
			}
			return true;
		case RuntimePlatform.WindowsEditor:
			return false;
		case RuntimePlatform.OSXEditor:
			return false;
		default:
			return false;
		}
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		_vungleAdBridgeWrapper = new VungleAdBridgeWrapper(this);
		if (!_addedListeners)
		{
			AddListeners();
		}
		_vungleAdBridgeWrapper.Initialize(configuration.appID, !AgeRestriction.isRestricted);
	}

	private void AddListeners()
	{
		_vungleAdBridgeWrapper.onInitialized += base.FinishInitialization;
		_vungleAdBridgeWrapper.onAdLoaded += OnAdLoaded;
		_vungleAdBridgeWrapper.onAdLoadFailed += OnAdLoadFailed;
		_vungleAdBridgeWrapper.onAdShowed += (string cacheID, AdType adType) =>
		{
			AbstractAdProvider.eventInvoker.ShowedAd(adType, id, _showVideoLocation);
		};
		_vungleAdBridgeWrapper.onAdClicked += (string cacheID, AdType adType) =>
		{
			AbstractAdProvider.eventInvoker.ClickedAd(adType, "vungle", _showVideoLocation);
		};
		_vungleAdBridgeWrapper.onAdWatched += (string cacheID, AdType adType) =>
		{
			AbstractAdProvider.eventInvoker.FinishedWatchingVideo("vungle", _showVideoLocation);
		};
		_vungleAdBridgeWrapper.onAdClosed += (string cacheID, AdType adType) =>
		{
			AbstractAdProvider.eventInvoker.DismissedAd(adType, id, _showVideoLocation);
		};
		_addedListeners = true;
	}

	private void OnAdLoadFailed(string cacheID, AdType adType)
	{
		if (_cacheIDToLocation.ContainsKey(cacheID))
		{
			AbstractAdProvider.eventInvoker.FailedToCacheAd(adType, id, _cacheIDToLocation[cacheID]);
			_cacheIDToLocation.Remove(cacheID);
			return;
		}
		AdSystemLog.LogError("Received cache fail for cacheId not in dictionary: " + cacheID);
		foreach (AdLocationInfo value in _cacheIDToLocation.Values)
		{
			AbstractAdProvider.eventInvoker.FailedToCacheAd(adType, id, value);
		}
		_cacheIDToLocation.Clear();
	}

	private void OnAdLoaded(string cacheID, AdType adType)
	{
		if (_cacheIDToLocation.ContainsKey(cacheID))
		{
			AbstractAdProvider.eventInvoker.CachedAd(adType, id, _cacheIDToLocation[cacheID], wasAlreadyCached: false);
			_cacheIDToLocation.Remove(cacheID);
		}
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		_vungleAdBridgeWrapper.LoadVideoAd(cacheID);
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return _vungleAdBridgeWrapper.IsVideoAdLoaded(cacheID);
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_showVideoLocation = adLocationInfo;
		_vungleAdBridgeWrapper.ShowVideoAd(cacheID);
	}
}
