namespace Kiloo.Monetization.AdBridgeCommon;

public interface IAdBridge
{
	event AdCallback onAdLoaded;

	event AdCallback onAdLoadFailed;

	event AdCallback onAdClicked;

	event AdCallback onAdShowed;

	event AdCallback onAdShowFailed;

	event AdCallback onAdClosed;

	event AdCallback onAdWatched;

	void Initialize(params object[] initializeParams);

	void LoadVideoAd(string placementId);

	void ShowVideoAd(string placementId);

	bool IsVideoAdLoaded(string placementId);

	void LoadInterstitial(string placementId);

	void ShowInterstitial(string placementId);

	bool IsInterstitialLoaded(string placementId);
}
