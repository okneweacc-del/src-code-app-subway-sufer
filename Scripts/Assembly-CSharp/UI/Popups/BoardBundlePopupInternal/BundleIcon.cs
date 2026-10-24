using System;
using Info;
using UnityEngine;

namespace UI.Popups.BoardBundlePopupInternal;

[Serializable]
internal class BundleIcon
{
	[SerializeField]
	private UISprite _bundleIconBackground;

	[SerializeField]
	private UISprite _bundleIcon;

	[SerializeField]
	private UILabel _bundleIconLabel;

	public void SetupTheme(Boards.BoardId boardId, int theme)
	{
		HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[boardId][theme - 1];
		_bundleIconBackground.color = hoverboardTheme.buttonBgColor;
		_bundleIcon.color = hoverboardTheme.buttonIconColor;
		_bundleIcon.spriteName = hoverboardTheme.iconName;
		Vector3 localScale = _bundleIcon.transform.localScale;
		_bundleIcon.MakePixelPerfect();
		_bundleIcon.transform.localScale = localScale;
		_bundleIconLabel.text = Strings.Get(hoverboardTheme.title);
	}

	public void SetupDefaultTheme(Boards.BoardId boardId)
	{
		Boards.Board board = Boards.boardData[boardId];
		_bundleIconBackground.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_FILL;
		_bundleIcon.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_ICON;
		Vector3 localScale = _bundleIcon.transform.localScale;
		_bundleIcon.MakePixelPerfect();
		_bundleIcon.transform.localScale = localScale;
		if (_bundleIconLabel != null)
		{
			_bundleIconLabel.text = string.Format(Strings.Get("BOARD_DISPLAY_NAME_WITH_SUFFIX"), Strings.Get(board.name));
		}
	}
}
