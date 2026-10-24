using System.Linq;
using Data.Save;
using InApp;
using InAppPurchase;
using Kiloo.Common.Backend.PurchaseValidation;
using UnityEngine;

public class BundlePurchaseButton : MonoBehaviour
{
	[SerializeField]
	private GameObject activeButtonContents;

	[SerializeField]
	private GameObject inactiveButtonContents;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UISprite bundle1Icon;

	[SerializeField]
	private UISprite bundle2Icon;

	[SerializeField]
	private UISprite bundle3Icon;

	[SerializeField]
	private UISprite bundle4Icon;

	[SerializeField]
	private UISprite bundle5Icon;

	[SerializeField]
	private BundleContentSlot coinsSlot;

	[SerializeField]
	private BundleContentSlot hoverboardsSlot;

	[SerializeField]
	private BundleContentSlot keysSlot;

	[SerializeField]
	private BundleContentSlot headstartSlot;

	[SerializeField]
	private BundleContentSlot scoreboosterSlot;

	private InAppData.BundleTier _lastPurchasedBundle;

	private void OnEnable()
	{
		InitializeButton();
		UpdateButton();
	}

	private void OnClick()
	{
		InAppManager.instance.StartPurchase(InAppData.GetBundleId(GetBundleToSell()));
		UpdateButton();
	}

	private void InitializeButton()
	{
		InAppManager.instance.onProductRequestDone += OnProductRequestDone;
		InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
		InAppManager.instance.onPurchaseFailure += OnPurchaseFailure;
	}

	private void OnDisable()
	{
		InAppManager instance = InAppManager.instance;
		if (instance != null)
		{
			instance.onProductRequestDone -= OnProductRequestDone;
			instance.onPurchaseSuccess -= OnPurchaseSuccess;
			instance.onPurchaseFailure -= OnPurchaseFailure;
		}
	}

	private void UpdatePrice()
	{
		_lastPurchasedBundle = UserSave.lastPurchasedBundle;
		priceLabel.text = InAppData.inAppData[InAppData.GetBundleId(GetBundleToSell())].localizedPrice;
	}

	private void OnProductRequestDone()
	{
		UpdatePrice();
	}

	private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
	{
		if (InAppData.GetInAppBundleTiers().Contains(productId))
		{
			UIScreenController.Instance.ShowRewardAnimationForBundlePurchase(InAppData.inAppData[productId].bundleContents, productId);
			UpdateButton();
		}
	}

	private void OnPurchaseFailure(string data, PurchaseResult ressult)
	{
		UpdateButton();
	}

	private InAppData.BundleTier GetBundleToSell()
	{
		switch (_lastPurchasedBundle)
		{
		case InAppData.BundleTier.None:
			return InAppData.BundleTier.BundleTier1;
		case InAppData.BundleTier.BundleTier1:
			return InAppData.BundleTier.BundleTier2;
		case InAppData.BundleTier.BundleTier2:
			return InAppData.BundleTier.BundleTier3;
		case InAppData.BundleTier.BundleTier3:
			return InAppData.BundleTier.BundleTier4;
		case InAppData.BundleTier.BundleTier4:
			return InAppData.BundleTier.BundleTier5;
		case InAppData.BundleTier.BundleTier5:
			SDebug.LogError("There are no more bundles to sell");
			return InAppData.BundleTier.None;
		default:
			SDebug.LogError("Unidetified bundle tier: " + _lastPurchasedBundle);
			return InAppData.BundleTier.None;
		}
	}

	private void UpdateButton()
	{
		_lastPurchasedBundle = UserSave.lastPurchasedBundle;
		if (_lastPurchasedBundle != InAppData.BundleTier.BundleTier5)
		{
			ShowActiveContents(active: true);
			InAppItemModel inAppItemModel = InAppData.inAppData[InAppData.GetBundleId(GetBundleToSell())];
			titleLabel.text = Strings.Get(inAppItemModel.displayName);
			ShowBundleIcon();
			UpdatePrice();
			InAppItemModel.BundleItem bundleItem = inAppItemModel.bundleContents.Find((InAppItemModel.BundleItem x) => x.type == InAppItemModel.ItemType.Coins);
			if (bundleItem != null && bundleItem.amount > 0)
			{
				coinsSlot.gameObject.SetActive(value: true);
				coinsSlot.UpdateAmount(GetFormattedAmount(bundleItem.amount));
			}
			else
			{
				coinsSlot.gameObject.SetActive(value: false);
			}
			InAppItemModel.BundleItem bundleItem2 = inAppItemModel.bundleContents.Find((InAppItemModel.BundleItem x) => x.type == InAppItemModel.ItemType.Hoverboards);
			if (bundleItem2 != null && bundleItem2.amount > 0)
			{
				hoverboardsSlot.gameObject.SetActive(value: true);
				hoverboardsSlot.UpdateAmount(GetFormattedAmount(bundleItem2.amount));
			}
			else
			{
				hoverboardsSlot.gameObject.SetActive(value: false);
			}
			InAppItemModel.BundleItem bundleItem3 = inAppItemModel.bundleContents.Find((InAppItemModel.BundleItem x) => x.type == InAppItemModel.ItemType.Keys);
			if (bundleItem3 != null && bundleItem3.amount > 0)
			{
				keysSlot.gameObject.SetActive(value: true);
				keysSlot.UpdateAmount(GetFormattedAmount(bundleItem3.amount));
			}
			else
			{
				keysSlot.gameObject.SetActive(value: false);
			}
			InAppItemModel.BundleItem bundleItem4 = inAppItemModel.bundleContents.Find((InAppItemModel.BundleItem x) => x.type == InAppItemModel.ItemType.Headstarts);
			if (bundleItem4 != null && bundleItem4.amount > 0)
			{
				headstartSlot.gameObject.SetActive(value: true);
				headstartSlot.UpdateAmount(GetFormattedAmount(bundleItem4.amount));
			}
			else
			{
				headstartSlot.gameObject.SetActive(value: false);
			}
			InAppItemModel.BundleItem bundleItem5 = inAppItemModel.bundleContents.Find((InAppItemModel.BundleItem x) => x.type == InAppItemModel.ItemType.Scoreboosters);
			if (bundleItem5 != null && bundleItem5.amount > 0)
			{
				scoreboosterSlot.gameObject.SetActive(value: true);
				scoreboosterSlot.UpdateAmount(GetFormattedAmount(bundleItem5.amount));
			}
			else
			{
				scoreboosterSlot.gameObject.SetActive(value: false);
			}
		}
		else
		{
			ShowActiveContents(active: false);
		}
	}

	private void ShowActiveContents(bool active)
	{
		activeButtonContents.SetActive(active);
		inactiveButtonContents.SetActive(!active);
		GetComponent<Collider>().enabled = active;
	}

	private void ShowBundleIcon()
	{
		bundle1Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.None);
		bundle2Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier1);
		bundle3Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier2);
		bundle4Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier3);
		bundle5Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier4);
	}

	private string GetFormattedAmount(int amount)
	{
		if (amount >= 10000)
		{
			return amount / 1000 + "K";
		}
		return amount.ToString();
	}
}
