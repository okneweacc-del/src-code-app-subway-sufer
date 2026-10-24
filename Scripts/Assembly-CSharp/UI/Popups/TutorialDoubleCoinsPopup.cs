using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Popups;

public class TutorialDoubleCoinsPopup : CharacterPopup
{
	[SerializeField]
	private GameObject _centerAnchor;

	[SerializeField]
	private UILabel _calloutText;

	[SerializeField]
	private GameObject _tutorialMultiLinesPrefab;

	[SerializeField]
	private GameObject _doubleCoinsPrefab;

	[SerializeField]
	private GameObject _doubleCoinsAnchor;

	private TutorialMultilineLabelsHelper _tutorialLinesHelper;

	private GameObject _doubleCoins;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
		_tutorialLinesHelper = TutorialMultilineLabelsHelper.AddToGameObject(_centerAnchor, _tutorialMultiLinesPrefab);
		_doubleCoins = _doubleCoinsAnchor.AddChild(_doubleCoinsPrefab);
		_doubleCoins.transform.SetLocalPosY(0f);
		_doubleCoins.GetComponent<DoubleCoinUpgradeHelper>().Init();
		NGUITools.AddWidgetCollider(_doubleCoins);
	}

	protected override void Show()
	{
		SetCharacter(Characters.CharacterId.frizzy, new Vector2(39f, -12.5f), null, 19f, posing: false, null, useSpecialAsset: false);
		_calloutText.text = Strings.Get("POPUP_TUTORIAL_DOUBLE_COINS_CALLOUT");
		_tutorialLinesHelper.SetText("POPUP_TUTORIAL_DOUBLE_COINS_LINE1_BIG", "POPUP_TUTORIAL_DOUBLE_COINS_LINE1_SMALL", "POPUP_TUTORIAL_DOUBLE_COINS_LINE2_BIG", "POPUP_TUTORIAL_DOUBLE_COINS_LINE2_SMALL", "POPUP_TUTORIAL_DOUBLE_COINS_LINE3_BIG", "POPUP_TUTORIAL_DOUBLE_COINS_LINE3_SMALL");
		UIScreenController.OnPopupShown += OnNewPopupShown;
		UIScreenController.OnPopupClosed += OnNewPopupClosed;
	}

	private void OnNewPopupShown(string popupName)
	{
		if (popupName != "TutorialDoubleCoinsPopup")
		{
			HideCharacter();
		}
	}

	private void OnNewPopupClosed(string popupName)
	{
		ShowCharacter();
	}

	protected override void Hide()
	{
		UIScreenController.OnPopupShown -= OnNewPopupShown;
		UIScreenController.OnPopupClosed -= OnNewPopupClosed;
	}
}
