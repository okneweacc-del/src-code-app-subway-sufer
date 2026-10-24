using InApp;
using UI.Generic;
using UnityEngine;

namespace UI;

public class DiscountButton : MonoBehaviour
{
	private enum DiscountButtonState
	{
		Normal = 0,
		Sale = 1,
		Promotion = 2
	}

	protected const int SALE_BUTTON_GROWTH = 12;

	protected const int LINETHROUGH_WIDTH_OFFSET = 6;

	protected static Vector2 SHADOW_OFFSET = Vector2.one;

	protected static Vector3 PRICE_DISCOUNT_OFFSET = new Vector3(0f, 6f, 0f);

	protected Color32 titleColor = GlobalColorsGenerated.DISCOUNT_BUTTON_TITLE_COLOR;

	protected Color32 titleColorShadow = GlobalColorsGenerated.DISCOUNT_BUTTON_TITLE_COLOR_SHADOW;

	protected Color32 description1ColorInactive = GlobalColorsGenerated.DISCOUNT_BUTTON_DESCRIPTION1_INACTIVE_COLOR;

	protected Color32 description1ColorShadowInactive = GlobalColorsGenerated.DISCOUNT_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR;

	protected Color32 description1ColorActive = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION1_ACTIVE_COLOR;

	protected Color32 description1ColorShadowActive = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR;

	protected Color32 description2Color = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION2_COLOR;

	protected Color32 description2ColorShadow = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION2_SHADOW_COLOR;

	protected Color32 priceColor = GlobalColorsGenerated.DISCOUNT_BUTTON_PRICE_COLOR;

	protected Color32 priceColorShadow = GlobalColorsGenerated.DISCOUNT_BUTTON_PRICE_SHADOW_COLOR;

	protected Color32 limitedTimerColor = GlobalColorsGenerated.DISCOUNT_BUTTON_LIMITED_TIME_COLOR;

	protected Color32 limitedTimerColorShadow = GlobalColorsGenerated.DISCOUNT_BUTTON_LIMITED_TIME_SHADOW_COLOR;

	protected Color32 stickerColor = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;

	protected Color32 stickerLabelColor = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;

	protected Color32 lineThroughColor = GlobalColors.DISCOUNT_BUTTON_LINE_THROUGH_COLOR;

	protected Color32 lineThroughShadowColor = GlobalColors.DISCOUNT_BUTTON_LINE_THROUGH_SHADOW_COLOR;

	protected const string SPRITE_NAME_ICON_BACKGROUND_NORMAL = "icon_background_normal";

	protected const string SPRITE_NAME_ICON_BACKGROUND_KEYS = "icon_background_keys";

	protected const string SPRITE_NAME_ICON_BACKGROUND_THEME = "icon_background_theme";

	protected string SPRITE_NAME_FILL_SHOPITEM_HIGHLIGHT_SALE = "button_fill_shopItem_highlight_sale";

	protected string SPRITE_NAME_FILL_SHOPITEM_MAIN_SALE = "button_fill_shopItem_main_sale";

	protected const string SPRITE_NAME_SALE_STRIKEOVER_HIGH = "sale_strikeover_hi";

	protected const string SPRITE_NAME_SALE_STRIKEOVER_LOW = "sale_strikeover_lo";

	protected const string ONLINE_SETTINGS_KEY_DISCOUNT_DEAL_NAME = "discount_deal_name";

	protected string iconBackgroundNormalSpriteName = "icon_background_normal";

	protected string iconBackgroundKeysSpriteName = "icon_background_keys";

	protected Vector3 originalMainTextPosition;

	protected Vector3 originalDescriptionPosition;

	protected Vector3 originalPricePosition;

	protected Color32 originalPriceColor;

	protected Color32 originalPriceColorShadow;

	protected Vector3 originalFillGrayScale;

	protected Vector3 originalFillColorScale;

	protected Vector3 originalOutlineScale;

	protected Vector3 originalContentPosition;

	protected string originalFillColorSpirteName;

	protected string originalFillGraySpriteName;

	protected Color32 originalTitleColor;

	protected Color32 originalTitleColorShadow;

	protected Vector2 originalTitleDistanceShadow;

	protected Color32 originalDescription1Color;

	protected Color32 originalDescription1ColorShadow;

