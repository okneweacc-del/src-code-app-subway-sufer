using UI.Popups;
using UnityEngine;

namespace Info;

public class WelcomePopupTheme : ScriptableObject
{
	[SerializeField]
	private Color32[] _backupColors;

	private static WelcomePopupTheme _instance;

	public Color32[] colors;

	public Color32[] backupColors => _backupColors;

	public static WelcomePopupTheme instance => _instance ?? (_instance = Resources.Load<WelcomePopupTheme>("Info/WelcomePopupTheme"));

	private WelcomePopupTheme()
	{
		_backupColors = new Color32[2];
		colors = new Color32[2];
	}

	public void Reset()
	{
		ref Color32 reference = ref colors[0];
		reference = backupColors[0];
		ref Color32 reference2 = ref colors[1];
		reference2 = backupColors[1];
	}

	public void UpdateBackupColors()
	{
		ref Color32 reference = ref _backupColors[0];
		reference = colors[0];
		ref Color32 reference2 = ref _backupColors[1];
		reference2 = colors[1];
	}

	public void RefreshWelcomePopup()
	{
		if (!(UIScreenController.Instance == null))
		{
			WelcomePopup welcomePopup = (WelcomePopup)UIScreenController.Instance.GetScreenFromCache("WelcomePopup");
			if (welcomePopup != null)
			{
				welcomePopup.UpdateColors();
			}
		}
	}
}
