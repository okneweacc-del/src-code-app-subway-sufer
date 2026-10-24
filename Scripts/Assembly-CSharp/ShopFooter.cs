using UnityEngine;

public class ShopFooter : MonoBehaviour
{
	[SerializeField]
	private UILabel menuButtonLabel;

	[SerializeField]
	private UILabel boostsButtonLabel;

	[SerializeField]
	private UILabel boostsCoolDealLabel;

	[SerializeField]
	private UILabel boostsCoolDealSingleLineLabel;

	[SerializeField]
	private UILabel earnCoinsButtonLabel;

	[SerializeField]
	private UILabel storeButtonLabel;

	[SerializeField]
	private UILabel storeCoolDealLabel;

	[SerializeField]
	private UILabel storeCoolDealSingleLineLabel;

	private void OnEnable()
	{
		menuButtonLabel.text = Strings.Get("FOOTER_MENU_BUTTON_TITLE");
		boostsButtonLabel.text = Strings.Get("FOOTER_BOOSTS_BUTTON_TITLE");
		boostsCoolDealSingleLineLabel.text = Strings.Get("COOL_DEAL_SINGLE_LINE");
		earnCoinsButtonLabel.text = Strings.Get("EARN_COINS_BUTTON_LABEL");
		storeButtonLabel.text = Strings.Get("STORE_TITLE");
		storeCoolDealSingleLineLabel.text = Strings.Get("COOL_DEAL_SINGLE_LINE");
	}
}
