using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Managers;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using UI;
using UI.FreeStuff;
using UI.Screens.RewardManagement;
using UnityEngine;

public class MysteryBoxDoubleUpButton : MonoBehaviour
{
	[SerializeField]
	private RewardForVideoButton _doubleButton;

	[SerializeField]
	private GameObject _stickerGameObject;

	[SerializeField]
	private Animation _stickerAnimation;

	private const string ONLINESETTINGS_MYSTERY_BOX_DOUBLE_REWARD_CHANCE_KEY = "mystery_box_double_reward_chance";

	private const int DEFAULT_MYSTERY_BOX_DOUBLE_REWARD_CHANCE = 100;

	private const string ONLINESETTINGS_MYSTERY_BOX_DOUBLE_REWARD_ALLOW_KEYS_KEY = "mystery_box_double_reward_allow_keys";

	private const bool DEFAULT_MYSTERY_BOX_DOUBLE_REWARD_ALLOW_KEYS = false;

	private const string ONLINESETTINGS_ALWAYS_ON_AD_BUTTONS_DOUBLE_UP = "ad_buttons_double_up_always_on";

	private const bool ONLINESETTINGS_ALWAYS_ON_AD_BUTTONS_DEFAULT_VALUE = true;

	private CelebrationReward _rewardToDouble;

	private bool _wasRewardDoubled;

	private bool _isButtonEnabled;

	private Dictionary<CelebrationRewardType, Action> _doubleRewardCallbacks = new Dictionary<CelebrationRewardType, Action>(0);

	private Coroutine _animateButtonCoroutine;

	private Coroutine _animateAlphaCoroutine;

	private VideoButtonPayload _videoButtonPayload;

	private bool _hasSetDefaultButtonColorOnce;

	private int chance => OnlineSettings.instance.GetValue("mystery_box_double_reward_chance", 100);

	private VideoButtonPayload videoButtonPayload
	{
		get
		{
			if (_videoButtonPayload == null)
			{
				_videoButtonPayload = new VideoButtonPayload
				{
					title = "CELEBRATION_POPUP_DOUBLE_UP",
					isAvailable = () => true,
					onVideoWatched = OnVideoWatched,
					adLocation = AdLocations.DOUBLE_UP_VIDEOS,
					firstColor = ColorData.instance.grass,
					firstColorDark = ColorData.instance.avocado,
					firstHighlightColor = ColorData.instance.lime,
					secondColor = ColorData.instance.grass,
					secondColorDark = ColorData.instance.avocado,
					secondHighlightColor = ColorData.instance.lime,
					titleColor = ColorData.instance.white,
					descriptionColor = ColorData.instance.white,
					alwaysOn = true,
					autoPlayOnFill = true
				};
			}
			return _videoButtonPayload;
		}
	}

	private bool ChanceRoll()
	{
		return chance > UnityEngine.Random.Range(0, 100);
	}

	private bool IsVideoAvailable()
	{
		bool flag = AdSystemManager.IsCached(AdLocations.DOUBLE_UP_VIDEOS);
		AdTrackingSystemLogger.LogFillRequest(AdLocations.DOUBLE_UP_VIDEOS, flag);
		return flag;
	}

	public bool IsAdButtonAlwaysOnEnabled()
	{
		return OnlineSettings.instance.GetValue("ad_buttons_double_up_always_on", defaultValue: true);
	}

