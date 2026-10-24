using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdHandling.AdConstants;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Monetization.TapjoyPlugin;
using UnityEngine;

namespace Kiloo.Monetization;

public class TapJoyAdProvider : AbstractAdProvider
{
	public const string ID = "tapjoy";

	private List<string> _cachedIDs = new List<string>();

	private bool _completedVideo;

	private bool _addedListeners;

	public override string id => "tapjoy";

	public override bool supportsMulticaching => false;

	protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
	{
		bool flag = TapJoy.IsSupportedOnPlatform();
		bool flag2 = !string.IsNullOrEmpty(configuration.appID) && !string.IsNullOrEmpty(configuration.appSDKKey);
		_supportedAdTypes[AdType.Video] = flag && flag2;
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (!_addedListeners)
		{
			TapJoy.onInitializeFinished += OnInitializeFinished;
			TapJoy.onFullScreenAdSuccess += OnFullScreenAdSuccess;
			TapJoy.onFullScreenAdFail += OnFullScreenAdFail;
			TapJoy.onVideoError += OnVideoError;
			TapJoy.onViewDidAppear += OnVideoStart;
			TapJoy.onViewDidDisappear += OnViewDidDisappear;
			TapJoy.onVideoComplete += OnVideoComplete;
			_addedListeners = true;
		}
		try
		{
			if (!TapJoy.isInitialized)
			{
				TapJoy.Initialize(configuration.appID, configuration.appSDKKey);
			}
			else
			{
				FinishInitialization(success: true);
			}
		}
		catch (Exception)
		{
			FinishInitialization(success: false);
		}
	}

	private void OnVideoStart(string cacheID)
	{
		if (AdConstants.instance.TapJoyCacheIDs.ContainsValue(cacheID))
		{
			_completedVideo = false;
			AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, _showVideoLocation);
		}
	}

	private void OnViewDidDisappear(string cacheID)
	{
		if (AdConstants.instance.TapJoyCacheIDs.ContainsValue(cacheID))
		{
			if (_completedVideo)
			{
				_completedVideo = false;
				AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _showVideoLocation);
			}
			AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, _showVideoLocation);
		}
	}

	private void OnVideoComplete()
	{
		_completedVideo = true;
	}

	private void OnVideoError(string error)
	{
		AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Video, id, _showVideoLocation);
	}

	private void OnFullScreenAdSuccess(string cacheID)
	{
		if (!(cacheID == "Offerwall"))
		{
			_cachedIDs.Add(cacheID);
			if (_cacheIDToLocation.ContainsKey(cacheID))
			{
				AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, _cacheIDToLocation[cacheID], wasAlreadyCached: false);
				_cacheIDToLocation.Remove(cacheID);
			}
			else
			{
				LogError("Received cache success for cacheId not in dictionary: " + cacheID);
			}
		}
	}

	private void OnFullScreenAdFail(string cacheID)
	{
		if (_cachedIDs.Contains(cacheID))
		{
			_cachedIDs.Remove(cacheID);
		}
		if (_cacheIDToLocation.ContainsKey(cacheID))
		{
			AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, _cacheIDToLocation[cacheID]);
			_cacheIDToLocation.Remove(cacheID);
			return;
		}
		LogError("Received cache success for cacheId not in dictionary: " + cacheID);
		foreach (AdLocationInfo item in _cacheIDToLocation.Values.Where((AdLocationInfo location) => location.adType == AdType.Video))
		{
			AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, item);
		}
		_cacheIDToLocation.RemoveWhere((KeyValuePair<string, AdLocationInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
	}

	private void OnInitializeFinished(string result)
	{
		FinishInitialization(result.ToLower() == "success");
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, adLocationInfo);
		TapJoy.GetFullScreenAd(cacheID);
	}

	protected override void ShowVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		if (_cachedIDs.Contains(cacheID))
		{
			_cachedIDs.Remove(cacheID);
		}
		_showVideoLocation = adLocationInfo;
		TapJoy.ShowFullScreenAd(cacheID);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return _cachedIDs.Contains(cacheID);
	}

	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError("[TapJoy] " + msg, context);
	}

	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_TAPJOY_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
