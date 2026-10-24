namespace UI.Screens;

public class UpgradePopup : UpgradeScreen
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
		return "UpgradesUI_quick";
	}

	protected override void HandleSafeArea()
	{
	}
}
