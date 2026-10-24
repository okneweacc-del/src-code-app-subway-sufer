#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.SupersonicPlugin.Internal;
using UnityEngine;

namespace Kiloo.Monetization.SupersonicPlugin;

public static class Supersonic
{
	private static ISupersonicPlatformBridge _platformBridge;

	private static bool _rewardedVideoIsAvailable;

	private const int MINIMUM_SUPPORTED_ANDROID_API = 17;

	private const int MINIMUM_SUPPORTED_IOS_VERSION = 8;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<bool> onInitFinished__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onRewardedVideoAdShowFailed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onRewardedVideoAdOpened__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onRewardedVideoAdClosed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onRewardedVideoAdRewarded__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<bool> onRewardedVideoAvailabilityChanged__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onRewardedVideoAdStarted__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onRewardedVideoAdEnded__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onInterstitialAdReady__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onInterstitialAdLoadFailed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onInterstitialAdOpened__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onInterstitialAdClosed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onInterstitialAdShowSucceeded__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onInterstitialAdShowFailed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onInterstitialAdClicked__BackingField;

	public static event Action<bool> onInitFinished
	{
		add
		{
			Action<bool> action = onInitFinished__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInitFinished__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<bool> action = onInitFinished__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInitFinished__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onRewardedVideoAdShowFailed
	{
		add
		{
			Action<string> action = onRewardedVideoAdShowFailed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onRewardedVideoAdShowFailed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onRewardedVideoAdOpened
	{
		add
		{
			Action action = onRewardedVideoAdOpened__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onRewardedVideoAdOpened__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onRewardedVideoAdClosed
	{
		add
		{
			Action action = onRewardedVideoAdClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onRewardedVideoAdClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onRewardedVideoAdRewarded
	{
		add
		{
			Action<string> action = onRewardedVideoAdRewarded__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdRewarded__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onRewardedVideoAdRewarded__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdRewarded__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<bool> onRewardedVideoAvailabilityChanged
	{
		add
		{
			Action<bool> action = onRewardedVideoAvailabilityChanged__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAvailabilityChanged__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<bool> action = onRewardedVideoAvailabilityChanged__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAvailabilityChanged__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onRewardedVideoAdStarted
	{
		add
		{
			Action action = onRewardedVideoAdStarted__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onRewardedVideoAdStarted__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onRewardedVideoAdEnded
	{
		add
		{
			Action action = onRewardedVideoAdEnded__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdEnded__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onRewardedVideoAdEnded__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRewardedVideoAdEnded__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onInterstitialAdReady
	{
		add
		{
			Action action = onInterstitialAdReady__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdReady__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onInterstitialAdReady__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdReady__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onInterstitialAdLoadFailed
	{
		add
		{
			Action<string> action = onInterstitialAdLoadFailed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdLoadFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onInterstitialAdLoadFailed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdLoadFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onInterstitialAdOpened
	{
		add
		{
			Action action = onInterstitialAdOpened__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onInterstitialAdOpened__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onInterstitialAdClosed
	{
		add
		{
			Action action = onInterstitialAdClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onInterstitialAdClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onInterstitialAdShowSucceeded
	{
		add
		{
			Action action = onInterstitialAdShowSucceeded__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdShowSucceeded__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onInterstitialAdShowSucceeded__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdShowSucceeded__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onInterstitialAdShowFailed
	{
		add
		{
			Action<string> action = onInterstitialAdShowFailed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onInterstitialAdShowFailed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action onInterstitialAdClicked
	{
		add
		{
			Action action = onInterstitialAdClicked__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onInterstitialAdClicked__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError("Supersonic: " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_SUPERSONIC_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}

	private static bool IsInitialized()
	{
		if (_platformBridge == null)
		{
			return false;
		}
		return true;
	}

	public static bool IsSupportedOnPlatform()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			return !DeviceUtility.isKindleDevice && DeviceUtility.GetAndroidApiLevel() >= 17;
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
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
			return true;
		default:
			return false;
		}
	}

	private static void SetupEvents()
	{
		_platformBridge.onInterstitialAdClicked += () =>
		{
			onInterstitialAdClicked__BackingField.SafeInvoke();
		};
		_platformBridge.onInterstitialAdClosed += () =>
		{
			onInterstitialAdClosed__BackingField.SafeInvoke();
		};
		_platformBridge.onInterstitialAdLoadFailed += (string obj) =>
		{
			onInterstitialAdLoadFailed__BackingField.SafeInvoke(obj);
		};
		_platformBridge.onInterstitialAdOpened += () =>
		{
			onInterstitialAdOpened__BackingField.SafeInvoke();
		};
		_platformBridge.onInterstitialAdReady += () =>
		{
			onInterstitialAdReady__BackingField.SafeInvoke();
		};
		_platformBridge.onInterstitialAdShowFailed += (string obj) =>
		{
			onInterstitialAdShowFailed__BackingField.SafeInvoke(obj);
		};
		_platformBridge.onInterstitialAdShowSucceeded += () =>
		{
			onInterstitialAdShowSucceeded__BackingField.SafeInvoke();
		};
		_platformBridge.onRewardedVideoAdClosed += () =>
		{
			onRewardedVideoAdClosed__BackingField.SafeInvoke();
		};
		_platformBridge.onRewardedVideoAdOpened += () =>
		{
			onRewardedVideoAdOpened__BackingField.SafeInvoke();
		};
		_platformBridge.onRewardedVideoAdRewarded += (string obj) =>
		{
			onRewardedVideoAdRewarded__BackingField.SafeInvoke(obj);
		};
		_platformBridge.onRewardedVideoAdShowFailed += (string obj) =>
		{
			onRewardedVideoAdShowFailed__BackingField.SafeInvoke(obj);
		};
		_platformBridge.onRewardedVideoAvailabilityChanged += (bool obj) =>
		{
			_rewardedVideoIsAvailable = obj;
			onRewardedVideoAvailabilityChanged__BackingField.SafeInvoke(obj);
		};
		_platformBridge.onRewardedVideoAdStarted += () =>
		{
			onRewardedVideoAdStarted__BackingField.SafeInvoke();
		};
		_platformBridge.onRewardedVideoAdEnded += () =>
		{
			onRewardedVideoAdEnded__BackingField.SafeInvoke();
		};
		_platformBridge.onInitFinished += (bool succeeded) =>
		{
			onInitFinished__BackingField.SafeInvoke(succeeded);
		};
	}

	public static void Init(string appKey, string userId)
	{
		if (!IsInitialized())
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = SupersonicIOS.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = SupersonicAndroid.CreatePlugin();
				break;
			default:
				LogError("There's no platform bridge for the current platform.");
				return;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				break;
			}
			if (_platformBridge != null)
			{
				SetupEvents();
				_platformBridge.Init(appKey, userId);
				_platformBridge.SetDebugLogsEnabled(enabled: false);
				_platformBridge.ShouldTrackNetworkState(shouldTrack: true);
			}
		}
	}

	public static void ShowInterstitial()
	{
		if (IsInitialized())
		{
			_platformBridge.ShowInterstitial();
		}
	}

	public static void ShowInterstitial(string placementName)
	{
		if (IsInitialized())
		{
			_platformBridge.ShowInterstitial(placementName);
		}
	}

	public static void LoadInterstitial()
	{
		if (IsInitialized())
		{
			_platformBridge.LoadInterstitial();
		}
	}

	public static bool IsInterstitialReady()
	{
		if (!IsInitialized())
		{
			return false;
		}
		return _platformBridge.IsInterstitialReady();
	}

	private static bool IsInterstitialPlacementCapped(string placementName)
	{
		if (!IsInitialized())
		{
			return false;
		}
		return _platformBridge.IsInterstitialPlacementCapped(placementName);
	}

	public static void ShowRewardedVideo()
	{
		if (IsInitialized())
		{
			_platformBridge.ShowRewardedVideo();
		}
	}

	public static void ShowRewardedVideo(string placementName)
	{
		if (IsInitialized())
		{
			_platformBridge.ShowRewardedVideo(placementName);
		}
	}

	public static bool IsRewardedVideoAvailable()
	{
		if (!IsInitialized())
		{
			return false;
		}
		if (_rewardedVideoIsAvailable)
		{
			return _platformBridge.IsRewardedVideoAvailable();
		}
		return false;
	}

	public static void ValidateIntegration()
	{
		if (IsInitialized())
		{
			_platformBridge.ValidateIntegration();
		}
	}

	public static void ShouldTrackNetworkState(bool shouldTrack)
	{
		if (IsInitialized())
		{
			_platformBridge.ShouldTrackNetworkState(shouldTrack);
		}
	}
}
