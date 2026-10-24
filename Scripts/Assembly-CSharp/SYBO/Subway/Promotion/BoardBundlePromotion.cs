using System;
using System.Globalization;
using Data.Managers;
using Info;
using UnityEngine;

namespace SYBO.Subway.Promotion;

public class BoardBundlePromotion : PromotionDefinition
{
	private const string ONLINE_SETTINGS_KEY_ENABLE_BOARD_BUNDLE_SALE = "active_board_bundle_sale";

	private const string BOARD_BUNDLE_SALE_DISABLED_VALUE = "off";

	private static readonly CultureInfo ONLINESETTINGS_DATETIME_CULTUREINFO = new CultureInfo("da-DK");

	[SerializeField]
	private Boards.BoardId _boardId = Boards.BoardId.rioBoard03;

	[SerializeField]
	private string _bundleTier01 = "rioboard03.bundletier1";

	[SerializeField]
	private string _bundleTier02 = "rioboard03.bundletier2";

	[Header("Bundle Background Presentation")]
	[SerializeField]
	private PromotionPresentation _boardBundlePresentation;

	[SerializeField]
	private PromotionPresentation _boardBundleTier01Presentation;

	[SerializeField]
	private PromotionPresentation _boardBundleTier02Presentation;

	[Header("Best Label")]
	[SerializeField]
	private Color _bestSplatBgColor;

	[SerializeField]
	private Color _bestSplatLabelColor;

	public Boards.BoardId BoardId => _boardId;

	public string BundleTier01 => _bundleTier01;

	public string BundleTier02 => _bundleTier02;

	public PromotionPresentation BoardBundlePresentation => _boardBundlePresentation;

	public PromotionPresentation BoardBundleTier01Presentation => _boardBundleTier01Presentation;

	public PromotionPresentation BoardBundleTier02Presentation => _boardBundleTier02Presentation;

	public Color BestSplatBgColor => _bestSplatBgColor;

	public Color BestSplatLabelColor => _bestSplatLabelColor;

	public override DateTime StartTime => BoardStartAvailabilityDate(_boardId);

	public override DateTime ExpireTime => BoardExpirationDate(_boardId);

	protected override bool IsActiveCheck => !BoardManager.IsBoardOwned(_boardId) && !BoardManager.IsBoardDisabled(_boardId) && !IsOnlineSettingsDisabled();

	public override string TitleId => Boards.boardData[BoardId].name;

	public override string SubtitleId => Boards.boardData[BoardId].limitedDescription;

	public override string PopupId => "BoardBundlePopup";

	private DateTime BoardStartAvailabilityDate(Boards.BoardId boardId)
	{
		string key = "hoverboard_starttime_" + boardId;
		DateTime result = DateTime.MinValue;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			result = ((!(Boards.boardData[boardId].defaultStartDate != DateTime.MinValue)) ? DateTime.MinValue : Boards.boardData[boardId].defaultStartDate);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
		{
			try
			{
				result = DateTime.Parse(valueString, ONLINESETTINGS_DATETIME_CULTUREINFO);
			}
			catch (FormatException)
			{
			}
		}
		return result;
	}

	public DateTime BoardExpirationDate(Boards.BoardId boardId)
	{
		string key = "hoverboard_expiretime_" + boardId;
		DateTime result = DateTime.MinValue;
		string valueString;
		if (!OnlineSettings.instance.HasValue(key))
		{
			result = DefaultExpirationDateForHoverboard(boardId);
		}
		else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
		{
			try
			{
				result = DateTime.Parse(valueString, ONLINESETTINGS_DATETIME_CULTUREINFO);
			}
			catch (FormatException)
			{
				result = DefaultExpirationDateForHoverboard(boardId);
			}
		}
		return result;
	}

	private DateTime DefaultExpirationDateForHoverboard(Boards.BoardId bId)
	{
		if (Boards.boardData[bId].defaultExpirationTime != DateTime.MinValue)
		{
			return Boards.boardData[bId].defaultExpirationTime;
		}
		return DateTime.MaxValue;
	}

	private bool IsOnlineSettingsDisabled()
	{
		if (OnlineSettings.instance.TryGetValue("active_board_bundle_sale", out var valueString) && valueString == "off")
		{
			return true;
		}
		return false;
	}

	public override string GetPurchaseFailureFlurryEventValue()
	{
		string result = string.Empty;
		Boards.Board board = Boards.boardData[_boardId];
		switch (board.unlockType)
		{
		case PurchaseData.UnlockType.Coins:
			result = "not enough coins";
			break;
		case PurchaseData.UnlockType.Keys:
			result = "not enough keys";
			break;
		case PurchaseData.UnlockType.InApp:
			result = "bought using iap fail";
			break;
		}
		return result;
	}

	public override string GetPurchaseSuccessFlurryEventValue()
	{
		string result = string.Empty;
		Boards.Board board = Boards.boardData[_boardId];
		switch (board.unlockType)
		{
		case PurchaseData.UnlockType.Coins:
			result = "bought using coins";
			break;
		case PurchaseData.UnlockType.Keys:
			result = "bought using keys";
			break;
		case PurchaseData.UnlockType.InApp:
			result = "bought using iap";
			break;
		}
		return result;
	}

	public override bool ShouldTrackPopupCloseFlurryEvent()
	{
		return !BoardManager.IsBoardOwned(_boardId);
	}
}
