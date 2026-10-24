using InApp;
using JetBrains.Annotations;
using UnityEngine;

public class RestorePurchaseHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[UsedImplicitly]
	private void OnClick()
	{
		InAppManager.instance.RestoreNonConsumablePurchases(InAppData.GetAllProductIds());
	}

	private void Awake()
	{
		titleLabel.text = Strings.Get("RESTORE_BUTTON_TITLE");
		descriptionLabel.text = Strings.Get("RESTORE_BUTTON_DESCRIPTION");
	}
}
