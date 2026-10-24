using InApp;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Timer;
using UnityEngine;

namespace UI.Popups;

public class WelcomePackPopup : UIPopup
{
	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _subtitle;

	[SerializeField]
	private GameObject _hoverBoardRenderObject;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
	}

	protected override void Show()
	{
		_title.text = Strings.Get("WELCOME_PACK_TITLE");
		_title.color = ColorData.instance.midnight;
		_subtitle.text = Strings.Get("LIMITED_TIME_OFFER");
		_subtitle.color = ColorData.instance.peacock;
		if (!WelcomePackManager.IsWelcomePackActive())
		{
			Timer.ExecuteNextFrame(ClosePopup);
		}
	}

	private void OnEnable()
	{
		if (InAppManager.isInitialized)
		{
			InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
		}
	}

	private void OnDisable()
	{
		if (InAppManager.isInitialized)
		{
			InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
		}
	}

	public override void GainFocus()
	{
		if (_hoverBoardRenderObject != null)
		{
			_hoverBoardRenderObject.layer = Layers.Instance._2DOVERLAYCLIP;
		}
	}

	public override void LoseFocus()
	{
		if (_hoverBoardRenderObject != null)
		{
			_hoverBoardRenderObject.layer = Layers.Instance._2DGUI;
		}
	}

	public void OnCloseButtonClicked()
	{
		Flurry.LogEventWithAParameter("welcome_pack_purchased", "source_of_purchase", "Closed");
		ClosePopup();
	}

	private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
	{
		if (productId == "com.kiloo.subwaysurfers.welcomepacktier01" || productId == "com.kiloo.subwaysurfers.welcomepacktier02")
		{
			Flurry.LogEventWithAParameter("welcome_pack_purchased", "source_of_purchase", "Purchased");
			ClosePopup();
		}
	}
}
