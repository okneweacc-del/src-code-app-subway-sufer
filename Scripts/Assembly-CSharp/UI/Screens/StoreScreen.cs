using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Data.Save;
using InApp;
using Info;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace UI.Screens;

public class StoreScreen : UIScreen
{
	private const string ONLINESETTINGS_MOST_POPULAR_STICKER_PRODUCT_ID_WITHOUT_PREFIX = "most_popular_sticker_product_id_without_prefix";

	private const int FIRST_PURCHASE_BONUS_BANNER_COINSCREEN_HEIGHT = 60;

	private const int MOST_POPULAR_STICKER_PADDING = 3;

	private static readonly Vector2 MOST_POPULAR_STICKER_POSITION = new Vector2(-132f, 19.4f);

	private static readonly string[] MOST_POPULAR_STICKER_DEFAULT_PRODUCT_ID_WITHOUT_PREFIX = new string[2]
	{
		InAppData.StripProductPrefixes("com.kiloo.subways.coinstier2.5"),
		InAppData.StripProductPrefixes("com.kiloo.subways.keystier1.5")
	};

	[FormerlySerializedAs("coinPrefab")]
	[SerializeField]
	private GameObject _coinPrefab;

	[FormerlySerializedAs("restorePrefab")]
	[SerializeField]
	private GameObject _restorePrefab;

	[SerializeField]
	private GameObject _specialSaleWinterHolidayNotificationPrefab;

	[SerializeField]
	private GameObject _specialSaleNewYearNotificationPrefab;

	[SerializeField]
	private GameObject _specialSaleBlackFridayNotificationPrefab;

	[SerializeField]
	private GameObject _doubleCoinsPrefab;

	[SerializeField]
	private GameObject _starterPackPrefab;

	[SerializeField]
	private GameObject _firstPurchaseBonusBannerPrefab;

	[SerializeField]
	private GameObject _welcomePackBannerPrefab;

	[SerializeField]
	private GameObject _mostPopularStickerPrefab;

	[FormerlySerializedAs("_ScrollView")]
	[FormerlySerializedAs("_parentDragPanel")]
	[SerializeField]
	private UIScrollView _scrollView;

	[FormerlySerializedAs("listTitleComponent")]
	[SerializeField]
	private GameObject _listTitleComponent;

	[SerializeField]
	private UIPanel _coinScreenPanel;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private UITable _contentTable;

	[SerializeField]
	private bool _isPopup;

	private const int TITLE_POPUP_FONT_SIZE = 23;

	private static bool _shouldRefreshScreen = false;

	private FirstPurchaseBonusCharacterHelper _firstPurchaseBonusBanner;

	private bool _wasWelcomePackActive;

	private bool _subscribedToEvents;

	private string[] _mostPopularStickerProductIds;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action coinScreenHasAppeared__BackingField;

	public static bool openedFromWelcomePackGetNowClick = false;

	private bool _firstPurchaseBonusAnimationActivated = true;

