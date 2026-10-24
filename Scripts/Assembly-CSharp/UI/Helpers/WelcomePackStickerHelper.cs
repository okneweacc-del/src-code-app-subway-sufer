using UnityEngine;

namespace UI.Helpers;

public class WelcomePackStickerHelper : MonoBehaviour
{
	private const int PROCENT_OFF = 70;

	[SerializeField]
	private UISprite _stickerBackground;

	[SerializeField]
	private UILabel _stickerLabel;

	private void OnEnable()
	{
		SetupSticker();
	}

	private void SetupSticker()
	{
		_stickerBackground.color = GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG;
		_stickerLabel.color = GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT;
		_stickerLabel.text = string.Format(Strings.Get("DISCOUNT_BUTTON_COOL_OFF"), 70);
	}
}
