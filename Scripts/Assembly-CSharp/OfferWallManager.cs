using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AdHandling.AdConstants;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.TapjoyPlugin;
using UI.SlideIns;
using UnityEngine;

public class OfferWallManager
{
	private enum RequestStates
	{
		NotStarted = 0,
		InProgress = 1,
		Succeeded = 2,
		Failed = 3
	}

	private const string OFFERWALL_INITIALIZE_FAIL_CODE = "fail";

	private const string OFFERWALL_DISABLED_ONLINE_SETTING = "offerwall_disabled";

	private const string OFFERWALL_EVENT_NAME = "Offerwall";

	private const bool DEFAULT_DISABLED_ONLINE_SETTING = false;

	private const int DEFAULT_REWARDS_REFRESH_INTERVAL_AFTER_FAIL = 30;

	private const int DEFAULT_REWARDS_REFRESH_INTERVAL_AFTER_SUCCESS = 120;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action onOfferWallReceived__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<int> onPendingRewardsReceived__BackingField;

	private static OfferWallManager _instance;

	private static bool _isOfferwallActive;

	private RequestStates _requestRewardState;

	private RequestStates _spendRewardState;

	private DateTime _lastTimeRewardsWereRequested = DateTime.MinValue;

	private int _amountPendingApprovalToBeRewarded;

	private int _amountApprovedToBeRewarded;

	private string _lastOriginForShow = string.Empty;

	private bool _listenersSet;

	private int MINIMUM_SUPPORTED_ANDROID_API = 19;

	public static bool isOfferwallActive
	{
		get
		{
			if (_isOfferwallActive)
			{
				_isOfferwallActive = false;
				return true;
			}
			return false;
		}
		private set
		{
			_isOfferwallActive = value;
		}
	}

	public static OfferWallManager instance => _instance ?? (_instance = new OfferWallManager());

	public bool isOfferWallDisabled => OnlineSettings.instance.GetValue("offerwall_disabled", defaultValue: false);

