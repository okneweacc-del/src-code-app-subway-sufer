using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Ads;

namespace Kiloo.Monetization.AdBridgeCommon;

public abstract class AdBridgeWrapper<TAdBridge> : IAdBridge where TAdBridge : IAdBridge
{
	protected AbstractAdProvider _adProvider;

	protected TAdBridge _adBridge;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdLoaded__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdLoadFailed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdClicked__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdShowed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdShowFailed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdClosed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AdCallback onAdWatched__BackingField;

	public event AdCallback onAdLoaded
	{
		add
		{
			AdCallback adCallback = this.onAdLoaded;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdLoaded, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = this.onAdLoaded;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdLoaded, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdLoadFailed
	{
		add
		{
			AdCallback adCallback = this.onAdLoadFailed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdLoadFailed, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = this.onAdLoadFailed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdLoadFailed, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdClicked
	{
		add
		{
			AdCallback adCallback = this.onAdClicked;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdClicked, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = this.onAdClicked;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdClicked, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdShowed
	{
		add
		{
			AdCallback adCallback = this.onAdShowed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdShowed, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = this.onAdShowed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdShowed, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdShowFailed
	{
		add
		{
			AdCallback adCallback = this.onAdShowFailed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdShowFailed, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = this.onAdShowFailed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdShowFailed, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdClosed
	{
		add
		{
			AdCallback adCallback = this.onAdClosed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdClosed, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = this.onAdClosed;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdClosed, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	public event AdCallback onAdWatched
	{
		add
		{
			AdCallback adCallback = this.onAdWatched;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdWatched, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
		remove
		{
			AdCallback adCallback = this.onAdWatched;
			AdCallback adCallback2;
			do
			{
				adCallback2 = adCallback;
				adCallback = Interlocked.CompareExchange(ref this.onAdWatched, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
			}
			while ((object)adCallback != adCallback2);
		}
	}

	protected AdBridgeWrapper(AbstractAdProvider adProvider)
	{
		_adProvider = adProvider;
	}

	protected bool IsInitialized()
	{
		return _adBridge != null && _adProvider.isInitialized;
	}

	public void Initialize(params object[] initializeParams)
	{
		if (!IsInitialized())
		{
			_adBridge = CreateAdBridge(_adProvider);
		}
		if (_adBridge != null)
		{
			_adBridge.Initialize(initializeParams);
			SubscribeToEvents();
		}
	}

	protected abstract TAdBridge CreateAdBridge(AbstractAdProvider adProvider);

	protected virtual void SubscribeToEvents()
	{
		ref TAdBridge adBridge = ref _adBridge;
		AdCallback value = this.onAdLoaded;
		adBridge.onAdLoaded += value;
		ref TAdBridge adBridge2 = ref _adBridge;
		AdCallback value2 = this.onAdLoadFailed;
		adBridge2.onAdLoadFailed += value2;
		ref TAdBridge adBridge3 = ref _adBridge;
		AdCallback value3 = this.onAdClicked;
		adBridge3.onAdClicked += value3;
		ref TAdBridge adBridge4 = ref _adBridge;
		AdCallback value4 = this.onAdShowed;
		adBridge4.onAdShowed += value4;
		ref TAdBridge adBridge5 = ref _adBridge;
		AdCallback value5 = this.onAdShowFailed;
		adBridge5.onAdShowFailed += value5;
		ref TAdBridge adBridge6 = ref _adBridge;
		AdCallback value6 = this.onAdClosed;
		adBridge6.onAdClosed += value6;
		ref TAdBridge adBridge7 = ref _adBridge;
		AdCallback value7 = this.onAdWatched;
		adBridge7.onAdWatched += value7;
	}

	public void LoadVideoAd(string placementId)
	{
		if (IsInitialized())
		{
			_adBridge.LoadVideoAd(placementId);
		}
	}

	public void ShowVideoAd(string placementId)
	{
		if (IsInitialized())
		{
			_adBridge.ShowVideoAd(placementId);
		}
	}

	public bool IsVideoAdLoaded(string placementId)
	{
		if (IsInitialized())
		{
			return _adBridge.IsVideoAdLoaded(placementId);
		}
		return false;
	}

	public void LoadInterstitial(string placementId)
	{
		if (IsInitialized())
		{
			_adBridge.LoadInterstitial(placementId);
		}
	}

	public void ShowInterstitial(string placementId)
	{
		if (IsInitialized())
		{
			_adBridge.ShowInterstitial(placementId);
		}
	}

	public bool IsInterstitialLoaded(string placementId)
	{
		if (IsInitialized())
		{
			return _adBridge.IsInterstitialLoaded(placementId);
		}
		return false;
	}
}
