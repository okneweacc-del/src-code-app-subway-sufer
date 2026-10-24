using System;

namespace Kiloo.Monetization.SupersonicPlugin.Internal;

public interface ISupersonicPlatformBridge
{
	event Action<bool> onInitFinished;

	event Action<string> onRewardedVideoAdShowFailed;

	event Action onRewardedVideoAdOpened;

	event Action onRewardedVideoAdClosed;

	event Action<bool> onRewardedVideoAvailabilityChanged;

	event Action onRewardedVideoAdStarted;

	event Action onRewardedVideoAdEnded;

	event Action<string> onRewardedVideoAdRewarded;

	event Action onInterstitialAdReady;

	event Action<string> onInterstitialAdLoadFailed;

	event Action onInterstitialAdOpened;

	event Action onInterstitialAdClosed;

	event Action onInterstitialAdShowSucceeded;

	event Action<string> onInterstitialAdShowFailed;

	event Action onInterstitialAdClicked;

	void Init(string appKey, string userId);

	void ValidateIntegration();

	void ShouldTrackNetworkState(bool shouldTrack);

	void SetDebugLogsEnabled(bool enabled);

	bool IsRewardedVideoAvailable();

	void ShowRewardedVideo();

	void ShowRewardedVideo(string location);

	void LoadInterstitial();

	bool IsInterstitialReady();

	bool IsInterstitialPlacementCapped(string placementName);

	void ShowInterstitial();

	void ShowInterstitial(string placementName);
}
