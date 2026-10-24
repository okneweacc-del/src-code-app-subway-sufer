using Data.Save;
using Info;
using JetBrains.Annotations;
using UI.Generic.Layout;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.Screens.UpgradeScreenInternal;

public class UpgradeHelper : MonoBehaviour
{
	private enum AnimState
	{
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3
	}

	[SerializeField]
	private UISprite _powerupIcon;

	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private UILabel _descriptionLabel;

	[SerializeField]
	private UILabel _amountLabel;

	[SerializeField]
	private UILabel _priceLabel;

	[SerializeField]
	private UISprite _priceCoinSprite;

	[SerializeField]
	private UISprite _priceKeySprite;

	[SerializeField]
	private GameObject _priceSpacing;

	[SerializeField]
	private Layout _priceLayout;

	[SerializeField]
	private DescriptionButtonHelper _prizesButton;

	[SerializeField]
	private BuyButtonIngame _buyButton;

	[SerializeField]
	private UILabel _buttonPriceLabel;

	[SerializeField]
	private UISprite _buttonPriceCoinSprite;

	[SerializeField]
	private UISprite _buttonPriceKeySprite;

	[SerializeField]
	private GameObject _buttonPriceSpacing;

	[SerializeField]
	private Layout _buttonPriceLayout;

	[SerializeField]
	private GameObject _expandedContent;

	[SerializeField]
	private UISprite _openButtonSprite;

	[Header("Permanent")]
	[SerializeField]
	private UISprite _fullBgCollapsed;

	[SerializeField]
	private UISprite _fullBgExpanded;

	[SerializeField]
	private UITierHelper _tierHelper;

	[Header("Button in description field")]
	[SerializeField]
	private GameObject _descriptionButton;

	[SerializeField]
	private UILabel _descButtonLabel;

	[SerializeField]
	private BreadCrumbHelper _breadCrumbHelper;

	private TableElementAlwaysVisible _tableElement;

	private UpgradeType _type;

	private UpgradeScreen _upgradeScreen;

	private bool _hasInited;

	private UIPanel _parentPanel;

	private bool _foldedOut;

	private UIPlayTween[] _tweens;

	private bool _wasForceOpened;

	private bool _freeTokenBoxAvailableOnInit;

	private AnimState _animState;

	private AnimState animState
	{
		get
		{
			return _animState;
		}
		set
		{
			_animState = value;
		}
	}

	private void OnEnable()
	{
		InventorySave.powerupsChanged += RefreshEvent;
		PurchaseHandler.Instance.onUpgradePurchase += RefreshEvent;
		RefreshUpgrade();
		if (_hasInited)
		{
			SetFoldout();
		}
	}

	private void OnDisable()
	{
		InventorySave.powerupsChanged -= RefreshEvent;
		PurchaseHandler.Instance.onUpgradePurchase -= RefreshEvent;
	}