	public event Action coinScreenHasAppeared
	{
		add
		{
			Action action = coinScreenHasAppeared__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref coinScreenHasAppeared__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = coinScreenHasAppeared__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref coinScreenHasAppeared__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public override string GetFlurryName()
	{
		return "CoinsUI_shop";
	}

	protected override void Init()
	{
		HandleSafeArea();
	}

	protected virtual void HandleSafeArea()
	{
		if (SafeArea.IsRestricted)
		{
			_scrollPanel.topAnchor.absolute -= SafeArea.OFFSET_TOP;
			_scrollPanel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
			_scrollPanel.ResetAndUpdateAnchors();
		}
	}

	private void OnApplicationResumed()
	{
		FillTable();
	}

	private void Update()
	{
		if (_shouldRefreshScreen)
		{
			RefreshCurrencyEarners();
		}
	}

	protected override void Show()
	{
		SubscribeToEvents();
		if (_footerHandler != null)
		{
			_footerHandler.HandleStickers();
		}
		RefreshMostPopularStickerProductIds();
		RefreshCurrencyEarners();
	}

	protected override void Hide()
	{
		openedFromWelcomePackGetNowClick = false;
		UnsubscribeFromEvents();
	}

	private void SubscribeToEvents()
	{
		if (!_subscribedToEvents)
		{
			UIScreenController.OnApplicationResumed += OnApplicationResumed;
			UIScreenController.OnPopupShown += OnPopupShown;
			UIScreenController.OnPopupClosed += OnPopupClosed;
			OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
			FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += RefreshCurrencyEarners;
			if (InAppManager.instance != null)
			{
				InAppManager.instance.onProductRequestDone += OnProductRequestDone;
				InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
			}
			_subscribedToEvents = true;
		}
	}

	private void UnsubscribeFromEvents()
	{
		if (_subscribedToEvents)
		{
			UIScreenController.OnApplicationResumed -= OnApplicationResumed;
			UIScreenController.OnPopupShown -= OnPopupShown;
			UIScreenController.OnPopupClosed -= OnPopupClosed;
			OnlineSettings.instance.RemoveOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
			FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive -= RefreshCurrencyEarners;
			if (InAppManager.instance != null)
			{
				InAppManager.instance.onProductRequestDone -= OnProductRequestDone;
				InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
			}
			_subscribedToEvents = false;
		}
	}

	public void RefreshCurrencyEarners()
	{
		FillTable();
	}

	private GameObject AddPrefab(GameObject prefab, ref int numberOfObjects)
	{
		GameObject gameObject = _contentTable.gameObject.AddChild(prefab);
		gameObject.name = $"{numberOfObjects:000}";
		numberOfObjects++;
		return gameObject;
	}

	private UIWidget AddSpacer(int height, ref int numberOfObjects)
	{
		UIWidget uIWidget = _contentTable.gameObject.AddWidget<UIWidget>();
		uIWidget.height = height;
		uIWidget.name = $"{numberOfObjects:000}";
		numberOfObjects++;
		return uIWidget;
	}

	private void FillTable()
	{
		List<Transform> childList = _contentTable.GetChildList();
		childList.ForEach((Transform k) =>
		{
			k.gameObject.Kill();
		});
		int numberOfObjects = 0;
		_scrollPanel.ResetAndUpdateAnchors();
		AddSpacer((!_isPopup) ? 10 : 3, ref numberOfObjects);
		GameObject gameObject = AddPrefab(_listTitleComponent, ref numberOfObjects);
		ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
		string text = Strings.Get("STORE_TITLE");
		Color midnight = ColorData.instance.midnight;
		int fontSize = ((!_isPopup) ? (-1) : 23);
		component.Setup(text, midnight, 300, fontSize);
		if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
		{
			_firstPurchaseBonusBanner = AddPrefab(_firstPurchaseBonusBannerPrefab, ref numberOfObjects).GetComponent<FirstPurchaseBonusCharacterHelper>();
			_firstPurchaseBonusBanner.Setup(new FirstPurchaseBonusCharacterHelper.FirstPurchasePayload
			{
				height = 60
			});
			if (!_firstPurchaseBonusAnimationActivated)
			{
				_firstPurchaseBonusBanner.DeactivateFirstPurchaseBonusBannerAnimation();
			}
		}
		bool flag = WelcomePackManager.IsWelcomePackActive();
		if (flag)
		{
			AddPrefab(_welcomePackBannerPrefab, ref numberOfObjects);
		}
		if (InAppManager.IsSpecialSaleActive)
		{
			GameObject gameObject2 = null;
			string[] array = null;
			Color32 color = default;
			if (InAppManager.IsWinterHolidaySpecialSaleActive)
			{
				gameObject2 = _specialSaleWinterHolidayNotificationPrefab;
				color = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_TITLE_COLOR;
				array = new string[3] { "SALE_WINTER_HEADER", "SALE_WINTER_SUBHEADER", "SALE_WINTER_OFFER_TEXT" };
			}
			else if (InAppManager.IsNewYearSpecialSaleActive)
			{
				gameObject2 = _specialSaleNewYearNotificationPrefab;
				color = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_TITLE_COLOR;
				array = new string[3] { "SALE_NEW_YEAR_HEADER", "SALE_NEW_YEAR_SUBHEADER", "SALE_NEW_YEAR_OFFER_TEXT" };
			}
			else if (InAppManager.IsBlackFridaySpecialSaleActive)
			{
				gameObject2 = _specialSaleBlackFridayNotificationPrefab;
				color = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION2_COLOR;
				array = new string[3] { "SALE_BLACK_FRIDAY_HEADER", "SALE_BLACK_FRIDAY_SUBHEADER", "SALE_BLACK_FRIDAY_OFFER_TEXT" };
			}
			if (gameObject2 != null && array.Length == 3)
			{
				GameObject gameObject3 = AddPrefab(gameObject2, ref numberOfObjects);
				NGUITools.AddWidgetCollider(gameObject3);
				SaleNotificationHelper componentInChildren = gameObject3.GetComponentInChildren<SaleNotificationHelper>();
				if (componentInChildren != null)
				{
					componentInChildren.Init(Strings.Get(array[0]), Strings.Get(array[1]), Strings.Get(array[2]), color.ToUIHex());
				}
			}
		}
		if (!flag && UserSave.lastPurchasedBundle != InAppData.BundleTier.BundleTier5)
		{
			AddPrefab(_starterPackPrefab, ref numberOfObjects);
		}
		Dictionary<string, InAppItemModel> activeInAppOffers = InAppManager.GetActiveInAppOffers();
		List<InAppItemModel> list = activeInAppOffers.Values.ToList();
		int num = 0;
		int num2 = list.Count - 1;
		while (num < list.Count)
		{
			if (list[num2].itemOfferType == InAppItemModel.OfferType.Sale || list[num2].itemOfferType == InAppItemModel.OfferType.Promotion)
			{
				InAppItemModel item = list[num2];
				list.RemoveAt(num2);
				list.Insert(0, item);
				num2++;
			}
			num++;
			num2--;
		}
		int num3 = 0;
		foreach (InAppItemModel item2 in list)
		{
			GameObject gameObject4 = AddPrefab(_coinPrefab, ref numberOfObjects);
			bool showDiscountSticker = num3 > 0 || InAppManager.IsSpecialSaleActive || !WelcomePackManager.IsWelcomePackActive();
			CoinButtonHelper component2 = gameObject4.GetComponent<CoinButtonHelper>();
			component2.Init(item2, gameObject.transform, screenName, showDiscountSticker);
			if (ShouldShowMostPopularSticker(item2))
			{
				component2.AddMostPopularSticker(_mostPopularStickerPrefab, MOST_POPULAR_STICKER_POSITION, _coinScreenPanel, 3);
			}
			NGUITools.AddWidgetCollider(gameObject4);
			num3++;
		}
		AddPrefab(_doubleCoinsPrefab, ref numberOfObjects).GetComponent<DoubleCoinUpgradeHelper>().Init();
		NGUITools.AddWidgetCollider(AddPrefab(_restorePrefab, ref numberOfObjects));
		_contentTable.Reposition();
		_shouldRefreshScreen = false;
		_wasWelcomePackActive = WelcomePackManager.IsWelcomePackActive();
		_scrollView.ResetPosition();
		coinScreenHasAppeared__BackingField.SafeInvoke();
	}

	private bool ShouldShowMostPopularSticker(InAppItemModel activeOffer)
	{
		return _mostPopularStickerProductIds.Contains(activeOffer.itemId) && activeOffer.itemOfferType == InAppItemModel.OfferType.Normal;
	}

	private void RefreshMostPopularStickerProductIds()
	{
		string[] mostPopularStickerProductIdsWithoutPrefix = GetMostPopularStickerProductIdsWithoutPrefix();
		for (int i = 0; i < mostPopularStickerProductIdsWithoutPrefix.Length; i++)
		{
			mostPopularStickerProductIdsWithoutPrefix[i] = "com.kiloo.subways." + mostPopularStickerProductIdsWithoutPrefix[i];
		}
		_mostPopularStickerProductIds = mostPopularStickerProductIdsWithoutPrefix;
	}

	private string[] GetMostPopularStickerProductIdsWithoutPrefix()
	{
		string defaultValue = StringUtility.ArrayToString(MOST_POPULAR_STICKER_DEFAULT_PRODUCT_ID_WITHOUT_PREFIX, ",");
		string value = OnlineSettings.instance.GetValue("most_popular_sticker_product_id_without_prefix", defaultValue);
		return value.Split(',');
	}

	private void OnProductRequestDone()
	{
		Show();
	}

	private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
	{
		if (_wasWelcomePackActive)
		{
			_shouldRefreshScreen = true;
		}
	}

	private void OnOnlineSettingsFinishedDownloading()
	{
		RefreshCurrencyEarners();
	}

	private void OnPopupClosed(string popupName)
	{
		if (IsPopupOnTopOfCoinScreen(popupName) && IsCoinScreenUnderPopup())
		{
			if (_firstPurchaseBonusBanner != null)
			{
				_firstPurchaseBonusBanner.ActivateFirstPurchaseBonusBannerAnimation();
				_firstPurchaseBonusAnimationActivated = true;
			}
			RefreshCurrencyEarners();
		}
	}

	private bool IsCoinScreenUnderPopup()
	{
		return UIScreenController.Instance.GetTopScreenName() == screenName || UIScreenController.Instance.GetCurrentPopupName() == screenName;
	}

	private void OnPopupShown(string popupName)
	{
		if (IsPopupOnTopOfCoinScreen(popupName) && _firstPurchaseBonusBanner != null)
		{
			_firstPurchaseBonusBanner.DeactivateFirstPurchaseBonusBannerAnimation();
			_firstPurchaseBonusAnimationActivated = false;
		}
	}

	private bool IsPopupOnTopOfCoinScreen(string popupName)
	{
		return !string.IsNullOrEmpty(popupName) && base.isActive && popupName != screenName;
	}
}
