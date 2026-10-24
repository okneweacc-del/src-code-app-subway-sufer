using System.Collections.Generic;
using Data.Managers;
using InApp;
using InAppPurchase;
using Info;
using Kiloo.Common.Backend.PurchaseValidation;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UI.Screens;
using UnityEngine;

namespace UI.Helpers.WelcomePack;

public class WelcomePackBannerHelper : MonoBehaviour
{
	[Header("MainCell")]
	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _boardName;

	[SerializeField]
	private DefaultRenderTextureSetup _mainCellRenderTextureSetup;

	[Header("Cell 1")]
	[SerializeField]
	private UILabel _cell1Label;

	[Header("Cell 2")]
	[SerializeField]
	private UILabel _cell2Label;

	[Header("Cell 3")]
	[SerializeField]
	private UILabel _cell3Label;

	[Header("BuyButton")]
	[SerializeField]
	private UILabel _buyButtonLabel;

	[Header("Timer")]
	[SerializeField]
	private TimerHelper _timerPrefab;

	[SerializeField]
	private UIWidget _timerContainer;

	private string _lastScreenNameWhenPressingBuy;

	private TimerHelper _timer;

	private InAppItemModel _inAppPack;

	private bool _isSetup;

	private void OnEnable()
	{
		if (WelcomePackManager.IsWelcomePackActive())
		{
			LoadInAppPackIfNotLoaded();
			Setup();
		}
		if (InAppManager.isInitialized)
		{
			InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
		}
	}

	private void LoadInAppPackIfNotLoaded()
	{
		if (_inAppPack == null && !string.IsNullOrEmpty(WelcomePackManager.welcomePackInAppID))
		{
			_inAppPack = InAppData.inAppData[WelcomePackManager.welcomePackInAppID];
		}
	}

	private void Setup()
	{
		SetupModels();
		SetupTimer();
		SetupLabels(_inAppPack.bundleContents);
		_isSetup = true;
	}

	private void SetupModels()
	{
		_mainCellRenderTextureSetup.Setup(new CameraSettings
		{
			backgroundColor = new Color(255f, 255f, 255f, 0f)
		});
	}

	private void SetupTimer()
	{
		if (_timer == null)
		{
			_timer = Object.Instantiate(_timerPrefab, _timerContainer.transform);
			_timer.SetTimerColors(GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT, GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG);
			_timer.SetAlignment(HorizontalAlignment.Center);
			_timer.transform.localPosition = Vector3.zero;
			_timer.StartTimer(WelcomePackManager.welcomePackTimeLeft);
		}
	}

	private void SetupLabels(List<InAppItemModel.BundleItem> content)
	{
		if (_title != null)
		{
			_title.text = Strings.Get("WELCOME_PACK_TITLE");
		}
		_boardName.text = Strings.Get(Boards.boardData[content[0].boardId].name);
		_cell1Label.text = content[1].amount.ToString();
		_cell2Label.text = "x" + content[2].amount;
		_cell3Label.text = "x" + content[3].amount;
		_buyButtonLabel.text = InAppData.inAppData[WelcomePackManager.welcomePackInAppID].localizedPrice;
	}

	private void OnDisable()
	{
		if (InAppManager.isInitialized)
		{
			InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
		}
		if (_isSetup)
		{
			DeactivateCells();
			_isSetup = false;
		}
	}

	private void DeactivateCells()
	{
		_mainCellRenderTextureSetup.ReleaseRenderTexture();
	}

	public void OnPurchaseClick()
	{
		InAppManager.instance.onItemRestoreFinished += OnItemRestoreFinished;
		InAppManager.instance.RestoreNonConsumablePurchases(InAppData.GetInAppSpecialBundles());
		UIScreenController instance = UIScreenController.Instance;
		if (instance.isShowingPopup)
		{
			OnPurchaseClickInPopup(instance);
		}
		else
		{
			_lastScreenNameWhenPressingBuy = instance.GetTopScreenName();
		}
	}

	private void OnItemRestoreFinished(RestoreFinishedResult result)
	{
		InAppManager.instance.onItemRestoreFinished -= OnItemRestoreFinished;
		if (!BoardManager.IsBoardOwned(Boards.BoardId.starterPackBoard01))
		{
			InAppManager.instance.StartPurchase(WelcomePackManager.welcomePackInAppID);
		}
	}

	private void OnPurchaseClickInPopup(UIScreenController uiScreenController)
	{
		if (StoreScreen.openedFromWelcomePackGetNowClick)
		{
			_lastScreenNameWhenPressingBuy = "BoardScreen";
		}
		else
		{
			_lastScreenNameWhenPressingBuy = uiScreenController.GetCurrentPopupName();
		}
	}

	private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
	{
		if ((productId == "com.kiloo.subwaysurfers.welcomepacktier01" || productId == "com.kiloo.subwaysurfers.welcomepacktier02") && !wasRestored)
		{
			UIScreenController.Instance.ShowRewardAnimationForBundlePurchase(_inAppPack.bundleContents, productId);
			if (validationResult != null && validationResult.receiptStatus == ReceiptStatus.Valid)
			{
				Flurry.LogEventWithAParameter("welcome_pack_purchased", "source_of_purchase", _lastScreenNameWhenPressingBuy);
			}
		}
	}
}
