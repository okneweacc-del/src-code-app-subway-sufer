using System;
using System.Collections;
using System.Collections.Generic;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Utility;
using UI.Generic;
using UI.VideoAdPrizes;
using UnityEngine;

public class CoinEarnerVideoAds : MonoBehaviour
{
	private enum ButtonState
	{
		Loading = 0,
		NoVideo = 1,
		VideoReady = 2
	}

	[SerializeField]
	private GameObject _videoAvailableGameObject;

	[SerializeField]
	private GameObject _loadingGameObject;

	[SerializeField]
	private UILabel _loadingLabel;

	[SerializeField]
	private GameObject _noVideosGameObject;

	[SerializeField]
	private UILabel _noVideosLabel;

	[SerializeField]
	private UILabel _noVideosTryAgainLabel;

	[SerializeField]
	private GameObject _prizeContent;

	[SerializeField]
	private UISprite _fillOverlay;

	[SerializeField]
	private UISprite _backgroundFill;

	[SerializeField]
	private UISprite _backgroundHighlightFill;

	[SerializeField]
	private UISprite _overlayHighlightFill;

	[SerializeField]
	private Collider _collider;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _priceLabelKeys;

	[SerializeField]
	private int _prizeIconDepth;

	[SerializeField]
	private UISprite _spritePrefab;

	[SerializeField]
	private GameObject _tvAnimatedIcon;

	[SerializeField]
	private UIWidget _tvContainer;

	[SerializeField]
	private List<EarnCoinButtonPrizeIconSetup> _prizeIconSetups;

	private int _earnCurrencyProfileIndex;

	private Action _onClickDelegate;

	private ButtonState _currentButtonState;

	private List<UISprite> _prizeIcons;

	private const string BG_SPRITENAME_ACTIVE_DEFAULT = "backgroud_callout_square";

	private const string BG_SPRITENAME_OVERLAY_DEFAULT = "whitebox_div";

	private const string BG_SPRITENAME_INACTIVE = "button_generic_1";

	private bool _hasSubscribedToEvents;

	private VideoAdPrize _curentVideoAdPrize;

	private const float CHECK_CACHE_STATE_TIMEOUT = 10f;

	private const int CHECK_CACHE_STATE_COUNT = 3;

	private IEnumerator _checkCacheState;

	public void Init(int earnCurrencyProfileIndex, string title, string desc, string iconName, int price, Action onClickDelegate, UIScrollView panel)
	{
		_earnCurrencyProfileIndex = earnCurrencyProfileIndex;
		_onClickDelegate = onClickDelegate;
		_prizeIcons = new List<UISprite>();
		_loadingLabel.text = Strings.Get("EARN_COINS_LOADING");
		_noVideosLabel.text = Strings.Get("EARN_COINS_LOADING_FAILED");
		_noVideosTryAgainLabel.text = Strings.Get("BUTTON_TRY_AGAIN");
		GameObject gameObject = _tvContainer.gameObject.AddChild(_tvAnimatedIcon);
		gameObject.GetComponent<FitSpriteToWidget>().widgetContainer = _tvContainer;
		gameObject.GetComponent<FitSpriteToWidget>().FitNow();
		gameObject.transform.GetChild(0).GetChild(0).GetChild(0)
			.gameObject.SetActive(value: false);
		base.gameObject.GetComponent<UIDragScrollView>().scrollView = panel;
		SubscribeToEvents();
		RefreshButton();
	}

	private void OnEnable()
	{
		SubscribeToEvents();
	}

