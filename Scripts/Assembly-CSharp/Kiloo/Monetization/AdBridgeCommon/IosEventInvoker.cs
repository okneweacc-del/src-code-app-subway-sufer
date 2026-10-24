using System;
using System.Runtime.InteropServices;

namespace Kiloo.Monetization.AdBridgeCommon;

public class IosEventInvoker : IPlatformEventInvoker
{
	private string _providerId;

	public AdBridgeLogger adBridgeLogger { get; set; }

	public IosEventInvoker(string providerId)
	{
		_providerId = providerId;
	}

	[DllImport("__Internal")]
	private static extern void adBridgeListenerInit(string providerId, string[] initializeParams, int initializeParamsLength);

	[DllImport("__Internal")]
	private static extern void adBridgeListenerEnableDebugLogs(string providerId);

	[DllImport("__Internal")]
	private static extern void adBridgeListenerDisableDebugLogs(string providerId);

	[DllImport("__Internal")]
	private static extern void adBridgeListenerLoadInterstitial(string providerId, string placementId);

	[DllImport("__Internal")]
	private static extern void adBridgeListenerShowInterstitial(string providerId, string placementId);

	[DllImport("__Internal")]
	private static extern bool adBridgeListenerIsInterstitialLoaded(string providerId, string placementId);

	[DllImport("__Internal")]
	private static extern void adBridgeListenerLoadVideoAd(string providerId, string placementId);

	[DllImport("__Internal")]
	private static extern void adBridgeListenerShowVideoAd(string providerId, string placementId);

	[DllImport("__Internal")]
	private static extern bool adBridgeListenerIsVideoAdLoaded(string providerId, string placementId);

	public virtual void OnInitialize(object[] initializeParams)
	{
		string[] initializeParams2 = Array.ConvertAll(initializeParams, ConvertObjectToString);
		adBridgeListenerInit(_providerId, initializeParams2, initializeParams.Length);
	}

	private string ConvertObjectToString(object obj)
	{
		if (obj != null)
		{
			return obj.ToString();
		}
		return string.Empty;
	}

	public void EnableDebugLogs()
	{
		adBridgeListenerEnableDebugLogs(_providerId);
	}

	public void DisableDebugLogs()
	{
		adBridgeListenerDisableDebugLogs(_providerId);
	}

	public void LoadVideoAd(string placementId)
	{
		adBridgeListenerLoadVideoAd(_providerId, placementId);
	}

	public void ShowVideoAd(string placementId)
	{
		adBridgeListenerShowVideoAd(_providerId, placementId);
	}

	public bool IsVideoAdLoaded(string placementId)
	{
		return adBridgeListenerIsVideoAdLoaded(_providerId, placementId);
	}

	public bool IsInterstitialLoaded(string placementId)
	{
		return adBridgeListenerIsInterstitialLoaded(_providerId, placementId);
	}

	public void LoadInterstitial(string placementId)
	{
		adBridgeListenerLoadInterstitial(_providerId, placementId);
	}

	public void ShowInterstitial(string placementId)
	{
		adBridgeListenerShowInterstitial(_providerId, placementId);
	}
}
