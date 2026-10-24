using System;
using Data.Managers;
using Info;
using SYBO.Subway.Info;
using UnityEngine;

namespace SYBO.Subway.Promotion;

public class CharacterPromotion : PromotionDefinition
{
	[Header("Character")]
	[SerializeField]
	private string _characterIdStr;

	[SerializeField]
	private AnimationClip[] _characterAnimations;

	[SerializeField]
	private bool _useSpecialAssets;

	[SerializeField]
	private int _themeIndex;

	[Header("Background")]
	[SerializeField]
	private Color _backgroundColor;

	[SerializeField]
	private Color _flashColor;

	[SerializeField]
	private Color _beamColor;

	[Header("Sticker")]
	[SerializeField]
	private Vector2 _stickerPosition = new Vector2(92f, -3f);

	[SerializeField]
	private Vector2 _stickerSize = Vector2.one;

	public SYBO.Subway.Info.Characters.CharacterId CharacterId => (SYBO.Subway.Info.Characters.CharacterId)Enum.Parse(typeof(SYBO.Subway.Info.Characters.CharacterId), _characterIdStr);

	public AnimationClip[] CharacterAnimations => _characterAnimations;

	public bool UseSpecialAssets => _useSpecialAssets;

	public int ThemeIndex => _themeIndex;

	public Color BackgroundColor => _backgroundColor;

	public Color BeamColor => _beamColor;

	public Color FlashColor => _flashColor;

	public Vector2 StickerPosition => _stickerPosition;

	public Vector2 StickerSize => _stickerSize;

	public override string TitleId => SYBO.Subway.Info.Characters.characterData[CharacterId].name;

	public override string SubtitleId => SYBO.Subway.Info.Characters.characterData[CharacterId].characterLimitedDescription;

	public override string PopupId => "CharacterPromotionPopup";

	public override DateTime StartTime => SYBO.Subway.Info.Characters.characterData[CharacterId].defaultStartDate;

	public override DateTime ExpireTime => SYBO.Subway.Info.Characters.characterData[CharacterId].defaultExpirationDate;

	protected override bool IsActiveCheck => !CharacterManager.IsCharacterOwned(CharacterId);

	public override string GetPurchaseFailureFlurryEventValue()
	{
		string result = string.Empty;
		SYBO.Subway.Info.Characters.Model model = SYBO.Subway.Info.Characters.characterData[CharacterId];
		switch (model.unlockType)
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
		SYBO.Subway.Info.Characters.Model model = SYBO.Subway.Info.Characters.characterData[CharacterId];
		switch (model.unlockType)
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
		return !CharacterManager.IsCharacterOwned(CharacterId);
	}
}
