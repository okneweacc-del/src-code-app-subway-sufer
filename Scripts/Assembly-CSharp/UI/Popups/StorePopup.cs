using UI.Screens;

namespace UI.Popups;

public class StorePopup : StoreScreen
{
	protected override float screenOffset => -13f;

	public void ClosePopup()
	{
		UIScreenController.Instance.ClosePopup();
	}

	public override void BackButtonPressed()
	{
		UIScreenController.Instance.ClosePopup();
	}

	public override string GetFlurryName()
	{
		return "CoinsUI_quick";
	}

	protected override void HandleSafeArea()
	{
	}
}
