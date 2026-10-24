using System.Diagnostics;
using Data.Save;
using InApp;
using JetBrains.Annotations;
using Kiloo.Common.Backend.PurchaseValidation;
using UI.Screens;
using UnityEngine;

namespace UI;

public class DoubleCoinUpgradeHelper : DiscountButton
{
	private Vector3 _shownDiscStickerPos = new Vector3(126f, 28f, 0f);

	private Vector3 _shownDiscLabelPos = new Vector3(125f, 28.5f, 0f);

	private Vector3 _shownDiscLabelSingleLinePos = new Vector3(127f, 29f, 0f);

	private string doubleCoinVersion;

	private bool _inited;

	public bool OnSale
	{
		get
		{
			if (InAppManager.IsDoubleCoinsDiscountActive)
			{
				int discountValue = InAppManager.GetDiscountValue(InAppData.GetInAppOnlineSettingsId("com.kiloo.subways.doublecoins"), 0);
				if (discountValue < 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	[UsedImplicitly]
	private void ForceEndDiscount()
	{
		_Setup(forceEndDiscount: true);
		UpgradeScreen upgradeScreen = NGUITools.FindInParents<UpgradeScreen>(base.transform.parent.gameObject);
		if (upgradeScreen != null)
		{
			upgradeScreen.SaleEnded();
		}
	}

	private void _DefaultSetup(string data, bool wasRestored, IPurchaseValidationResult validationResult)
	{
		_Setup();
	}

	private void _Setup(bool forceEndDiscount = false)
	{
		if (UserSave.hasDoubleCoinsUpgrade)
		{
			doubleCoinVersion = "com.kiloo.subways.doublecoins";
			ShowNoDiscount(doubleCoinVersion, string.Empty);
			fillColorSprite.spriteName = "button_check_grey";
			fillColorSprite.pivot = UIWidget.Pivot.Center;
			fillColorSprite.type = UIBasicSprite.Type.Simple;
			fillColorSprite.MakePixelPerfect();
			fillColorSprite.cachedTransform.localPosition = new Vector3(97f, 2f, 0f);
			price.enabled = false;
			GetComponent<BoxCollider>().enabled = false;
		}
		else if (OnSale && !forceEndDiscount)
		{
			doubleCoinVersion = "com.kiloo.subways.doublecoinsdiscount";
			ShowOnSale("com.kiloo.subways.doublecoinsdiscount", string.Empty);
		}
		else
		{
			doubleCoinVersion = "com.kiloo.subways.doublecoins";
			ShowNoDiscount("com.kiloo.subways.doublecoins", string.Empty);
			if (base.transform.parent.name == "000")
			{
				base.transform.parent = base.transform.parent.parent;
			}
		}
	}

	public void Init()
	{
		_inited = true;
		_Setup();
	}

	private void OnEnable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onPurchaseSuccess += _DefaultSetup;
		}
		if (_inited)
		{
			_Setup();
		}
	}

	private void OnDisable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onPurchaseSuccess -= _DefaultSetup;
		}
	}

	private void UpdatePrice()
	{
		price.text = InAppData.inAppData[doubleCoinVersion].localizedPrice;
	}

	private void OnClick()
	{
		if (!UserSave.hasDoubleCoinsUpgrade)
		{
			InAppManager.instance.StartPurchase(doubleCoinVersion);
		}
	}

	protected override void ShowOnPromotion(string productString, string sendDescription, string sendDescription2)
	{
		base.ShowOnPromotion(productString, sendDescription, sendDescription2);
		EnableDiscount();
	}

	protected override void ShowOnSale(string productString, string sendDescription = "")
	{
		base.ShowOnSale(productString, sendDescription);
		EnableDiscount();
	}

	protected override void ShowNoDiscount(string productString, string backupDescription = "")
	{
		base.ShowNoDiscount(productString, backupDescription);
		DisableDiscount();
	}

	private void EnableDiscount()
	{
		discountSticker.cachedTransform.localPosition = _shownDiscStickerPos;
		discountLabel.cachedTransform.localPosition = _shownDiscLabelPos;
		discountLabelSingleLine.cachedTransform.localPosition = _shownDiscLabelSingleLinePos;
	}

	private void DisableDiscount()
	{
		discountSticker.cachedTransform.localPosition = Vector3.zero;
		discountLabel.cachedTransform.localPosition = new Vector3(0f, 0f, -1f);
		discountLabelSingleLine.cachedTransform.localPosition = new Vector3(0f, 0f, -1f);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, Object context = null)
	{
		SDebug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, Object context = null)
	{
	}

	[Conditional("ENABLE_DOUBLECOINS_LOGS")]
	public static void Log(string msg, Object context = null)
	{
	}
}
