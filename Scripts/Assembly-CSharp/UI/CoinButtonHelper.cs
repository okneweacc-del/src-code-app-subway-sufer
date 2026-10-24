using System;
using System.Collections.Generic;
using InApp;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.Helpers;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

namespace UI;

public class CoinButtonHelper : DiscountButton
{
	private Transform _onDisplayParent;

	private CoinContainer _coinContainer;

	private InAppItemModel _itemData;

	private string _screenName;

	public Action onClick;

	[UsedImplicitly]
	private void OnClick()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("productID", _itemData.itemId);
		dictionary.Add("screen", _screenName);
		Dictionary<string, string> parameters = dictionary;
		Flurry.LogEventWithSeveralParameters("A/B Product Click", parameters);
		InAppManager.instance.StartPurchase(_itemData.itemId);
		onClick.SafeInvoke();
	}

	public override bool SupportsSpecialSale()
	{
		return true;
	}

	public void Init(InAppItemModel inAppItem, Transform offerContainer, string screenName, bool showDiscountSticker = true)
	{
		_showDiscountSticker = showDiscountSticker;
		_itemData = inAppItem;
		_onDisplayParent = offerContainer;
		_screenName = screenName;
		_coinContainer = GetComponentInParent<CoinContainer>();
		Show();
	}

	public void AddMostPopularSticker(GameObject prefab, Vector2 stickerOffset, UIPanel coinScreenPanel, int padding = 0)
	{
		GameObject gameObject = content.gameObject.AddChild(prefab);
		MostPopularStickerHelper component = gameObject.GetComponent<MostPopularStickerHelper>();
		component.SetDepthFrom(coinScreenPanel.depth);
		AddSticker(component.gameObject, stickerOffset, component.height, padding);
	}

	public void AddSticker(GameObject stickerGo, Vector2 stickerOffset, int stickerHeight, int padding = 0)
	{
		SetOffsets(stickerGo, stickerOffset, stickerHeight, padding);
		_widget.height += stickerHeight + padding;
	}

	private void SetOffsets(GameObject stickerGo, Vector2 stickerOffset, int stickerHeight, int padding = 0)
	{
		stickerGo.transform.localPosition = stickerOffset;
		int num = stickerHeight / 2 + padding;
		leftContent.SetLocalPosY(leftContent.localPosition.y - (float)num);
	}

	[UsedImplicitly]
	private void ForceEndDiscount()
	{
		if (_coinContainer != null)
		{
			Show(forceEndDiscount: true);
			_coinContainer.Reset();
			_coinContainer.RepositionTableNow();
		}
	}

	public void Show(bool forceEndDiscount = false)
	{
		switch (_itemData.itemOfferType)
		{
		case InAppItemModel.OfferType.Normal:
			ShowNoDiscount(_itemData.itemId, GetCurrencyText());
			if (base.transform.parent == _onDisplayParent && _onDisplayParent != null)
			{
				base.transform.parent = _onDisplayParent.parent;
			}
			break;
		case InAppItemModel.OfferType.Sale:
			if (!forceEndDiscount)
			{
				ShowOnSale(_itemData.itemId, GetCurrencyText());
			}
			break;
		case InAppItemModel.OfferType.Promotion:
			if (!forceEndDiscount)
			{
				ShowAsPromotion();
			}
			break;
		default:
			SDebug.LogError("Item offer type not supported in CoinButtonHelper: " + _itemData.itemType);
			break;
		}
		if (_screenName == "NotEnoughCurrencyPopup")
		{
			_widget.width = 249;
		}
	}

	private string GetCurrencyText()
	{
		switch (_itemData.itemType)
		{
		case InAppItemModel.ItemType.Coins:
			return string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_COINS"), _itemData.payoutAmount);
		case InAppItemModel.ItemType.Keys:
			return string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_KEYS"), _itemData.payoutAmount);
		default:
			SDebug.LogError("Item type not supported in CoinButtonHelper: " + _itemData.itemType);
			return string.Empty;
		}
	}

	private void ShowAsPromotion()
	{
		if (_itemData.itemType == InAppItemModel.ItemType.Coins)
		{
			ShowOnPromotion(_itemData.itemId, string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_COINS"), InAppData.inAppData[_itemData.normalOfferId].payoutAmount), string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_COINS_SALE"), _itemData.payoutAmount));
		}
		else if (_itemData.itemType == InAppItemModel.ItemType.Keys)
		{
			ShowOnPromotion(_itemData.itemId, string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_KEYS"), InAppData.inAppData[_itemData.normalOfferId].payoutAmount), string.Format(Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_KEYS_SALE"), _itemData.payoutAmount));
		}
		else
		{
			SDebug.LogError("Item type not supported in CoinButtonHelper: " + _itemData.itemType);
		}
	}
}