	public void InitSingle(UpgradeType type, UIPanel panel)
	{
		_type = type;
		_parentPanel = panel;
		_powerupIcon.spriteName = Upgrades.upgrades[_type].iconName;
		_powerupIcon.MakePixelPerfect();
		_titleLabel.text = Strings.Get(Upgrades.upgrades[_type].name);
		string description = Upgrades.upgrades[_type].description;
		_descriptionLabel.text = ((!(description == string.Empty)) ? Strings.Get(description) : string.Empty);
		_freeTokenBoxAvailableOnInit = !UserSave.hasUserRecievedFreeTokenMysteryBox;
		_priceLabel.text = string.Empty + Upgrades.upgrades[_type].GetPrice(0);
		_priceCoinSprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Coins);
		_priceKeySprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Keys);
		_buttonPriceLabel.text = string.Empty + Upgrades.upgrades[_type].GetPrice(0);
		_buttonPriceCoinSprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Coins);
		_buttonPriceKeySprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Keys);
		_priceSpacing.SetActive(value: false);
		_buttonPriceSpacing.SetActive(value: false);
		animState = AnimState.Closed;
		_buyButton.InitBuyButton(_type);
		if (_prizesButton != null)
		{
			_prizesButton.Init(_type);
		}
		if (_type != UpgradeType.mysterybox && _type != UpgradeType.tokenmysterybox && _descriptionButton != null)
		{
			Object.Destroy(_descriptionButton);
		}
		if (_type == UpgradeType.tokenmysterybox && _breadCrumbHelper != null)
		{
			Object.Destroy(_breadCrumbHelper.gameObject);
		}
		_hasInited = true;
		RefreshUpgrade();
		SetFoldout();
	}

	public void InitPermanent(UpgradeType type, UIPanel panel)
	{
		_type = type;
		_parentPanel = panel;
		_powerupIcon.spriteName = Upgrades.upgrades[_type].iconName;
		_powerupIcon.MakePixelPerfect();
		_titleLabel.text = Strings.Get(Upgrades.upgrades[_type].name);
		string description = Upgrades.upgrades[_type].description;
		_descriptionLabel.text = ((!(description == string.Empty)) ? Strings.Get(description) : string.Empty);
		if (_tierHelper != null)
		{
			_tierHelper.SetupTiers(_type);
		}
		animState = AnimState.Closed;
		if (_buyButton != null)
		{
			_buyButton.InitBuyButton(_type);
		}
		_hasInited = true;
		RefreshUpgrade();
		SetFoldout();
	}

	public void UpgradePurchased(UpgradeType type)
	{
		switch (type)
		{
		case UpgradeType.mysterybox:
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox, ObtainableItemSource.IngameCurrencyPurchase);
			UIScreenController.Instance.PushCelebrationPopup();
			break;
		case UpgradeType.tokenmysterybox:
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.TokenMysteryBox, ObtainableItemSource.IngameCurrencyPurchase);
			UIScreenController.Instance.PushCelebrationPopup();
			break;
		}
		switch (type)
		{
		case UpgradeType.scorebooster:
			Flurry.LogEvent("Boost Scorebooster purchased");
			break;
		case UpgradeType.headstart2000:
			Flurry.LogEvent("Boost Headstart2000 purchased");
			break;
		case UpgradeType.hoverboard:
			Flurry.LogEvent("Boost Hoverboard purchased");
			break;
		case UpgradeType.coinmagnet:
			Flurry.LogEventWithAParameter("Boost Coinmagnet purchased", "Tier", InventorySave.GetTier(type).ToString());
			break;
		case UpgradeType.doubleMultiplier:
			Flurry.LogEventWithAParameter("Boost 2x multiplier purchased", "Tier", InventorySave.GetTier(type).ToString());
			break;
		case UpgradeType.jetpack:
			Flurry.LogEventWithAParameter("Boost jetpack purchased", "Tier", InventorySave.GetTier(type).ToString());
			break;
		case UpgradeType.letters:
			Flurry.LogEventWithAParameter("Boost letters purchased", "Tier", InventorySave.GetTier(type).ToString());
			break;
		case UpgradeType.supersneakers:
			Flurry.LogEventWithAParameter("Boost supersneakers purchased", "Tier", InventorySave.GetTier(type).ToString());
			break;
		case UpgradeType.mysterybox:
			Flurry.LogEvent("Boost MysteryBox purchased");
			break;
		case UpgradeType.tokenmysterybox:
			Flurry.LogEventWithAParameter("Character Token Box Opened", "ScreenName", "UpgradeScreen");
			break;
		}
		RefreshUpgrade();
	}

	private void RefreshEvent(UpgradeType type, int amount)
	{
		if (type == _type)
		{
			RefreshUpgrade();
		}
	}

	private void RefreshUpgrade()
	{
		if (!_hasInited)
		{
			return;
		}
		switch (_type)
		{
		case UpgradeType.hoverboard:
		case UpgradeType.headstart2000:
		case UpgradeType.scorebooster:
			if (_amountLabel != null)
			{
				_amountLabel.text = string.Format(Strings.Get("UPGRADES_HELPER_YOU_HAVE"), InventorySave.GetAmount(_type));
			}
			_priceLayout.gameObject.SetActive(!_foldedOut);
			break;
		case UpgradeType.jetpack:
		case UpgradeType.supersneakers:
		case UpgradeType.coinmagnet:
		case UpgradeType.letters:
		case UpgradeType.doubleMultiplier:
		{
			_tierHelper.ResetTiers();
			bool flag = InventorySave.GetTier(_type) >= Upgrades.upgrades[_type].numberOfTiers - 1;
			_fullBgCollapsed.gameObject.SetActive(!_foldedOut && flag);
			_fullBgExpanded.gameObject.SetActive(_foldedOut && flag);
			_priceLayout.gameObject.SetActive(!_foldedOut && !flag);
			_buyButton.gameObject.SetActive(_foldedOut && !flag);
			if (!flag)
			{
				string text = Upgrades.upgrades[_type].GetPrice(InventorySave.GetTier(_type) + 1) + string.Empty;
				_priceLabel.text = text;
				_buttonPriceLabel.text = text;
			}
			break;
		}
		case UpgradeType.mysterybox:
			_amountLabel.text = Strings.Get("UPGRADES_HELPER_MYSTERY_BOX");
			_descButtonLabel.text = Strings.Get("UPGRADES_MYSTERYBOX_VIEW_PRIZES");
			_priceLayout.gameObject.SetActive(!_foldedOut);
			break;
		case UpgradeType.tokenmysterybox:
			_amountLabel.text = Strings.Get("UPGRADES_HELPER_MYSTERY_BOX");
			_descButtonLabel.text = Strings.Get("UPGRADES_MYSTERYBOX_VIEW_PRIZES");
			_priceLayout.gameObject.SetActive(!_foldedOut);
			if (_freeTokenBoxAvailableOnInit)
			{
				if (!UserSave.hasUserRecievedFreeTokenMysteryBox)
				{
					_priceLabel.text = Strings.Get("FREE_WITH_EXCLAMATION");
					_priceSpacing.SetActive(value: true);
					_priceCoinSprite.gameObject.SetActive(value: false);
					_priceKeySprite.gameObject.SetActive(value: false);
					_buttonPriceLabel.text = Strings.Get("FREE_WITH_EXCLAMATION");
					_buttonPriceCoinSprite.gameObject.SetActive(value: false);
					_buttonPriceKeySprite.gameObject.SetActive(value: false);
					_buttonPriceSpacing.SetActive(value: true);
				}
				else if (_freeTokenBoxAvailableOnInit && UserSave.hasUserRecievedFreeTokenMysteryBox)
				{
					_priceLabel.text = string.Empty + Upgrades.upgrades[_type].GetPrice(0);
					_priceSpacing.SetActive(value: false);
					_priceCoinSprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Coins);
					_priceKeySprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Keys);
					_buttonPriceLabel.text = string.Empty + Upgrades.upgrades[_type].GetPrice(0);
					_buttonPriceCoinSprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Coins);
					_buttonPriceKeySprite.gameObject.SetActive(Upgrades.upgrades[_type].priceType == PurchaseData.UnlockType.Keys);
					_buttonPriceSpacing.SetActive(value: false);
				}
			}
			break;
		}
		if (_priceLayout.gameObject.activeInHierarchy)
		{
			_priceLayout.Align(once: true);
		}
		if (_buttonPriceLayout.gameObject.activeInHierarchy)
		{
			_buttonPriceLayout.Align(once: true);
			_descriptionLabel.UpdateAnchors();
		}
	}

	[UsedImplicitly]
	private void OnClick()
	{
		AnimationStarted();
	}

	private void AnimationStarted()
	{
		if (animState == AnimState.Closed || animState == AnimState.Closing)
		{
			animState = AnimState.Opening;
			if (_upgradeScreen == null)
			{
				_upgradeScreen = NGUITools.FindInParents<UpgradeScreen>(base.gameObject);
			}
			foreach (UpgradeHelper cachedUpgradeHelper in _upgradeScreen.cachedUpgradeHelpers)
			{
				if (cachedUpgradeHelper != this && cachedUpgradeHelper.animState == AnimState.Open)
				{
					cachedUpgradeHelper.SendMessage("OnClick");
				}
			}
		}
		else
		{
			animState = AnimState.Closing;
		}
		FoldClicked();
		if (_parentPanel != null)
		{
			_parentPanel.Refresh();
		}
		Collider component = base.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	[UsedImplicitly]
	public void AnimationEnded()
	{
		if (animState == AnimState.Closing || animState == AnimState.Opening)
		{
			Collider component = base.gameObject.GetComponent<Collider>();
			if (component != null)
			{
				component.enabled = true;
			}
			animState = ((animState != AnimState.Closing) ? AnimState.Open : AnimState.Closed);
			TriggerContent();
		}
		else
		{
			SDebug.LogError("Unexpected anim state: " + animState, this);
		}
	}

	private void ForceExpandContent(bool expand)
	{
		_wasForceOpened = expand;
		_tableElement = base.gameObject.GetComponent<TableElementAlwaysVisible>();
		if (_tableElement != null)
		{
			_tableElement.ToggleButtonScale();
		}
		if (_tweens == null)
		{
			_tweens = base.gameObject.GetComponents<UIPlayTween>();
		}
		if (_tweens != null)
		{
			UIPlayTween[] tweens = _tweens;
			foreach (UIPlayTween uIPlayTween in tweens)
			{
				uIPlayTween.Play(expand);
			}
		}
		AnimationStarted();
	}

	private void SetFoldout()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.BiggerJackpot);
		if (_type == UpgradeType.mysterybox && activeEventManager != null)
		{
			if (!_foldedOut)
			{
				ForceExpandContent(expand: true);
			}
		}
		else if (_wasForceOpened)
		{
			ForceExpandContent(expand: false);
		}
		else
		{
			ContentActivation(_foldedOut);
		}
	}

	private void TriggerContent()
	{
		if (_foldedOut)
		{
			ContentActivation(_foldedOut);
		}
	}

	private void FoldClicked()
	{
		if (!_foldedOut)
		{
			_foldedOut = true;
			_openButtonSprite.gameObject.SetActive(value: false);
			_priceLayout.gameObject.SetActive(value: false);
			if (_fullBgCollapsed != null)
			{
				_fullBgCollapsed.gameObject.SetActive(value: false);
			}
		}
		else
		{
			_foldedOut = false;
			ContentActivation(foldedOut: false);
		}
	}

	private void ContentActivation(bool foldedOut)
	{
		_descriptionLabel.gameObject.SetActive(foldedOut);
		_openButtonSprite.gameObject.SetActive(!foldedOut);
		_expandedContent.SetActive(foldedOut);
		RefreshUpgrade();
	}
}
