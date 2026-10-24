namespace Kiloo.Monetization.AdBridgeCommon;

public interface IPlatformEventInvoker
{
	AdBridgeLogger adBridgeLogger { get; set; }

	void OnInitialize(object[] initializeParams);

	void EnableDebugLogs();

	void DisableDebugLogs();

	void LoadVideoAd(string placementId);

	void ShowVideoAd(string placementId);

	bool IsVideoAdLoaded(string placementId);

	bool IsInterstitialLoaded(string placementId);

	void LoadInterstitial(string placementId);

	void ShowInterstitial(string placementId);
}
