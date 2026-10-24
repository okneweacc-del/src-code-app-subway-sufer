using UnityEngine;

namespace Kiloo.Monetization.AdBridgeCommon;

public class AndroidEventInvoker : IPlatformEventInvoker
{
	private const string INITIALIZE_METHOD_NAME = "init";

	private const string ENABLE_DEBUG_LOGS_METHOD_NAME = "enableDebugLogs";

	private const string DISABLE_DEBUG_LOGS_METHOD_NAME = "disableDebugLogs";

	private const string LOAD_VIDEO_AD_METHOD_NAME = "loadVideoAd";

	private const string SHOW_VIDEO_AD_METHOD_NAME = "showVideoAd";

	private const string IS_VIDEO_AD_LOADED_METHOD_NAME = "isVideoAdLoaded";

	private const string LOAD_INTERSTITIAL_METHOD_NAME = "loadInterstitial";

	private const string SHOW_INTERSTITIAL_METHOD_NAME = "showInterstitial";

	private const string IS_INTERSTITIAL_LOADED_METHOD_NAME = "isInterstitialLoaded";

	private AndroidJavaObject _platformBridge;

	public AdBridgeLogger adBridgeLogger { get; set; }

	public AndroidEventInvoker(string javaBridgePath)
	{
		_platformBridge = new AndroidJavaObject(javaBridgePath);
	}

	public void OnInitialize(object[] initializeParams)
	{
		Log("OnInitialize");
		_platformBridge.Call("init", initializeParams);
	}

	public void EnableDebugLogs()
	{
		Log("EnableDebugLogs");
		_platformBridge.Call("enableDebugLogs");
	}

	public void DisableDebugLogs()
	{
		Log("DisableDebugLogs");
		_platformBridge.Call("disableDebugLogs");
	}

	public void LoadVideoAd(string placementId)
	{
		Log("LoadVideoAd" + placementId);
		_platformBridge.Call("loadVideoAd", placementId);
	}

	public void ShowVideoAd(string placementId)
	{
		Log("ShowVideoAd" + placementId);
		_platformBridge.Call("showVideoAd", placementId);
	}

	public bool IsVideoAdLoaded(string placementId)
	{
		bool flag = _platformBridge.Call<bool>("isVideoAdLoaded", new object[1] { placementId });
		Log($"IsVideoAdLoaded placementID: {placementId}, isVideoAdLoaded: {flag}");
		return flag;
	}

	public bool IsInterstitialLoaded(string placementId)
	{
		bool flag = _platformBridge.Call<bool>("isInterstitialLoaded", new object[1] { placementId });
		Log($"IsVideoAdLoaded placementID: {placementId}, isInterstitialLoaded: {flag}");
		return flag;
	}

	public void LoadInterstitial(string placementId)
	{
		_platformBridge.Call("loadInterstitial", placementId);
	}

	public void ShowInterstitial(string placementId)
	{
		_platformBridge.Call("showInterstitial", placementId, Screen.width - 2 * (Screen.width / 10));
	}

	private void Log(string message)
	{
		adBridgeLogger.Log(message);
	}
}
