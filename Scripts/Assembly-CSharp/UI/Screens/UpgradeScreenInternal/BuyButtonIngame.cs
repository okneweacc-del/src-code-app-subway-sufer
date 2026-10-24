using Info;
using JetBrains.Annotations;
using UnityEngine;

namespace UI.Screens.UpgradeScreenInternal;

public class BuyButtonIngame : MonoBehaviour, IBuyButton
{
	public UpgradeHelper updater;

	private UpgradeType _type;

	private bool _purchaseInProgress;

	public UpgradeType type => _type;

	[UsedImplicitly]
	private void OnClick()
	{
		if (!_purchaseInProgress)
		{
			PurchaseHandler.Instance.PurchaseUpgrade(_type, this);
		}
	}

	public void InitBuyButton(UpgradeType type)
	{
		_type = type;
	}

	public void OnPurchaseSuccessful()
	{
		updater.UpgradePurchased(_type);
		_purchaseInProgress = false;
	}

	public void OnPurchaseFailure()
	{
		_purchaseInProgress = false;
	}
}