	protected Color32 originalDescription2Color;

	protected Color32 originalDescription2ColorShadow;

	protected Vector2 originalIconSize;

	protected Vector2 originalIconBGSize;

	protected Vector3 originalIconBGPosition;

	protected Vector3 originalWidgetScale;

	[SerializeField]
	protected UISprite iconBG;

	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected UILabel title;

	[SerializeField]
	protected UILabel description1;

	[SerializeField]
	protected UILabel description2;

	[SerializeField]
	protected UISprite lineThrough;

	[SerializeField]
	protected UISprite lineThroughShadow;

	[SerializeField]
	protected UILabel price;

	[SerializeField]
	protected UISprite fillGraySprite;

	[SerializeField]
	protected UISprite fillColorSprite;

	[SerializeField]
	protected UISprite outlineSprite;

	[SerializeField]
	protected Transform content;

	[SerializeField]
	protected Transform leftContent;

	[SerializeField]
	protected Transform rightContent;

	[SerializeField]
	protected Transform mainTextContent;

	[SerializeField]
	protected UISprite discountSticker;

	[SerializeField]
	protected UILabel discountLabel;

	[SerializeField]
	protected UILabel discountLabelSingleLine;

	[SerializeField]
	protected UILabel limitedTitle;

	[SerializeField]
	protected UILabel limitedTime;

	[SerializeField]
	protected UILabel discountLabelSpecialSale;

	protected UIWidget _widget;

	protected bool _showDiscountSticker;

