using System;
using System.Collections.Generic;
using InApp;
using Info.VideoRewardCooldowns;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using SYBO.Subway;
using UI.ButtonHelpers;
using UI.FreeStuff;
using UI.Generic.Layout;
using UI.Helpers;
using UI.Popups.NotEnoughCurrencyPopupInternal;
using UI.Screens;
using UI.Screens.NotEnoughCurrencyPopupInternal;
using UI.Screens.RewardManagement;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups;

public class NotEnoughCurrencyPopup : UIPopup
{
	private enum PopupClosingAction
	{
		None = 0,
		Buy = 1,
		Cancel = 2,
		Ok = 3,
		VideoCoins = 4
	}

	private const int FIRST_PURCHASE_BONUS_BANNER_WIDTH = 305;

	[SerializeField]
	private UILabel _popupTitle;

	[SerializeField]
	private UILabel _popupDescription;

	[SerializeField]
	private GameObject _uiContentCoins;

	[SerializeField]
	private GameObject _uiContentKeys;

	[SerializeField]
	private GameObject _emptySpacePrefab;

	[SerializeField]
	private GameObject _uiContentVideoForCoins;

	[SerializeField]
	private Transform _dynamicContentRoot;

	[SerializeField]
	private VerticalLayout _dynamicContentVerticalLayout;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private GameObject _currencyButtonPrefab;

	[SerializeField]
	private GameObject _firstPurchaseBonusBannerPrefab;

	[SerializeField]
	private GameObject _okButtonPrefab;

	[SerializeField]
	private GameObject _buyMoreInStoreLabelPrefab;

	[SerializeField]
	private GameObject _emptySpaceAboveDynamicContent;

	[SerializeField]
	private UIWidget _bgOutline;

	[SerializeField]
	private UIWidget _closeButton;

	private PopupClosingAction _closingAction;

	private static InAppManagerPopupData _popupData;

	private UiElementsManager _uiElementsManager;

	private GameObject _coinsForAdButton;

	private GameObject _buyCurrencyButton;

	private GameObject _okButton;

	private GameObject _buyMoreInStorelabel;

	private GameObject _firstPurchaseBanner;

	private int _newCoinPrice;

	private static RewardForVideoButton _rewardForVideoButton;

	private const string STANDARD_VIDEO_REWARD = "-1";

	private const string VIDEO_AVAIL_YES = "Yes";

	private const string VIDEO_AVAIL_FAIL = "No_caching_failed";

	private const string VIDEO_AVAIL_NO_TRY = "No_caching_not_attempted";

	private const string BUTTON_NAME_CLOSE = "Closed";

	private const string BUTTON_NAME_IAP = "IAP";

	private const string BUTTON_NAME_VIDEO = "Video";

	private const string BUTTON_NO_COOLDOWN = "No Cooldown";

	private const string BUTTON_COOLDOWN = "Cooldown_{0}_watched";

	private string _flurryVideoAvailable;

	private string _flurryButtonName;

	private string _flurryVideoReward;

	private string _flurryCooldownStep;

	private VideoButtonPayload _videoButtonPayload;

	private bool isInRun => Game.Instance.IsInGame.Value && UIScreenController.Instance.GetTopScreenName() == "IngameUI";

	protected override float screenOffset => (!isInRun) ? (-13f) : 0f;

	private static int GetCachedRewardAmount()
	{
		return _rewardForVideoButton.cachedRewardAmount;
	}

	public override bool ShouldShowTopBar()
	{
		return !isInRun;
	}

	protected override void Init()
	{
		_videoButtonPayload = CreateButtonPayload();
		_uiElementsManager = new UiElementsManager();
	}