	private bool IsDoublable(CelebrationReward reward)
	{
		bool result = false;
		switch (reward.rewardType)
		{
		case CelebrationRewardType.keys:
			result = OnlineSettings.instance.GetValue("mystery_box_double_reward_allow_keys", defaultValue: false);
			break;
		case CelebrationRewardType.token:
			result = CharacterManager.IsTokenUseful(reward.characterId, reward, isDoubling: true);
			break;
		case CelebrationRewardType.coins:
			result = true;
			break;
		case CelebrationRewardType.powerup:
			switch (reward.upgradeType)
			{
			case UpgradeType.hoverboard:
				result = true;
				break;
			case UpgradeType.headstart2000:
				result = true;
				break;
			case UpgradeType.scorebooster:
				result = true;
				break;
			}
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	private bool IsValidOrigin(CelebrationReward reward)
	{
		switch (reward.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
		case CelebrationRewardOrigin.MysteryBoxMini:
		case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			return true;
		default:
			return false;
		}
	}

	public bool TryEnable(CelebrationReward rewardToDouble, Dictionary<CelebrationRewardType, Action> doubleRewardCallbacks)
	{
		_wasRewardDoubled = false;
		bool flag = IsValidOrigin(rewardToDouble);
		bool flag2 = IsDoublable(rewardToDouble);
		bool flag3 = IsVideoAvailable();
		bool flag4 = ChanceRoll();
		bool flag5 = IsAdButtonAlwaysOnEnabled() || IsVideoAvailable();
		bool flag6 = (_isButtonEnabled = flag && flag2 && flag4 && flag5);
		_doubleButton.gameObject.SetActive(flag6);
		_doubleButton.Init(videoButtonPayload);
		if (flag6)
		{
			_rewardToDouble = rewardToDouble;
			_doubleRewardCallbacks = doubleRewardCallbacks;
			if (_animateButtonCoroutine != null)
			{
				CoroutineUtil.StopCoroutine(_animateButtonCoroutine);
			}
			_animateButtonCoroutine = CoroutineUtil.StartCoroutine(AnimateButton());
			SetDefaultButtonColorOnce();
		}
		if (flag && flag2)
		{
			string arg = ((!flag4) ? "No Chance" : "Chance");
			string arg2 = ((!flag3) ? "No Fill" : "Fill");
			string argValue = $"{arg} - {arg2}";
			Flurry.LogEventWithAParameter("Attempted to show double up button", "Result", argValue);
			if (!flag3)
			{
				string currentProviderIDForLocation = AdSystemManager.GetCurrentProviderIDForLocation(AdLocations.DOUBLE_UP_VIDEOS);
				string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
				Flurry.LogEventWithSeveralParameters("No fill for double button", "State;Country", currentProviderIDForLocation + ";" + contryCodeFromLocale);
			}
		}
		return flag6;
	}

	private IEnumerator AnimateButton()
	{
		float alphaDuration = 0.2f;
		float bounceDuration = 1f;
		ResetAnimationState();
		if (_animateAlphaCoroutine != null)
		{
			CoroutineUtil.StopCoroutine(_animateAlphaCoroutine);
		}
		_animateAlphaCoroutine = CoroutineUtil.StartCoroutine(AnimateAlpha(_doubleButton.GetComponent<UIWidget>(), alphaDuration, 0f, 1f));
		yield return new WaitForSeconds(alphaDuration);
		_stickerGameObject.SetActive(value: true);
		float p = 0f;
		Time.timeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		while (p < bounceDuration)
		{
			_stickerGameObject.transform.localScale = Vector3.one * Easings.ElasticEaseOut(p / bounceDuration);
			p += Time.deltaTime;
			yield return null;
		}
		_stickerAnimation.Play();
	}

	private void ResetAnimationState()
	{
		_stickerGameObject.SetActive(value: false);
		_doubleButton.GetComponent<UIWidget>().alpha = 1f;
		_stickerGameObject.transform.localScale = Vector3.one;
		_stickerAnimation.Stop();
		_stickerAnimation.clip.SampleAnimation(_stickerGameObject, 0f);
	}

	public void DisableButton()
	{
		if (_isButtonEnabled && _rewardToDouble != null && !_wasRewardDoubled)
		{
			FlurryLogDoubleUpResultAs(doubledUp: false);
		}
		_wasRewardDoubled = false;
		_isButtonEnabled = false;
		_doubleButton.gameObject.SetActive(value: false);
	}

	[UsedImplicitly]
	public void OnVideoWatched()
	{
		_wasRewardDoubled = true;
		Flurry.LogEvent("Double up button clicked");
		Flurry.LogEvent("Double up button shown");
		DisableButton();
		OnDoubleReward();
	}

	private void OnDoubleReward()
	{
		if (_rewardToDouble != null && IsDoublable(_rewardToDouble))
		{
			if (_doubleRewardCallbacks.TryGetValue(_rewardToDouble.rewardType, out var value))
			{
				value?.Invoke();
			}
			FlurryLogDoubleUpResultAs(doubledUp: true);
		}
		_rewardToDouble = null;
	}

	private void OnEnable()
	{
		DisableButton();
		_rewardToDouble = null;
	}

	private IEnumerator AnimateAlpha(UIWidget widget, float duration, float from, float to)
	{
		widget.alpha = from;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			widget.alpha = Mathf.Lerp(from, to, factor);
			yield return null;
		}
	}

	public void FlurryLogDoubleUpResultAs(bool doubledUp)
	{
		string arg = ((_rewardToDouble.rewardType != CelebrationRewardType.powerup) ? _rewardToDouble.rewardType.ToString() : _rewardToDouble.upgradeType.ToString());
		string arg2 = ((_rewardToDouble.rewardType != CelebrationRewardType.coins) ? _rewardToDouble.amount.ToString() : GroupCoinsForFlurryLogging(_rewardToDouble.amount));
		string arg3 = ((!doubledUp) ? "Not Doubled" : "Doubled");
		string argValue = $"{arg2} {arg} {arg3}";
		Flurry.LogEventWithAParameter("Double up choice", "Result", argValue);
	}

	public string GroupCoinsForFlurryLogging(int originalCoinAmount)
	{
		if (originalCoinAmount < 500)
		{
			int num = originalCoinAmount / 10 * 10;
			int num2 = num + 9;
			return num + "-" + num2;
		}
		return originalCoinAmount.ToString();
	}

	private void SetDefaultButtonColorOnce()
	{
		if (!_hasSetDefaultButtonColorOnce)
		{
			UIButtonColor component = _doubleButton.GetComponent<UIButtonColor>();
			if (component != null)
			{
				Color defaultColor = component.defaultColor;
				component.defaultColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1f);
			}
			_hasSetDefaultButtonColorOnce = true;
		}
	}

	[Conditional("MYSTERY_BOX_DOUBLE_REWARD_LOGS")]
	private void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