	public virtual void Awake()
	{
		if (SupportsSpecialSale() && InAppManager.IsSpecialSaleActive)
		{
			if (InAppManager.IsWinterHolidaySpecialSaleActive)
			{
				titleColor = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_TITLE_COLOR;
				titleColorShadow = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_TITLE_COLOR_SHADOW;
				description1ColorInactive = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_INACTIVE_COLOR;
				description1ColorShadowInactive = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR;
				priceColor = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_PRICE_COLOR;
				priceColorShadow = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_PRICE_SHADOW_COLOR;
				limitedTimerColor = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LIMITED_TIME_COLOR;
				limitedTimerColorShadow = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LIMITED_TIME_SHADOW_COLOR;
				stickerColor = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_COLOR;
				stickerLabelColor = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_LABEL_COLOR;
				description1ColorActive = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_ACTIVE_COLOR;
				description1ColorShadowActive = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR;
				description2Color = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION2_COLOR;
				description2ColorShadow = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION2_SHADOW_COLOR;
				lineThroughColor = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LINE_THROUGH_COLOR;
				lineThroughShadowColor = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LINE_THROUGH_SHADOW_COLOR;
				SPRITE_NAME_FILL_SHOPITEM_HIGHLIGHT_SALE = "button_fill_shopItem_holiday_highlight_sale";
				SPRITE_NAME_FILL_SHOPITEM_MAIN_SALE = "button_fill_shopItem_holiday_main_sale";
				iconBackgroundNormalSpriteName = "icon_background_keys";
				iconBackgroundKeysSpriteName = "icon_background_keys";
			}
			else if (InAppManager.IsNewYearSpecialSaleActive)
			{
				titleColor = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_TITLE_COLOR;
				titleColorShadow = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_TITLE_COLOR_SHADOW;
				description1ColorInactive = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_INACTIVE_COLOR;
				description1ColorShadowInactive = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR;
				priceColor = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_PRICE_COLOR;
				priceColorShadow = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_PRICE_SHADOW_COLOR;
				limitedTimerColor = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LIMITED_TIME_COLOR;
				limitedTimerColorShadow = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LIMITED_TIME_SHADOW_COLOR;
				stickerColor = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_STICKER_COLOR;
				stickerLabelColor = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_STICKER_LABEL_COLOR;
				description1ColorActive = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_ACTIVE_COLOR;
				description1ColorShadowActive = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR;
				description2Color = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION2_COLOR;
				description2ColorShadow = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION2_SHADOW_COLOR;
				lineThroughColor = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LINE_THROUGH_COLOR;
				lineThroughShadowColor = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LINE_THROUGH_SHADOW_COLOR;
				SPRITE_NAME_FILL_SHOPITEM_HIGHLIGHT_SALE = "button_fill_shopItem_newyear_highlight_sale";
				SPRITE_NAME_FILL_SHOPITEM_MAIN_SALE = "button_fill_shopItem_newyear_main_sale";
				iconBackgroundNormalSpriteName = "icon_background_violet";
				iconBackgroundKeysSpriteName = "icon_background_violet";
			}
			else if (InAppManager.IsBlackFridaySpecialSaleActive)
			{
				titleColor = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_TITLE_COLOR;
				titleColorShadow = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_TITLE_COLOR_SHADOW;
				description1ColorInactive = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_INACTIVE_COLOR;
				description1ColorShadowInactive = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR;
				priceColor = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_PRICE_COLOR;
				priceColorShadow = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_PRICE_SHADOW_COLOR;
				limitedTimerColor = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LIMITED_TIME_COLOR;
				limitedTimerColorShadow = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LIMITED_TIME_SHADOW_COLOR;
				stickerColor = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_COLOR;
				stickerLabelColor = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_LABEL_COLOR;
				description1ColorActive = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_ACTIVE_COLOR;
				description1ColorShadowActive = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR;
				description2Color = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION2_COLOR;
				description2ColorShadow = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION2_SHADOW_COLOR;
				lineThroughColor = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LINE_THROUGH_COLOR;
				lineThroughShadowColor = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LINE_THROUGH_SHADOW_COLOR;
				SPRITE_NAME_FILL_SHOPITEM_HIGHLIGHT_SALE = "button_fill_shopItem_black_friday_highlight_sale";
				SPRITE_NAME_FILL_SHOPITEM_MAIN_SALE = "button_fill_shopItem_black_friday_main_sale";
				iconBackgroundNormalSpriteName = "icon_background_normal";
				iconBackgroundKeysSpriteName = "icon_background_normal";
			}
		}
		_widget = GetComponent<UIWidget>();
		if (_widget != null)
		{
			originalWidgetScale = new Vector3(_widget.width, _widget.height, 1f);
		}
		originalMainTextPosition = mainTextContent.localPosition;
		originalFillColorSpirteName = fillColorSprite.spriteName;
		originalFillGraySpriteName = fillGraySprite.spriteName;
		originalFillGrayScale = new Vector3(fillGraySprite.width, fillGraySprite.height, 1f);
		originalFillColorScale = new Vector3(fillColorSprite.width, fillColorSprite.height, 1f);
		originalOutlineScale = new Vector3(outlineSprite.width, outlineSprite.height, 1f);
		originalContentPosition = content.localPosition;
		originalDescriptionPosition = description1.cachedTransform.localPosition;
		originalPricePosition = price.cachedTransform.localPosition;
		originalPriceColor = price.color;
		originalPriceColorShadow = price.effectColor;
		originalTitleColor = title.color;
		originalTitleColorShadow = title.effectColor;
		originalTitleDistanceShadow = title.effectDistance;
		originalDescription1Color = description1.color;
		originalDescription1ColorShadow = description1.effectColor;
		originalDescription2Color = description2.color;
		originalDescription2ColorShadow = description2.effectColor;
		originalIconSize = new Vector2(icon.width, icon.height);
		originalIconBGSize = new Vector2(iconBG.width, iconBG.height);
		originalIconBGPosition = iconBG.transform.localPosition;
		limitedTitle.text = Strings.Get("DISCOUNT_BUTTON_LIMITED_TITLE");
	}

	public virtual bool SupportsSpecialSale()
	{
		return false;
	}

	protected virtual void ShowOnPromotion(string productString, string backupDescription, string backupDescription2)
	{
		DiscountButtonState discountButtonState = DiscountButtonState.Promotion;
		PositionContent(discountButtonState);
		SetLimitedLabels(discountButtonState);
		SetDiscountAssets(discountButtonState, productString);
		SetDiscountColors(discountButtonState);
		SetProductData(discountButtonState, productString, backupDescription, backupDescription2);
		FormatButton(discountButtonState);
	}

	protected virtual void ShowOnSale(string productString, string backupDescription = "")
	{
		DiscountButtonState discountButtonState = DiscountButtonState.Sale;
		PositionContent(discountButtonState);
		SetLimitedLabels(discountButtonState);
		SetDiscountAssets(discountButtonState, productString);
		SetDiscountColors(discountButtonState);
		SetProductData(discountButtonState, productString, backupDescription, string.Empty);
		FormatButton(discountButtonState);
	}

