using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Ads;
using UnityEngine;
using Utilities;

namespace Kiloo.Monetization.AdBridgeCommon;

public abstract class AdBridgeBase : MonoBehaviour, IAdBridge
{
	protected IPlatformEventInvoker _platformEventInvoker;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdLoaded__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdLoadFailed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdShowed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdShowFailed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdClosed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdWatched__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdClicked__BackingField;

	protected string _providerId;

	protected abstract RuntimePlatform platform { get; }

	protected abstract bool isLoggingEnabled { get; }

	public AdBridgeLogger adBridgeLogger { get; private set; }

	public event AdCallback onAdLoaded
	{
		add
		{
			AdCallback adCallback = onAdLoaded__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdLoaded__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = onAdLoaded__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdLoaded__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdLoadFailed
	{
		add
		{
			AdCallback adCallback = onAdLoadFailed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdLoadFailed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = onAdLoadFailed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdLoadFailed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdShowed
	{
		add
		{
			AdCallback adCallback = onAdShowed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdShowed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = onAdShowed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdShowed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdShowFailed
	{
		add
		{
			AdCallback adCallback = onAdShowFailed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdShowFailed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = onAdShowFailed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdShowFailed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdClosed
	{
		add
		{
			AdCallback adCallback = onAdClosed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdClosed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = onAdClosed__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdClosed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdWatched
	{
		add
		{
			AdCallback adCallback = onAdWatched__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdWatched__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = onAdWatched__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdWatched__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdClicked
	{
		add
		{
			AdCallback adCallback = onAdClicked__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdClicked__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = onAdClicked__BackingField;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref onAdClicked__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public void Initialize(params object[] initializeParams)
	{
		_platformEventInvoker = CreateAdBridgeEventInvoker();
		adBridgeLogger = new AdBridgeLogger(platform.ToFriendlyString(), _providerId, isLoggingEnabled, _platformEventInvoker);
		_platformEventInvoker.adBridgeLogger = adBridgeLogger;
		Log("Initialize");
		adBridgeLogger.UpdateDebugLogsState();
		InitializeAdBridge(initializeParams);
	}

	protected virtual void InitializeAdBridge(object[] initializeParams)
	{
		_platformEventInvoker.OnInitialize(initializeParams);
	}

	protected abstract IPlatformEventInvoker CreateAdBridgeEventInvoker();

	protected static T CreatePlugin<T>(string callbackReceiverName, AbstractAdProvider adProvider) where T : AdBridgeBase
	{
		GameObject gameObject = new GameObject(callbackReceiverName);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		T result = gameObject.AddComponent<T>();
		result.SetPropertiesFromAdProvider(adProvider);
		return result;
	}

	public void SetPropertiesFromAdProvider(AbstractAdProvider adProvider)
	{
		_providerId = adProvider.id;
	}

	public void LoadVideoAd(string placementId)
	{
		_platformEventInvoker.LoadVideoAd(placementId);
	}

	public void ShowVideoAd(string placementId)
	{
		_platformEventInvoker.ShowVideoAd(placementId);
	}

	public bool IsVideoAdLoaded(string placementId)
	{
		return _platformEventInvoker.IsVideoAdLoaded(placementId);
	}

	public void LoadInterstitial(string placementId)
	{
		_platformEventInvoker.LoadInterstitial(placementId);
	}

	public void ShowInterstitial(string placementId)
	{
		_platformEventInvoker.ShowInterstitial(placementId);
	}

	public bool IsInterstitialLoaded(string placementId)
	{
		return _platformEventInvoker.IsInterstitialLoaded(placementId);
	}

	public void CallbackOnVideoAdLoaded(string placementId)
	{
		Log("CallbackOnVideoAdLoaded " + placementId);
		if (onAdLoaded__BackingField != null)
		{
			onAdLoaded__BackingField(placementId, AdType.Video);
		}
	}

	public void CallbackOnVideoAdLoadFailed(string placementId)
	{
		Log("CallbackOnVideoAdLoadFailed " + placementId);
		if (onAdLoadFailed__BackingField != null)
		{
			onAdLoadFailed__BackingField(placementId, AdType.Video);
		}
	}

	public void CallbackOnVideoAdClicked(string placementId)
	{
		Log("CallbackOnVideoAdClicked " + placementId);
		if (onAdClicked__BackingField != null)
		{
			onAdClicked__BackingField(placementId, AdType.Video);
		}
	}

	public void CallbackOnVideoAdShowed(string placementId)
	{
		Log("CallbackOnVideoAdShowed " + placementId);
		if (onAdShowed__BackingField != null)
		{
			onAdShowed__BackingField(placementId, AdType.Video);
		}
	}

	public void CallbackOnVideoAdShowFailed(string placementId)
	{
		Log("CallbackOnVideoAdShowFailed " + placementId);
		if (onAdShowFailed__BackingField != null)
		{
			onAdShowFailed__BackingField(placementId, AdType.Video);
		}
	}

	public void CallbackOnVideoAdClosed(string placementId)
	{
		Log("CallbackOnVideoAdClosed " + placementId);
		if (onAdClosed__BackingField != null)
		{
			onAdClosed__BackingField(placementId, AdType.Video);
		}
	}

	public void CallbackOnVideoAdWatched(string placementId)
	{
		Log("CallbackOnVideoAdWatched " + placementId);
		if (onAdWatched__BackingField != null)
		{
			onAdWatched__BackingField(placementId, AdType.Video);
		}
	}

	public void CallbackOnInterstitialAdLoaded(string placementId)
	{
		Log("CallbackOnInterstitialAdLoaded " + placementId);
		if (onAdLoaded__BackingField != null)
		{
			onAdLoaded__BackingField(placementId, AdType.Interstitial);
		}
	}

	public void CallbackOnInterstitialAdLoadFailed(string placementId)
	{
		Log("CallbackOnInterstitialAdLoadFailed " + placementId);
		if (onAdLoadFailed__BackingField != null)
		{
			onAdLoadFailed__BackingField(placementId, AdType.Interstitial);
		}
	}

	public void CallbackOnInterstitialAdClicked(string placementId)
	{
		Log("CallbackOnInterstitialAdClicked " + placementId);
		if (onAdClicked__BackingField != null)
		{
			onAdClicked__BackingField(placementId, AdType.Interstitial);
		}
	}

	public void CallbackOnInterstitialAdShowed(string placementId)
	{
		Log("CallbackOnInterstitialAdShowed " + placementId);
		if (onAdShowed__BackingField != null)
		{
			onAdShowed__BackingField(placementId, AdType.Interstitial);
		}
	}

	public void CallbackOnInterstitialAdShowFailed(string placementId)
	{
		Log("CallbackOnInterstitialAdShowFailed " + placementId);
		if (onAdShowFailed__BackingField != null)
		{
			onAdShowFailed__BackingField(placementId, AdType.Interstitial);
		}
	}

	public void CallbackOnInterstitialAdClosed(string placementId)
	{
		Log("CallbackOnInterstitialAdClosed " + placementId);
		if (onAdClosed__BackingField != null)
		{
			onAdClosed__BackingField(placementId, AdType.Interstitial);
		}
	}

	public void CallbackOnInterstitialAdWatched(string placementId)
	{
		Log("CallbackOnInterstitialAdWatched " + placementId);
		if (onAdWatched__BackingField != null)
		{
			onAdWatched__BackingField(placementId, AdType.Interstitial);
		}
	}

	protected void Log(string message)
	{
		adBridgeLogger.Log(message);
	}
}
