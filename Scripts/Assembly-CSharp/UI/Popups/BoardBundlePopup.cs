using System.Collections.Generic;
using System.Linq;
using Data.Managers;
using InApp;
using InAppPurchase;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Backend.PurchaseValidation;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.Popups.BoardBundlePopupInternal;
using UI.Popups.LimitedBoardPopupInternal;
using UI.Screens;
using UnityEngine;

namespace UI.Popups;

public class BoardBundlePopup : UIPopup
{
	[Header("Anchors")]
	[SerializeField]
	private GameObject _topAnchor;

	[Header("Top")]
	[SerializeField]
	private UILabel _popupBoardTitle;

	[SerializeField]
	private UILabel _popupBoardName;

	[SerializeField]
	private TimerHelper _timerPrefab;

	[SerializeField]
	private UIWidget _timerContainer;

	[SerializeField]
	private Layout _timerLayout;

	[SerializeField]
	private UITexture _popupStickerTexture;

	[Header("Bundles")]
	[SerializeField]
	private BoardBundleItem _board;

	[SerializeField]
	private BoardBundleItem _bundle1;

	[SerializeField]
	private BoardBundleItem _bundle2;

	[Header("Sticker")]
	[SerializeField]
	private UILabel _bestStickerLabel;

	private TimerHelper _timer;

	private BoardBundlePromotion _boardBundlePromotion;

	private HoverboardThemes.ThemeIdentifier _firstCustomSet;

	private HoverboardThemes.ThemeIdentifier _secondCustomSet;

	private Boards.BoardId _boardId;

	private string _boardBundleId;

	private string _bundleTier01Id;

