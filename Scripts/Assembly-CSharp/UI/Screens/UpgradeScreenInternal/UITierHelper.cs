using Data.Save;
using Info;
using UnityEngine;

namespace UI.Screens.UpgradeScreenInternal;

public class UITierHelper : MonoBehaviour
{
	public UIAtlas usedAtlas;

	private UpgradeType _type;

	private int _currentTier = int.MinValue;

	public bool ResetTiers()
	{
		SetupTiers(_type);
		return InventorySave.GetTier(_type) >= Upgrades.upgrades[_type].numberOfTiers - 1;
	}

	public void SetupTiers(UpgradeType type)
	{
		_type = type;
		int numberOfTiers = Upgrades.upgrades[type].numberOfTiers;
		int tier = InventorySave.GetTier(type);
		if (tier == _currentTier)
		{
			return;
		}
		_currentTier = tier;
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
		UISprite uISprite = base.gameObject.AddSprite(usedAtlas, "progressbar_background");
		uISprite.name = "Background";
		uISprite.width = numberOfTiers * 20 - 10;
		uISprite.height = 16;
		uISprite.pivot = UIWidget.Pivot.BottomLeft;
		uISprite.cachedTransform.localPosition = Vector3.zero;
		uISprite.depth = 11;
		for (int i = 0; i < numberOfTiers - 1; i++)
		{
			UISprite uISprite2 = base.gameObject.AddSprite(usedAtlas, "progressbar_bar_off");
			uISprite2.name = "slot" + (i + 1);
			uISprite2.pivot = UIWidget.Pivot.BottomLeft;
			uISprite2.cachedTransform.localPosition = new Vector3(5f + (float)(20 * i), 3f, 0f);
			uISprite2.width = 16;
			uISprite2.height = 10;
			uISprite2.depth = 12;
			if (i < tier)
			{
				AddActiveIndicator(uISprite2);
			}
		}
	}

	private void AddActiveIndicator(UISprite parentSprite)
	{
		UISprite uISprite = parentSprite.gameObject.AddSprite(usedAtlas, "progressbar_bar_on");
		uISprite.name = "Active";
		uISprite.pivot = UIWidget.Pivot.BottomLeft;
		uISprite.transform.localPosition = new Vector3(-1f, 0f, 0f);
		uISprite.width = 18;
		uISprite.height = 9;
		uISprite.depth = 13;
	}
}