	protected virtual void ShowNoDiscount(string productString, string backupDescription = "")
	{
		DiscountButtonState discountButtonState = DiscountButtonState.Normal;
		PositionContent(discountButtonState);
		SetLimitedLabels(discountButtonState);
		SetDiscountAssets(discountButtonState, productString);
		SetNodiscountColors();
		SetProductData(discountButtonState, productString, backupDescription, string.Empty);
		FormatButton(discountButtonState);
	}

	private void SetProductData(DiscountButtonState state, string productString, string backupDescription, string backupDescription2 = "")
	{
		bool flag = state == DiscountButtonState.Normal;
		price.text = InAppData.inAppData[productString].localizedPrice;
		icon.spriteName = InAppData.inAppData[productString].icon;
		if (flag)
		{
			title.text = Strings.Get(InAppData.inAppData[productString].displayName);
			if (InAppData.inAppData[productString].itemType == InAppItemModel.ItemType.Coins || InAppData.inAppData[productString].itemType == InAppItemModel.ItemType.DoubleCoins)
			{
				iconBG.spriteName = "icon_background_normal";
			}
			else
			{
				iconBG.spriteName = "icon_background_keys";
			}
		}
		else
		{
			title.text = Strings.Get(InAppData.inAppData[productString].displayName);
			if (InAppManager.IsSpecialSaleActive)
			{
				if (InAppData.inAppData[productString].itemType == InAppItemModel.ItemType.Coins)
				{
					iconBG.spriteName = iconBackgroundNormalSpriteName;
				}
				else
				{
					iconBG.spriteName = iconBackgroundKeysSpriteName;
				}
			}
			else
			{
				title.text = Strings.GetLocalizedOnlineSettings("discount_deal_name", Strings.Get("DISCOUNT_BUTTON_COOL_SPACIAL_DEAL"));
				iconBG.spriteName = "icon_background_theme";
			}
		}
		description2.enabled = !flag;
		string text = Strings.Get(InAppData.inAppData[productString].displayText);
		bool flag2 = state == DiscountButtonState.Promotion;
		if (lineThrough != null && lineThroughShadow != null)
		{
			lineThrough.enabled = flag2;
			lineThroughShadow.enabled = flag2;
		}
		switch (state)
		{
		case DiscountButtonState.Normal:
			description1.text = ((!string.IsNullOrEmpty(text)) ? text : backupDescription);
			description2.text = string.Empty;
			description1.color = Color.white;
			break;
		case DiscountButtonState.Sale:
			description1.text = Strings.Get(InAppData.inAppData[productString].displayName);
			description2.text = ((!string.IsNullOrEmpty(text)) ? text : backupDescription);
			break;
		case DiscountButtonState.Promotion:
			description1.text = backupDescription;
			description2.text = backupDescription2;
			if (lineThrough != null && lineThroughShadow != null)
			{
				lineThrough.color = lineThroughColor;
				lineThroughShadow.color = lineThroughShadowColor;
				string spriteName = ((!DeviceInfo.Instance.isHighres) ? "sale_strikeover_lo" : "sale_strikeover_hi");
				lineThrough.spriteName = spriteName;
				lineThroughShadow.spriteName = spriteName;
			}
			break;
		}
	}

	private void FormatButton(DiscountButtonState state)
	{
		bool flag = state == DiscountButtonState.Normal;
		int num = ((!flag) ? 12 : 0);
		if (_widget != null)
		{
			_widget.height = Mathf.RoundToInt(originalWidgetScale.y + (float)num);
			outlineSprite.UpdateAnchors();
			fillColorSprite.UpdateAnchors();
			fillGraySprite.UpdateAnchors();
		}
		fillColorSprite.spriteName = ((!flag) ? SPRITE_NAME_FILL_SHOPITEM_HIGHLIGHT_SALE : originalFillColorSpirteName);
		fillGraySprite.spriteName = ((!flag) ? SPRITE_NAME_FILL_SHOPITEM_MAIN_SALE : originalFillGraySpriteName);
		Vector2 vector = originalIconBGSize;
		iconBG.width = Mathf.RoundToInt(vector.x);
		iconBG.height = Mathf.RoundToInt(vector.y);
		FitSpriteToWidget component = icon.GetComponent<FitSpriteToWidget>();
		if (component != null)
		{
			component.fittingMode = FitSpriteToWidget.FittingMode.PixelPerfect;
			component.FitNow();
		}
	}

