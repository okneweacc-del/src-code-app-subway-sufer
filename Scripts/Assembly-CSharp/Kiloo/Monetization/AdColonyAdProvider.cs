using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common.Ads;
using Kiloo.Monetization.AdColonyPlugin;
using UnityEngine;

namespace Kiloo.Monetization;

public class AdColonyAdProvider : AbstractAdProvider
{
	public const string ID = "adcolony";

	private bool _addedListeners;

	private bool _receivedReward;

	private readonly HashSet<string> _availableVideos = new HashSet<string>();

	public override string id => "adcolony";

	public override bool supportsMulticaching => false;

	protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
	{
		bool flag = AdColony.IsSupportedOnPlatform();
		bool flag2 = !string.IsNullOrEmpty(configuration.appID) && configuration.zoneIDs != null && configuration.zoneIDs.Length > 0;
		_supportedAdTypes[AdType.Video] = flag && flag2;
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		if (IsVideoExpired(cacheID))
		{
			_availableVideos.Remove(cacheID);
		}
		return _availableVideos.Contains(cacheID);
	}

	private bool IsVideoExpired(string cacheID)
	{
		return AdColony.IsVideoExpired(cacheID);
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (!_addedListeners)
		{
			AdColony.onInitFinished += () =>
			{
				FinishInitialization(success: true);
			};
			AdColony.onAdReward += (bool success) =>
			{
				_receivedReward = success;
			};
			AdColony.onAdClosed += () =>
			{
				if (_receivedReward)
				{
					_receivedReward = false;
					AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, _showVideoLocation);
				}
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, _showVideoLocation);
			};
			AdColony.onCachingFailed += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(_cacheIDToLocation[cacheID].adType, id, _cacheIDToLocation[cacheID]);
					_cacheIDToLocation.Remove(cacheID);
				}
				else
				{
					foreach (AdLocationInfo value in _cacheIDToLocation.Values)
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(value.adType, id, value);
					}
					_cacheIDToLocation.Clear();
				}
			};
			AdColony.onCachingSuccess += (string cacheID) =>
			{
				if (_cacheIDToLocation.ContainsKey(cacheID))
				{
					_availableVideos.Add(cacheID);
					AbstractAdProvider.eventInvoker.CachedAd(_cacheIDToLocation[cacheID].adType, id, _cacheIDToLocation[cacheID], wasAlreadyCached: false);
					_cacheIDToLocation.Remove(cacheID);
				}
			};
			_addedListeners = true;
		}
		try
		{
			AdColony.Configure(configuration.appID, configuration.zoneIDs);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			FinishInitialization(success: false);
		}
	}

	protected override void CacheVideo(AdLocationInfo adLocationInfo, string cacheID)
	{
		_cacheIDToLocation[cacheID] = adLocationInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(adLocationInfo.adType, id, adLocationInfo);
		AdColony.LoadVideoAd(cacheID);
	}

	protected override void ShowVideo(AdLocationInfo adLocationID, string cacheID)
	{
		_showVideoLocation = adLocationID;
		_availableVideos.Remove(cacheID);
		AdStarted();
		AdColony.PlayVideoAd(cacheID);
	}

	private void AdStarted()
	{
		_receivedReward = false;
		AbstractAdProvider.eventInvoker.ShowedAd(_showVideoLocation.adType, id, _showVideoLocation);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError("AdColony: " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_ADCOLONY_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