	private static VideoButtonPayload CreateButtonPayload()
	{
		VideoButtonPayload videoButtonPayload = new VideoButtonPayload();
		videoButtonPayload.iconName = "icon_coinPack_3";
		videoButtonPayload.title = "COINS_FOR_AD_BUTTON_POPUP_TITLE";
		videoButtonPayload.description = "COINS_FOR_AD_BUTTON_DESCRIPTION";
		videoButtonPayload.firstColor = ColorData.instance.peacock;
		videoButtonPayload.firstColorDark = ColorData.instance.royale;
		videoButtonPayload.firstHighlightColor = ColorData.instance.sky.WithAlpha(0.6f);
		videoButtonPayload.secondColor = ColorData.instance.dutch;
		videoButtonPayload.secondColorDark = ColorData.instance.safran;
		videoButtonPayload.secondHighlightColor = ColorData.instance.sun.WithAlpha(0.4f);
		videoButtonPayload.titleColor = ColorData.instance.sun;
		videoButtonPayload.descriptionColor = ColorData.instance.white;
		videoButtonPayload.isAvailable = () => CooldownManager.HasCharge("coins");
		videoButtonPayload.onVideoWatched = () =>
		{
			CooldownManager.Consume("coins");
			string value = GetCachedRewardAmount().ToString();
			string value2 = ((CooldownManager.GetChargesUsed("coins") - 1 != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins") - 1) : "No Cooldown");
			Flurry.LogEventWithSeveralParameters("NotEnoughCurrencyPopup", new Dictionary<string, string>
			{
				{ "Video_Available", "Yes" },
				{ "Button_Name", "Video" },
				{ "Video_Reward", value },
				{ "Cooldown_step", value2 }
			});
		};
		videoButtonPayload.getRewardAmount = CoinsForVideoManager.instance.GetRewardAmount;
		videoButtonPayload.adLocation = AdLocations.FREE_COINS_POPUP_VIDEOS;
		videoButtonPayload.rewardType = CelebrationRewardType.coins;
		videoButtonPayload.unlockItemSource = UnlockItemSource.NotEnoughCurrencyPopup;
		videoButtonPayload.flurryName = "Coins";
		return videoButtonPayload;
	}

	protected override void Show()
	{
		RefreshPopupContent();
		AdSystemManager.onFinishedWatchingVideo += HandleOnCoinsForVideoFinished;
	}

	private void RefreshPopupContent()
	{
		_popupData = InAppManager.instance.GetPopupData();
		if (_popupData != null)
		{
			_popupTitle.text = _popupData.popupTitle;
			_popupTitle.color = ColorData.instance.midnight;
			_popupDescription.text = ((_newCoinPrice != 0) ? string.Format(Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_COINS"), _newCoinPrice) : _popupData.popupDescription);
			_popupDescription.color = ColorData.instance.peacock;
			if (_popupData.isErrorMessage)
			{
				_uiContentCoins.SetActive(value: false);
				_uiContentKeys.SetActive(value: false);
			}
			else if (_popupData.isCoinsPack)
			{
				_uiContentCoins.SetActive(value: true);
				_uiContentKeys.SetActive(value: false);
				if (base.parentScreen == "CharacterNameEventPopup")
				{
					Flurry.LogEvent("Attempt to buy coins for Wordy Weekend");
				}
			}
			else
			{
				_uiContentCoins.SetActive(value: false);
				_uiContentKeys.SetActive(value: true);
				if (base.parentScreen == "CharacterNameEventPopup")
				{
					Flurry.LogEvent("Attempt to buy keys for the Wordy Weekend");
				}
			}
			HideDynamicUiElements();
			if (_popupData.canMakePurchase)
			{
				UpdateFirstPurchaseBonusBannerStatus();
				ActivateUiElement(UiElementType.CoinButtonSmall, _currencyButtonPrefab, OnCurrencyOfferActivated);
				if (CooldownManager.HasCharge("coins"))
				{
					ActivateUiElement(UiElementType.EmptySpaceOnTopOfButton, _emptySpacePrefab);
					ActivateUiElement(UiElementType.AdForCoinsButton, _uiContentVideoForCoins, OnVideoForCoinsActivated);
				}
				AdTrackingSystemLogger.LogFillRequest(AdLocations.FREE_COINS_POPUP_VIDEOS);
				if (_coinsForAdButton != null)
				{
					bool flag = AdSystemManager.IsCached(AdLocations.FREE_COINS_POPUP_VIDEOS);
					if (!Game.Instance.IsInGame.Value && flag && _popupData.isCoinsPack && CooldownManager.HasCharge("coins"))
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("Result", "Fill;popup");
						Flurry.LogEventWithSeveralParameters("Attempt to show not enough coin video button", dictionary);
						_coinsForAdButton.SetActive(value: true);
					}
					else if (!flag && _popupData.isCoinsPack)
					{
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
						dictionary2.Add("Result", "No_Fill;popup");
						Flurry.LogEventWithSeveralParameters("Attempt to show not enough coin video button", dictionary2);
						_coinsForAdButton.SetActive(value: false);
					}
					else
					{
						_coinsForAdButton.SetActive(value: false);
					}
				}
			}
			else
			{
				_emptySpaceAboveDynamicContent.SetActive(value: true);
				ActivateUiElement(UiElementType.OkButton, _okButtonPrefab, OnOkButtonActivated);
				ActivateUiElement(UiElementType.BuyMoreInStoreLabelSmall, _buyMoreInStoreLabelPrefab, OnBuyMoreInStoreLabelActivated);
			}
			_table.Reposition();
			_dynamicContentVerticalLayout.Align();
			_bgOutline.UpdateAnchors();
			_closeButton.UpdateAnchors();
		}
		if (_rewardForVideoButton != null)
		{
			_rewardForVideoButton.Init(_videoButtonPayload);
		}
		if (!CooldownManager.HasCharge("coins"))
		{
			_flurryVideoAvailable = "No_caching_not_attempted";
		}
		else if (CooldownManager.HasCharge("coins") && !AdSystemManager.IsCached(AdLocations.FREE_COINS_POPUP_VIDEOS))
		{
			_flurryVideoAvailable = "No_caching_failed";
		}
		else
		{
			_flurryVideoAvailable = "Yes";
		}
		if (_coinsForAdButton != null)
		{
			_coinsForAdButton.transform.SetLocalPosX(1f);
		}
		if (_buyCurrencyButton != null)
		{
			_buyCurrencyButton.transform.SetLocalPosX(1f);
		}
		if (_okButton != null)
		{
			_okButton.transform.SetLocalPosX(1f);
		}
		if (_buyMoreInStorelabel != null)
		{
			_buyMoreInStorelabel.transform.SetLocalPosX(1f);
		}
		if (_firstPurchaseBanner != null)
		{
			_firstPurchaseBanner.transform.SetLocalPosX(1f);
		}
	}

	private void HandleOnCoinsForVideoFinished(AdLocationInfo adLocationInfo)
	{
		if (_popupData.isCoinsPack)
		{
			_newCoinPrice = _popupData.currencyMissing - GetCachedRewardAmount();
			if (_newCoinPrice <= 0)
			{
				_newCoinPrice = 0;
				InAppManager.instance.DeletePopupData();
				UIScreenController.Instance.ClosePopup(screenName);
			}
			else
			{
				RefreshPopupContent();
			}
		}
	}

	private void HideDynamicUiElements()
	{
		_uiElementsManager.DeactivateAllUiElements();
	}

	private void ActivateUiElement(UiElementType uiElementType, GameObject prefab, OnActivatedPrefab onActivatedPrefab = null)
	{
		_uiElementsManager.ActivateUiElement(uiElementType, prefab, _dynamicContentRoot, onActivatedPrefab);
	}

	private void UpdateFirstPurchaseBonusBannerStatus()
	{
		_emptySpaceAboveDynamicContent.SetActive(value: true);
		if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
		{
			ActivateUiElement(UiElementType.FirstPurchaseBonusBanner, _firstPurchaseBonusBannerPrefab, OnFirstPurchaseBonusBannerSpawned);
			GameObject uIElement = _uiElementsManager.GetUIElement(UiElementType.FirstPurchaseBonusBanner);
			if (uIElement != null)
			{
				uIElement.GetComponent<FirstPurchaseBonusCharacterHelper>().Setup(new FirstPurchaseBonusCharacterHelper.FirstPurchasePayload
				{
					width = 278
				});
			}
		}
	}

	public override string GetFlurryName()
	{
		return "NotEnoughCurencyPopup";
	}

	private void OnFirstPurchaseBonusBannerSpawned(GameObject firstPurchaseBonusBannerGo, bool wasSpawned)
	{
		_firstPurchaseBanner = firstPurchaseBonusBannerGo;
		UIWidget component = firstPurchaseBonusBannerGo.GetComponent<UIWidget>();
		component.width = 305;
	}

	private void OnOkButtonActivated(GameObject okButtonGo, bool wasSpawned)
	{
		if (wasSpawned)
		{
			_okButton = okButtonGo;
			OkButtonHelper component = okButtonGo.GetComponent<OkButtonHelper>();
			component.OkButtonClicked += OnOkClicked;
		}
	}

	private void OnVideoForCoinsActivated(GameObject coinVideoGo, bool wasSpawned)
	{
		if (wasSpawned)
		{
			_coinsForAdButton = coinVideoGo;
			_rewardForVideoButton = coinVideoGo.GetComponent<RewardForVideoButton>();
			_rewardForVideoButton.Init(_videoButtonPayload);
		}
	}

	private void OnCurrencyOfferActivated(GameObject currencyButtonGo, bool wasSpawned)
	{
		_buyCurrencyButton = currencyButtonGo;
		CoinButtonHelper component = currencyButtonGo.GetComponent<CoinButtonHelper>();
		component.Init(_popupData.bestOffer, null, "NotEnoughCurrencyPopup");
		component.onClick = (Action)Delegate.Combine(component.onClick, new Action(OnBuyClicked));
	}

	private void OnBuyMoreInStoreLabelActivated(GameObject activatedGo, bool wasSpawned)
	{
		_buyMoreInStorelabel = activatedGo;
		BuyMoreInStoreLabelHelper component = activatedGo.GetComponent<BuyMoreInStoreLabelHelper>();
		component.UpdateBuyMoreInStoreLabel();
	}

	protected override void Hide()
	{
		base.Hide();
		if (_closingAction == PopupClosingAction.None)
		{
			_flurryVideoReward = "-1";
			_flurryButtonName = "Closed";
			_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
			Payout(PopupClosingAction.Cancel);
		}
		_closingAction = PopupClosingAction.None;
		_popupData = null;
		AdSystemManager.onFinishedWatchingVideo -= HandleOnCoinsForVideoFinished;
		_newCoinPrice = 0;
	}

	[UsedImplicitly]
	public void OnOkClicked()
	{
		_flurryVideoReward = "-1";
		_flurryButtonName = "Closed";
		_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
		_closingAction = PopupClosingAction.Ok;
		Payout(_closingAction);
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}

	public override void BackButtonPressed()
	{
		base.BackButtonPressed();
		if (Game.Instance.IsInGame.Value)
		{
			Revive.Instance.SendSkipRevive();
		}
	}

	[UsedImplicitly]
	public void OnBuyClicked()
	{
		if (UIScreenController.IsInstanced)
		{
			if (InAppManager.isInitialized)
			{
				_flurryVideoReward = "-1";
				_flurryButtonName = "IAP";
				_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
				_closingAction = PopupClosingAction.Buy;
				UIScreenController.Instance.CloseActivePopups(screenName);
				Payout(_closingAction);
			}
			else
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.ErrorMessage,
					stringValue = "Purchase Failed"
				});
			}
		}
	}

