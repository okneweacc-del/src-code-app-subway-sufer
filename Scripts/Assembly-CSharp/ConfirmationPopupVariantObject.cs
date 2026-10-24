using System;

public struct ConfirmationPopupVariantObject(string titleText, Func<string> getDescriptionText, string greenButtonText, string redButtonText, Action greenButtonAction, Action redButtonAction)
{
	public string titleText = titleText;

	public Func<string> getDescriptionText = getDescriptionText;

	public string greenButtonText = greenButtonText;

	public string redButtonText = redButtonText;

	public Action greenButtonAction = greenButtonAction;

	public Action redButtonAction = redButtonAction;
}
