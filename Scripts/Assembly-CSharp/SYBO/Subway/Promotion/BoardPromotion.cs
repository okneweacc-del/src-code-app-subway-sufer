using System;
using Data.Managers;
using Info;
using UnityEngine;

namespace SYBO.Subway.Promotion;

public class BoardPromotion : PromotionDefinition
{
	[SerializeField]
	private Boards.BoardId _boardId = Boards.BoardId.hongKongBoard01;

	[Header("Background")]
	[SerializeField]
	private Color _backgroundColor;

	[SerializeField]
	private Color _flashColor;

	[SerializeField]
	private Color _beamColor;

	public Boards.BoardId BoardId => _boardId;

	public Color BackgroundColor => _backgroundColor;

	public Color BeamColor => _beamColor;

	public Color FlashColor => _flashColor;

	public override DateTime StartTime => Boards.boardData[_boardId].defaultStartDate;

	public override DateTime ExpireTime => Boards.boardData[_boardId].defaultExpirationTime;

	public override string TitleId => Boards.boardData[BoardId].name;

	public override string SubtitleId => Boards.boardData[BoardId].limitedDescription;

	public override string PopupId => "BoardPromotionPopup";

	protected override bool IsActiveCheck => !BoardManager.IsBoardOwned(_boardId);

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