	[UsedImplicitly]
	public void OnCancelClicked()
	{
		_flurryVideoReward = "-1";
		_flurryButtonName = "Closed";
		_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
		_closingAction = PopupClosingAction.Cancel;
		Payout(_closingAction);
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}

	private void Payout(PopupClosingAction action)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Video_Available", _flurryVideoAvailable);
		dictionary.Add("Button_Name", _flurryButtonName);
		dictionary.Add("Video_Reward", _flurryVideoReward);
		dictionary.Add("Cooldown_step", _flurryCooldownStep);
		Flurry.LogEventWithSeveralParameters("NotEnoughCurrencyPopup", dictionary);
		if (_popupData == null)
		{
			return;
		}
		switch (action)
		{
		case PopupClosingAction.Buy:
			InAppManager.instance.DeletePopupData();
			if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
			{
				if (_popupData.isCoinsPack)
				{
					Flurry.LogEvent("Bought coins for Wordy Weekend");
				}
				else
				{
					Flurry.LogEvent("Bought keys for Wordy Weekend");
				}
			}
			break;
		case PopupClosingAction.Ok:
			InAppManager.instance.NativePurchaseFail("0");
			InAppManager.instance.DeletePopupData();
			if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
			{
				if (_popupData.isCoinsPack)
				{
					Flurry.LogEvent("Unable to buy coins for Wordy Weekend");
				}
				else
				{
					Flurry.LogEvent("Unable to buy keys for Wordy Weekend");
				}
			}
			break;
		case PopupClosingAction.Cancel:
			InAppManager.instance.NativePurchaseInappPack("0");
			InAppManager.instance.DeletePopupData();
			if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
			{
				if (_popupData.isCoinsPack)
				{
					Flurry.LogEvent("Canceled buying coins for Wordy Weekend");
				}
				else
				{
					Flurry.LogEvent("Canceled buying keys for Wordy Weekend");
				}
			}
			break;
		case PopupClosingAction.VideoCoins:
			break;
		}
	}
}