	private void OnDisable()
	{
		AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.EARN_COINS_VIDEOS, _currentButtonState == ButtonState.VideoReady);
		UnsubscribeToEvents();
		CoroutineUtil.StopCoroutine(_checkCacheState);
	}

	private void OnDestroy()
	{
		UnsubscribeToEvents();
		CoroutineUtil.StopCoroutine(_checkCacheState);
	}

	private void SubscribeToEvents()
	{
		if (!_hasSubscribedToEvents)
		{
			_hasSubscribedToEvents = true;
			UIScreenController.OnApplicationResumed += UIScreenController_OnApplicationResumed;
			AdSystemManager.onFinishedWatchingVideo += OnFinishedWatchingVideo;
			AdSystemManager.onCachedVideo += OnVideoCached;
		}
	}

	private void UnsubscribeToEvents()
	{
		if (_hasSubscribedToEvents)
		{
			_hasSubscribedToEvents = false;
			UIScreenController.OnApplicationResumed -= UIScreenController_OnApplicationResumed;
			AdSystemManager.onFinishedWatchingVideo -= OnFinishedWatchingVideo;
			AdSystemManager.onCachedVideo -= OnVideoCached;
		}
	}

	private void OnFinishedWatchingVideo(AdLocationInfo adLocationInfo)
	{
		RefreshButton();
	}

	private void OnVideoCached(AdLocationInfo adLocationInfo, bool wasAlreadyCached)
	{
		CoroutineUtil.StopCoroutine(_checkCacheState);
		SetButtonState(ButtonState.VideoReady);
	}

	private ButtonState RefreshButton()
	{
		bool flag = AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS);
		ButtonState buttonState = (flag ? ButtonState.VideoReady : ButtonState.Loading);
		AdTrackingSystemLogger.LogFillRequest(AdLocations.EARN_COINS_VIDEOS, flag);
		SetButtonState(buttonState);
		return buttonState;
	}

	private void UIScreenController_OnApplicationResumed()
	{
		RefreshButton();
	}

	[UsedImplicitly]
	private void OnClick()
	{
		switch (_currentButtonState)
		{
		case ButtonState.NoVideo:
			Flurry.LogEventWithAParameter("Video Ads button clicked", "State;Reward", string.Concat(ButtonState.NoVideo, ";No Reward"));
			RefreshButton();
			break;
		case ButtonState.VideoReady:
		{
			bool flag = AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS);
			AdTrackingSystemLogger.LogFillRequest(AdLocations.EARN_COINS_VIDEOS, flag);
			if (flag)
			{
				AdTrackingSystemLogger.LogVideoAdButtonClick(AdLocations.EARN_COINS_VIDEOS);
				SetButtonState(ButtonState.Loading);
				Flurry.LogEventWithAParameter("Video Ads button clicked", "State;Reward", string.Concat(ButtonState.VideoReady, ";", _curentVideoAdPrize.AsString()));
				EarnCoinsData.Trigger(_earnCurrencyProfileIndex);
				_onClickDelegate.SafeInvoke();
			}
			else
			{
				RefreshButton();
			}
			break;
		}
		}
	}

	private void SetButtonText(string title, string desc, int price, bool keys)
	{
		_title.text = title;
		_description.text = desc;
		if (keys && _priceLabelKeys != null)
		{
			_priceLabelKeys.text = price.ToString();
		}
	}

	private void SetButtonState(ButtonState state)
	{
		if (!(base.gameObject == null) && base.gameObject.activeInHierarchy)
		{
			_currentButtonState = state;
			GameObject stateGameObjectActive;
			switch (state)
			{
			case ButtonState.VideoReady:
				stateGameObjectActive = _videoAvailableGameObject;
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.EARN_COINS_VIDEOS, isCached: true);
				break;
			case ButtonState.Loading:
				stateGameObjectActive = _loadingGameObject;
				CoroutineUtil.StopCoroutine(_checkCacheState);
				_checkCacheState = CheckCacheState();
				CoroutineUtil.StartCoroutine(_checkCacheState);
				break;
			case ButtonState.NoVideo:
				stateGameObjectActive = _noVideosGameObject;
				UpdateNoFillState();
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.EARN_COINS_VIDEOS, "NoFillButtonTimeout");
				break;
			default:
				stateGameObjectActive = _loadingGameObject;
				break;
			}
			SetStateGameObjectActive(stateGameObjectActive);
		}
	}

	private void UpdateNoFillState()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			_noVideosLabel.text = Strings.Get("CLOUDSAVE_STATUS_OFFLINE_INFO");
		}
		else
		{
			_noVideosLabel.text = Strings.Get("EARN_COINS_LOADING_FAILED");
		}
	}

	private IEnumerator CheckCacheState()
	{
		float delay = 3.3333333f;
		for (int i = 0; i < 3; i++)
		{
			yield return new WaitForSeconds(delay);
			bool isAdAvailable = AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS);
			if (i == 0)
			{
				AdTrackingSystemLogger.LogFillRequest(AdLocations.EARN_COINS_VIDEOS, isAdAvailable);
			}
			if (isAdAvailable)
			{
				SetButtonState(ButtonState.VideoReady);
				yield break;
			}
			if (AdSystemManager.HaveAllCachePoolPriorityListsWrapped(AdLocations.EARN_COINS_VIDEOS.id))
			{
				break;
			}
		}
		SetButtonState(ButtonState.NoVideo);
	}

	private void SetStateGameObjectActive(GameObject go)
	{
		if (_videoAvailableGameObject != null)
		{
			_videoAvailableGameObject.SetActive(value: false);
		}
		if (_loadingGameObject != null)
		{
			_loadingGameObject.SetActive(value: false);
		}
		if (_noVideosGameObject != null)
		{
			_noVideosGameObject.SetActive(value: false);
		}
		if (go != null)
		{
			go.SetActive(value: true);
		}
		if (go == _videoAvailableGameObject || go == _noVideosGameObject)
		{
			if (_currentButtonState == ButtonState.VideoReady)
			{
				_curentVideoAdPrize = VideoAdPrizeManager.instance.GetVideoAdPrize();
				SetButtonText(GetTitleForVideoAdPrize(_curentVideoAdPrize.prizeType), GetDescriptionForVideoAdPrize(_curentVideoAdPrize.prizeType), VideoAdPrizeManager.instance.GetAmountOfKeysRewardedForVideo(), keys: true);
				SetPrizeIconState(_curentVideoAdPrize);
				LogButtonShownToFlurry(_curentVideoAdPrize);
			}
			_backgroundFill.spriteName = "backgroud_callout_square";
			_backgroundFill.color = GlobalColors.PRIMARY_ACTION_COLOR;
			_fillOverlay.spriteName = "whitebox_div";
			_fillOverlay.enabled = true;
			_collider.enabled = true;
			_backgroundHighlightFill.enabled = true;
			_overlayHighlightFill.enabled = true;
		}
		else
		{
			_overlayHighlightFill.enabled = false;
			_backgroundHighlightFill.enabled = false;
			_fillOverlay.enabled = false;
			_collider.enabled = false;
			_backgroundFill.color = GlobalColors.PRIMARY_ACTION_COLOR_UNAVAILABLE;
			_backgroundFill.spriteName = "button_generic_1";
		}
	}

	private string GetTitleForVideoAdPrize(VideoAdPrizeType type)
	{
		return Strings.Get(GetTitleStringIdForVideoAdPrize(type));
	}

	private string GetTitleStringIdForVideoAdPrize(VideoAdPrizeType type)
	{
		return type switch
		{
			VideoAdPrizeType.SuperMysteryBox => "WEEKLYHUNT_PRIZE_SUPERMYSTERYBOX_TITLE_SHORT", 
			VideoAdPrizeType.MysteryBox => "UPGRADES_MYSTERY_BOX", 
			VideoAdPrizeType.MiniMysteryBox => "MINI_MYSTERY_BOX", 
			VideoAdPrizeType.Keys => "PRIZE_POPUP_REWARD_KEY_PLURAL", 
			VideoAdPrizeType.Coins => "FACEBOOK_PROMOTION_COIN_CALLOUT", 
			VideoAdPrizeType.DailyVideoPrize => "PRIZE_POPUP_REWARD_KEY_PLURAL", 
			_ => string.Empty, 
		};
	}

	private string GetDescriptionForVideoAdPrize(VideoAdPrizeType type)
	{
		return type switch
		{
			VideoAdPrizeType.SuperMysteryBox => string.Format("{0} {1}", 1, Strings.Get("WEEKLYHUNT_PRIZE_SUPERMYSTERYBOX_TITLE_SHORT")), 
			VideoAdPrizeType.MysteryBox => string.Format("{0} {1}", 1, Strings.Get("UPGRADES_MYSTERY_BOX")), 
			VideoAdPrizeType.MiniMysteryBox => string.Format("{0} {1}", 1, Strings.Get("MINI_MYSTERY_BOX")), 
			VideoAdPrizeType.Keys => string.Format("{0} {1}", 1, Strings.Get("PRIZE_POPUP_REWARD_KEY_SINGULAR")), 
			VideoAdPrizeType.Coins => string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_COINS"), 1000), 
			VideoAdPrizeType.DailyVideoPrize => string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_KEYS"), 2), 
			_ => string.Empty, 
		};
	}

	private void SetPrizeIconState(VideoAdPrize videoAdPrize)
	{
		List<EarnCoinButtonPrizeIconSetup> prizeIconSetupsWithTypeAndDeactivateTheRest = PrizeIconSetup.GetPrizeIconSetupsWithTypeAndDeactivateTheRest(_prizeIconSetups, videoAdPrize.prizeType);
		for (int i = 0; i < prizeIconSetupsWithTypeAndDeactivateTheRest.Count; i++)
		{
			prizeIconSetupsWithTypeAndDeactivateTheRest[i].iconSprite = GetPrizeIcon(i);
			prizeIconSetupsWithTypeAndDeactivateTheRest[i].Setup();
		}
	}

	private UISprite GetPrizeIcon(int index)
	{
		while (index >= _prizeIcons.Count)
		{
			UISprite uISprite = UnityEngine.Object.Instantiate(_spritePrefab, _prizeContent.transform);
			uISprite.depth = _prizeIconDepth;
			_prizeIcons.Add(uISprite);
		}
		return _prizeIcons[index];
	}

	private void LogButtonShownToFlurry(VideoAdPrize videoAdPrize)
	{
		if (videoAdPrize.prizeType == VideoAdPrizeType.DailyVideoPrize)
		{
			AdsSave.videosWatchedSinceDailyKeys = 0;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		PrizePoolState prizePoolState = VideoAdPrizeManager.instance.prizePoolState;
		int num = ((prizePoolState != PrizePoolState.Intro) ? AdsSave.currentRandomVideoAdPrizeIndex : AdsSave.currentIntroVideoAdPrizeIndex);
		int videosWatchedSinceDailyKeys = AdsSave.videosWatchedSinceDailyKeys;
		dictionary.Add(prizePoolState.ToString(), "step{0}|sinceDailyKeys{1}|{2}".Populate(num, videosWatchedSinceDailyKeys, videoAdPrize.AsString()));
		dictionary.Add("SinceDailyKeys" + prizePoolState, AdsSave.videosWatchedSinceDailyKeys.ToString());
		Flurry.LogEventWithSeveralParameters("Free Stuff Quick Video Ads Button Shown", dictionary);
	}
}