	public event Action onOfferWallReceived
	{
		add
		{
			Action action = onOfferWallReceived__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onOfferWallReceived__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onOfferWallReceived__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onOfferWallReceived__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action<int> onPendingRewardsReceived
	{
		add
		{
			Action<int> action = onPendingRewardsReceived__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPendingRewardsReceived__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = onPendingRewardsReceived__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPendingRewardsReceived__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private string GetSDKAppID()
	{
		return AdConstants.instance.TapJoyAppId;
	}

	private string GetSDKAppKey()
	{
		return AdConstants.instance.TapJoyAppKey;
	}

	private string GetSDKEventName()
	{
		return "Offerwall";
	}

	private bool HasTimeIntervalPassedSinceDate(DateTime lastRequestDateTime, int timeInterval)
	{
		double totalSeconds = CDateTime.Now.Subtract(lastRequestDateTime).TotalSeconds;
		return (double)timeInterval < totalSeconds;
	}

	public void Initialize()
	{
		if (IsOfferWallAllowed() && !TapJoy.isInitialized)
		{
			SetListeners();
			TapJoy.Initialize(GetSDKAppID(), GetSDKAppKey());
		}
	}

	private void SetListeners()
	{
		if (!_listenersSet)
		{
			TapJoy.onInitializeFinished += OnInitializeFinished;
			TapJoy.onOfferWallResponse += OnOfferWallResponse;
			TapJoy.onGetPoints += OnGotPoints;
			TapJoy.onGetPointsError += OnGetPointsError;
			TapJoy.onEarnPoints += OnEarnPoints;
			TapJoy.onSpendPoints += OnSpendPoints;
			TapJoy.onSpendPointsError += OnSpendPointsError;
			TapJoy.onViewDidAppear += OnViewDidAppear;
			TapJoy.onViewDidDisappear += OnViewDidDisappear;
			_listenersSet = true;
		}
	}

	public bool IsOfferWallAllowed()
	{
		return !AgeRestriction.isRestricted && Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice && DeviceUtility.GetAndroidApiLevel() >= MINIMUM_SUPPORTED_ANDROID_API;
	}

	public void RequestOfferWall()
	{
		if (IsOfferWallReady())
		{
			OnOfferWallResponse("Offerwall already ready");
			return;
		}
		if (!TapJoy.isInitialized)
		{
			Initialize();
		}
		TapJoy.GetOfferWall(GetSDKEventName());
	}

	public void ShowOfferWall(string origin)
	{
		if (IsOfferWallReady())
		{
			_lastOriginForShow = origin;
			TapJoy.ShowOfferWall();
			isOfferwallActive = true;
		}
	}

	public bool IsOfferWallReady()
	{
		return TapJoy.IsOfferWallReady();
	}

	public void GetPendingRewards()
	{
		if (!TapJoy.isInitialized)
		{
			return;
		}
		int timeInterval = ((_requestRewardState != RequestStates.Succeeded || _spendRewardState != RequestStates.Succeeded) ? 30 : 120);
		if (HasTimeIntervalPassedSinceDate(_lastTimeRewardsWereRequested, timeInterval))
		{
			if (_requestRewardState != RequestStates.InProgress && _spendRewardState != RequestStates.InProgress)
			{
				_lastTimeRewardsWereRequested = DateTime.Now;
				_requestRewardState = RequestStates.InProgress;
				TapJoy.GetPoints();
			}
		}
		else if (_amountApprovedToBeRewarded >= 0)
		{
			PayoutRewardToUser(_amountApprovedToBeRewarded);
			_amountApprovedToBeRewarded = 0;
		}
	}

	private void SpendPoints(int amount)
	{
		_spendRewardState = RequestStates.InProgress;
		TapJoy.SpendPoints(amount);
	}

	private void PayoutRewardToUser(int rewardAmount)
	{
		if (rewardAmount > 0)
		{
			onPendingRewardsReceived__BackingField.SafeInvoke(rewardAmount);
			InventorySave.IncreaseCoins(rewardAmount, "Offerwall");
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.OfferWall,
					stringValue = rewardAmount.ToString()
				});
			}
			SaveManager.Save();
		}
	}

	private void OnOfferWallResponse(string message)
	{
		Flurry.LogEventWithAParameter("Attempt loading OfferWall", "Message", message);
		onOfferWallReceived__BackingField.SafeInvoke();
	}

	private void OnInitializeFinished(string s)
	{
		if (s != "fail")
		{
			RequestOfferWall();
		}
	}

	private void OnViewDidAppear(string viewType)
	{
		if (viewType == GetSDKEventName())
		{
			Flurry.LogEventWithAParameter("OfferWall Shown", "Origin", _lastOriginForShow);
		}
	}

	private void OnViewDidDisappear(string viewType)
	{
		if (viewType == GetSDKEventName())
		{
			Flurry.LogEventWithAParameter("OfferWall Closed", "Origin", _lastOriginForShow);
			GetPendingRewards();
			if (!IsOfferWallReady())
			{
				RequestOfferWall();
			}
			isOfferwallActive = false;
		}
	}

	private void OnGotPoints(string amount)
	{
		Flurry.LogEventWithAParameter("OfferWall Get Points", "Amount", amount);
		int result = 0;
		if (int.TryParse(amount, out result) && result > 0)
		{
			_amountPendingApprovalToBeRewarded = result;
		}
		_requestRewardState = RequestStates.Succeeded;
		SpendPoints(_amountPendingApprovalToBeRewarded);
	}

	private void OnGetPointsError()
	{
		Flurry.LogEventWithAParameter("OfferWall Get Points", "Result", "Failed");
		_requestRewardState = RequestStates.Failed;
	}

	private void OnSpendPoints(string amountOfPointsLeftInUsersBalance)
	{
		Flurry.LogEventWithAParameter("OfferWall Spend Points", "Amount", amountOfPointsLeftInUsersBalance);
		_amountApprovedToBeRewarded += _amountPendingApprovalToBeRewarded;
		_amountPendingApprovalToBeRewarded = 0;
		if (_amountApprovedToBeRewarded >= 0)
		{
			PayoutRewardToUser(_amountApprovedToBeRewarded);
			_amountApprovedToBeRewarded = 0;
		}
		_spendRewardState = RequestStates.Succeeded;
	}

	private void OnSpendPointsError()
	{
		Flurry.LogEventWithAParameter("OfferWall Spend Points", "Result", "Failed");
		_spendRewardState = RequestStates.Failed;
	}

	private void OnEarnPoints(string amount)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Origin", _lastOriginForShow);
		dictionary.Add("Amount", amount);
		Flurry.LogEventWithSeveralParameters("OfferWall Earned Points", dictionary);
	}

	[Conditional("ENABLE_OFFERWALL_LOGS")]
	private void Log(string message)
	{
	}
}
