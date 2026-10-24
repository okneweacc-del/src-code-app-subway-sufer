using System.Collections.Generic;
using Data.Save;
using Info;
using JetBrains.Annotations;
using UI.Generic.Layout;
using UI.RenderTexture;
using UI.Screens.RewardManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Popups;

public class TokenMysteryBoxPopup : UIPopup
{
	[Header("Prefabs")]
	[SerializeField]
	private GameObject _tokenMysteryBoxPrefab;

	[SerializeField]
	private GameObject _tokenSpinnerPrefab;

	[Header("Labels")]
	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private UILabel _descriptionLabel;

	[SerializeField]
	private UILabel _freeStickerLabel;

	[FormerlySerializedAs("_buttonlabel")]
	[SerializeField]
	private UILabel _buttonLabel;

	[Header("Sprites")]
	[SerializeField]
	private UISprite _freeSticker;

	[SerializeField]
	private UISprite _buttonKeyIcon;

	[SerializeField]
	private UISprite _bg;

	[SerializeField]
	private List<UISprite> _stripes;

	[Header("Layouts")]
	[SerializeField]
	private HorizontalLayout _buttonLayout;

	[Header("other refs")]
	[SerializeField]
	private UIWidget _buttonLabelSpacer;

	[SerializeField]
	private DefaultRenderTextureSetup _tokenBoxRenderSetup;

	private GameObject _tokenMysteryBox;

	private Transform _tokenIcons;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
		base.gameObject.BroadcastMessage("ResetAndUpdateAnchors");
		SetupTexts();
		SetupColors();
		RefreshLayout();
	}

	protected override void Show()
	{
		SetupRenderTexture();
		RefreshLayout();
		Flurry.LogEventWithAParameter("Character token pop-up", "BoxCost", (UserSave.hasUserRecievedFreeTokenMysteryBox ? Upgrades.upgrades[UpgradeType.tokenmysterybox].GetPrice(0) : 0).ToString());
	}

	protected override void Hide()
	{
		_tokenBoxRenderSetup.ReleaseRenderTexture();
	}

	public override void GainFocus()
	{
		_freeSticker.gameObject.SetActive(value: false);
		RefreshLayout();
	}

	public override void LoseFocus()
	{
		_freeSticker.gameObject.SetActive(value: false);
	}

	[UsedImplicitly]
	public void OnPurchaseButtonClicked()
	{
		PurchaseHandler.Instance.PurchaseUpgrade(UpgradeType.tokenmysterybox, () =>
		{
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.ClosePopup();
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.TokenMysteryBox, ObtainableItemSource.IngameCurrencyPurchase);
			UIScreenController.Instance.PushCelebrationPopup();
			Flurry.LogEventWithAParameter("Character Token Box Opened", "ScreenName", screenName);
		}, null);
	}

	private void SetupRenderTexture()
	{
		_tokenBoxRenderSetup.Setup(new CameraSettings
		{
			backgroundColor = new Color(255f, 255f, 255f, 0f)
		});
	}

	private void SetupTexts()
	{
		_titleLabel.text = Strings.Get("TOKEN_MYSTERY_BOX_NAME");
		_descriptionLabel.text = Strings.Get("TOKEN_MYSTERY_BOX_DESCRIPTION");
		_descriptionLabel.effectStyle = UILabel.Effect.Shadow;
		_descriptionLabel.effectDistance = new Vector2(1f, 1f);
		_freeStickerLabel.text = Strings.Get("FREE");
		_buttonLabel.text = Strings.Get("TOKEN_MYSTERY_BOX_OPEN_BOX");
		_buttonLabel.effectStyle = UILabel.Effect.Shadow;
		_buttonLabel.effectDistance = new Vector2(1f, 1f);
	}

	private void SetupColors()
	{
		_bg.applyGradient = true;
		_bg.gradientTop = ColorData.instance.teal;
		_bg.gradientBottom = ColorData.instance.turquoise;
		_titleLabel.color = ColorData.instance.midnight;
		_descriptionLabel.color = ColorData.instance.pit;
		_descriptionLabel.effectColor = ColorData.instance.turquoise;
		_freeSticker.color = ColorData.instance.grape;
		_freeStickerLabel.color = ColorData.instance.grass;
		_buttonLabel.color = ColorData.instance.white;
		_buttonLabel.effectColor = ColorData.instance.shadow;
	}

	private void RefreshLayout()
	{
		if (!(_buttonKeyIcon == null) && !(_buttonLabelSpacer == null) && !(_buttonLabel == null) && !(_freeSticker == null) && !(_buttonLayout == null))
		{
			if (UserSave.hasUserRecievedFreeTokenMysteryBox)
			{
				_buttonKeyIcon.gameObject.SetActive(value: true);
				_buttonLabelSpacer.gameObject.SetActive(value: true);
				_buttonLabel.text = Upgrades.upgrades[UpgradeType.tokenmysterybox].pricesRaw[0].ToString();
				_freeSticker.gameObject.SetActive(value: false);
			}
			else
			{
				_buttonKeyIcon.gameObject.SetActive(value: false);
				_buttonLabelSpacer.gameObject.SetActive(value: false);
				_buttonLabel.text = Strings.Get("TOKEN_MYSTERY_BOX_OPEN_BOX");
				_freeSticker.gameObject.SetActive(value: true);
			}
			_buttonLayout.Align();
		}
	}
}
