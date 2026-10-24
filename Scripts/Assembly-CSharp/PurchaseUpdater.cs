using Data.Save;
using Info;
using UI.Screens.UpgradeScreenInternal;
using UnityEngine;

public class PurchaseUpdater : MonoBehaviour
{
	public UILabel price;

	public UISprite coin;

	public UITierHelper tierHelper;

	public UILabel haveAmount;

	public GameObject buyButton;

	public UILabel buyButtonTitle;

	public UISprite buyButtonBackground;

	public void UpgradePurchased(UpgradeType type)
	{
		switch (type)
		{
		case UpgradeType.hoverboard:
		case UpgradeType.headstart2000:
			if (haveAmount != null)
			{
				haveAmount.text = string.Format(Strings.Get("UPGRADES_HELPER_YOU_HAVE"), InventorySave.GetAmount(type));
			}
			break;
		case UpgradeType.jetpack:
		case UpgradeType.supersneakers:
		case UpgradeType.coinmagnet:
		case UpgradeType.letters:
		case UpgradeType.doubleMultiplier:
			if (tierHelper.ResetTiers())
			{
				NGUITools.Destroy(coin);
				price.text = string.Empty;
				buyButtonTitle.text = "Max";
				Object.Destroy(buyButton.GetComponent<BoxCollider>());
			}
			else
			{
				price.text = string.Empty + Upgrades.upgrades[type].GetPrice(InventorySave.GetTier(type) + 1);
			}
			break;
		case (UpgradeType)2:
		case UpgradeType.mysterybox:
		case UpgradeType.tokenmysterybox:
			break;
		}
	}
}