	private string _bundleTier02Id;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
		_boardBundlePromotion = PromotionManager.Instance.TryGetCurrentPromotion() as BoardBundlePromotion;
		if (_boardBundlePromotion == null)
		{
			Debug.LogError("Current Promotion is not a board bundle promotion.");
			return;
		}
		_bundleTier01Id = "com.kiloo.subwayboard.rioboard03.bundletier1";
		_bundleTier02Id = "com.kiloo.subwayboard.rioboard03.bundletier2";
		if (!InAppData.inAppData.TryGetValue(_bundleTier02Id, out var value))
		{
			return;
		}
		for (int i = 0; i < value.bundleContents.Count; i++)
		{
			if (value.bundleContents[i].type == InAppItemModel.ItemType.Board)
			{
				_boardId = value.bundleContents[i].boardId;
			}
			else if (value.bundleContents[i].type == InAppItemModel.ItemType.BoardAbility)
			{
				if (i == 1)
				{
					_firstCustomSet = value.bundleContents[i].themeIdentifier;
				}
				else
				{
					_secondCustomSet = value.bundleContents[i].themeIdentifier;
				}
			}
		}
		_boardBundleId = InAppData.ConvertBoardTypeToDynamicPurchaseId(_boardId);
		InitializeTimer();
		SetupTop(_boardId);
		_bestStickerLabel.text = Strings.Get("BOARD_BUNDLE_BEST_STICKER");
		SetupBundle(_board, _boardId, InAppData.inAppData[_boardBundleId], _boardBundlePromotion.BoardBundlePresentation);
		SetupBundle(_bundle1, _boardId, InAppData.inAppData[_bundleTier01Id], _boardBundlePromotion.BoardBundleTier01Presentation);
		SetupBundle(_bundle2, _boardId, InAppData.inAppData[_bundleTier02Id], _boardBundlePromotion.BoardBundleTier02Presentation);
		_bundle2.SetUpSplat(_boardBundlePromotion.BestSplatBgColor, _boardBundlePromotion.BestSplatLabelColor);
		_bundle2.ShowStars(show: false);
	}

	private void SetupBundle(BoardBundleItem boardBundleItem, Boards.BoardId boardId, InAppItemModel inAppItemModel, PromotionPresentation promotionPresentation)
	{
		List<int> allThemesForBoard = inAppItemModel.GetAllThemesForBoard(boardId);
		boardBundleItem.Init(boardId, inAppItemModel.localizedPrice);
		boardBundleItem.SetUpPresentation(promotionPresentation);
		boardBundleItem.SetupBoard(allThemesForBoard);
		for (int i = 0; i < allThemesForBoard.Count; i++)
		{
			boardBundleItem.SetupThemeInfo(allThemesForBoard[i], i);
		}
		boardBundleItem.Align();
	}

	protected override void Show()
	{
		if (_boardBundlePromotion != null)
		{
			InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
			InAppManager.instance.onPurchaseCanceled += OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseFailure += OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseItemNotAvailable += OnPurchaseFailedOrCancelled;
			_board.Show();
			_bundle1.Show();
			_bundle2.Show();
			Flurry.LogEvent("Board bundle popup shown");
		}
	}

	protected override void Hide()
	{
		base.Hide();
		if (_boardBundlePromotion != null)
		{
			InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
			InAppManager.instance.onPurchaseCanceled -= OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseFailure -= OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseItemNotAvailable -= OnPurchaseFailedOrCancelled;
		}
	}

	public override void GainFocus()
	{
		if (_boardBundlePromotion != null)
		{
			_board.GainFocus();
			_bundle1.GainFocus();
			_bundle2.GainFocus();
		}
	}

	public override void LoseFocus()
	{
		if (_boardBundlePromotion != null)
		{
			_board.LoseFocus();
			_bundle1.LoseFocus();
			_bundle2.LoseFocus();
		}
	}

	private void InitializeTimer()
	{
		_timer = Object.Instantiate(_timerPrefab, _timerContainer.transform);
		_timer.SetTimerColors(_boardBundlePromotion.TimerTextColor, _boardBundlePromotion.TimerBgColor);
		_timer.SetAlignment(HorizontalAlignment.Center);
	}

	public void OnCloseClicked()
	{
		ClosePopup();
	}

	[UsedImplicitly]
	public void OnBoardPurchaseClicked()
	{
		InAppManager.instance.StartPurchase(_boardBundleId);
	}

	[UsedImplicitly]
	public void OnBundle1PurchaseClicked()
	{
		InAppManager.instance.StartPurchase(_bundleTier01Id);
	}

	[UsedImplicitly]
	public void OnBundle2PurchaseClicked()
	{
		InAppManager.instance.StartPurchase(_bundleTier02Id);
	}

	private void OnPurchaseSuccess(string productid, bool wasrestored, IPurchaseValidationResult validationresult)
	{
		OnCloseClicked();
		Boards.BoardId boardReturned = Boards.BoardId.normal;
		bool flag = false;
		List<HoverboardThemes.ThemeIdentifier> list = new List<HoverboardThemes.ThemeIdentifier>();
		if (InAppData.GetInAppBoardBundleTiers().Contains(productid))
		{
			InAppItemModel inAppItemModel = InAppData.inAppData[productid];
			foreach (InAppItemModel.BundleItem bundleContent in inAppItemModel.bundleContents)
			{
				if (bundleContent.type == InAppItemModel.ItemType.Board)
				{
					BoardManager.ResetDisplayedCustomSets(bundleContent.boardId);
					UIModelController.Instance.SelectCurrentlyShownBoard(bundleContent.boardId);
					boardReturned = bundleContent.boardId;
					flag = true;
				}
				if (bundleContent.type == InAppItemModel.ItemType.BoardAbility)
				{
					list.Add(bundleContent.themeIdentifier);
					BoardManager.SetThemeActive(bundleContent.boardId, bundleContent.themeIdentifier, active: true);
				}
			}
			if (flag)
			{
				if (list.Contains(_firstCustomSet))
				{
					BoardManager.DisplayCustomSet(0, display: true);
				}
				if (list.Contains(_secondCustomSet))
				{
					BoardManager.DisplayCustomSet(1, display: true);
				}
				UIScreenController.Instance.ShowUnlockAnimationForHoverboardBundle(boardReturned, list);
			}
		}
		else if (InAppData.TryConvertDynamicBoardIdToBoardType(productid, out boardReturned))
		{
			BoardManager.ResetDisplayedCustomSets(boardReturned);
			UIModelController.Instance.SelectCurrentlyShownBoard(boardReturned);
			UIScreenController.Instance.ShowUnlockAnimationForHoverboard(boardReturned);
			flag = true;
		}
		if (!flag)
		{
			Debug.Log("Board type not valid for product id " + productid);
			return;
		}
		Boards.Board board = Boards.boardData[boardReturned];
		Flurry.LogEventWithAParameter("Hoverboard bought", "Id", char.ToUpper(Strings.Get(board.name)[0]) + Strings.Get(board.name).Substring(1));
		LimitedBoardPopupManager.instance.OverrideNextShowingWithBoard(_boardId);
		UIScreenController.Instance.QueuePopup("LimitedBoardPopup");
		PromotionManager.Instance.OnPromotionPurchaseSuccess();
		PromotionManager.Instance.EnableCurrentPromotion(enable: false);
	}

	private void OnPurchaseFailedOrCancelled(string id, PurchaseResult result)
	{
		PromotionManager.Instance.OnPromotionPurchaseFailed();
	}

	private void SetupTop(Boards.BoardId boardId)
	{
		Boards.Board board = Boards.boardData[boardId];
		_popupBoardTitle.text = string.Format(Strings.Get("BOARD_DISPLAY_NAME_WITH_SUFFIX"), Strings.Get(board.name));
		_popupBoardTitle.color = _boardBundlePromotion.TitleTextColor;
		_popupBoardTitle.fontSize = 23;
		_popupBoardName.text = Strings.Get(board.limitedDescription);
		_popupBoardName.color = _boardBundlePromotion.SubtitleTextColor;
		_popupBoardName.fontSize = 18;
		_popupStickerTexture.mainTexture = _boardBundlePromotion.StickerTexture;
		_timer.StartTimer(_boardBundlePromotion.TimeLeft);
		_timerLayout.Align();
	}
}