	private void PositionContent(DiscountButtonState state)
	{
		Vector3 vector = ((state != DiscountButtonState.Normal) ? new Vector3(0f, 6f, 0f) : Vector3.zero);
		content.localPosition = originalContentPosition + vector;
	}

	private void SetLimitedLabels(DiscountButtonState state)
	{
		bool flag = state == DiscountButtonState.Normal;
		limitedTitle.gameObject.SetActive(!flag);
		limitedTime.gameObject.SetActive(!flag);
		limitedTitle.enabled = !flag;
		limitedTime.enabled = !flag;
		if (limitedTime.enabled && UIScreenController.Instance.GetCurrentPopupName() == "NotEnoughCurrencyPopup")
		{
			limitedTime.GetComponent<UIWidget>().SetDimensions(65, 9);
		}
		price.transform.localPosition = (flag ? new Vector3(97f, -1.2f, 0f) : new Vector3(97f, 2.5f, 0f));
	}

	private void SetDiscountAssets(DiscountButtonState state, string productString)
	{
		bool flag = state == DiscountButtonState.Normal;
		discountSticker.gameObject.SetActive(!flag && _showDiscountSticker);
		discountSticker.enabled = !flag && _showDiscountSticker;
		if ((bool)discountLabelSpecialSale && InAppManager.IsSpecialSaleActive && state == DiscountButtonState.Promotion)
		{
			discountLabelSpecialSale.gameObject.SetActive(value: true);
			discountLabelSpecialSale.color = stickerLabelColor;
			return;
		}
		if ((bool)discountLabelSpecialSale)
		{
			discountLabelSpecialSale.gameObject.SetActive(value: false);
		}
		discountLabel.enabled = !flag;
		discountLabel.gameObject.SetActive(!flag);
		discountLabelSingleLine.enabled = !flag;
		discountLabelSingleLine.gameObject.SetActive(!flag);
		switch (state)
		{
		case DiscountButtonState.Promotion:
			discountLabelSingleLine.text = Strings.Get("COOL_DEAL_SINGLE_LINE");
			break;
		case DiscountButtonState.Sale:
		{
			int num = Mathf.Abs(InAppManager.GetDiscountValue(InAppData.GetInAppOnlineSettingsId(InAppData.inAppData[productString].normalOfferId), 0));
			discountLabel.text = string.Format(Strings.Get("DISCOUNT_BUTTON_COOL_OFF"), num);
			break;
		}
		case DiscountButtonState.Normal:
			break;
		}
	}

	private void SetNodiscountColors()
	{
		title.color = ColorData.instance.sun;
		title.effectColor = ColorData.instance.shadow;
		title.effectDistance = originalTitleDistanceShadow;
		description1.color = originalDescription1Color;
		description1.effectColor = ColorData.instance.shadow;
		description2.color = originalDescription2Color;
		description2.effectColor = originalDescription2ColorShadow;
		price.color = originalPriceColor;
		price.effectColor = originalPriceColorShadow;
	}

	private void SetDiscountColors(DiscountButtonState state)
	{
		title.color = titleColor;
		title.effectColor = titleColorShadow;
		title.effectDistance = SHADOW_OFFSET;
		description2.color = description2Color;
		description2.effectColor = description2ColorShadow;
		price.color = priceColor;
		price.effectColor = priceColorShadow;
		limitedTitle.color = limitedTimerColor;
		limitedTitle.effectColor = limitedTimerColorShadow;
		limitedTime.color = limitedTimerColor;
		limitedTime.effectColor = limitedTimerColorShadow;
		discountSticker.color = stickerColor;
		discountLabel.color = stickerLabelColor;
		if (state == DiscountButtonState.Sale)
		{
			description1.color = description1ColorActive;
			description1.effectColor = description1ColorShadowActive;
		}
		if (state == DiscountButtonState.Promotion)
		{
			description1.color = description1ColorInactive;
			description1.effectColor = description1ColorShadowInactive;
			discountLabelSingleLine.color = stickerLabelColor;
		}
	}
}
