using UnityEngine;

public class SecretPopupTabHelper : MonoBehaviour
{
	public enum Tabs
	{
		Tab1 = 0,
		Tab2 = 1,
		Tab3 = 2,
		Tab4 = 3
	}

	[SerializeField]
	private GameObject _tab1ActiveBG;

	[SerializeField]
	private GameObject _tab1InactiveBG;

	[SerializeField]
	private UILabel _tab1Label;

	[SerializeField]
	private GameObject _tab2ActiveBG;

	[SerializeField]
	private GameObject _tab2InactiveBG;

	[SerializeField]
	private UILabel _tab2Label;

	[SerializeField]
	private GameObject _tab3ActiveBG;

	[SerializeField]
	private GameObject _tab3InactiveBG;

	[SerializeField]
	private UILabel _tab3Label;

	[SerializeField]
	private GameObject _tab4ActiveBG;

	[SerializeField]
	private GameObject _tab4InactiveBG;

	[SerializeField]
	private UILabel _tab4Label;

	private void Awake()
	{
		Tab1Tapped();
	}

	public void Tab1Tapped()
	{
		SetTabActivation(Tabs.Tab1);
	}

	public void Tab2Tapped()
	{
		SetTabActivation(Tabs.Tab2);
	}

	public void Tab3Tapped()
	{
		SetTabActivation(Tabs.Tab3);
	}

	public void Tab4Tapped()
	{
		SetTabActivation(Tabs.Tab4);
	}

	private void SetTabActivation(Tabs tab)
	{
		_tab1ActiveBG.SetActive(value: false);
		_tab1InactiveBG.SetActive(value: true);
		SetLabelActive(_tab1Label, isActive: false);
		_tab2ActiveBG.SetActive(value: false);
		_tab2InactiveBG.SetActive(value: true);
		SetLabelActive(_tab2Label, isActive: false);
		_tab3ActiveBG.SetActive(value: false);
		_tab3InactiveBG.SetActive(value: true);
		SetLabelActive(_tab3Label, isActive: false);
		_tab4ActiveBG.SetActive(value: false);
		_tab4InactiveBG.SetActive(value: true);
		SetLabelActive(_tab4Label, isActive: false);
		switch (tab)
		{
		case Tabs.Tab1:
			_tab1ActiveBG.SetActive(value: true);
			_tab1InactiveBG.SetActive(value: false);
			SetLabelActive(_tab1Label, isActive: true);
			break;
		case Tabs.Tab2:
			_tab2ActiveBG.SetActive(value: true);
			_tab2InactiveBG.SetActive(value: false);
			SetLabelActive(_tab2Label, isActive: true);
			break;
		case Tabs.Tab3:
			_tab3ActiveBG.SetActive(value: true);
			_tab3InactiveBG.SetActive(value: false);
			SetLabelActive(_tab3Label, isActive: true);
			break;
		case Tabs.Tab4:
			_tab4ActiveBG.SetActive(value: true);
			_tab4InactiveBG.SetActive(value: false);
			SetLabelActive(_tab4Label, isActive: true);
			break;
		}
	}

	private void SetLabelActive(UILabel label, bool isActive)
	{
		if (isActive)
		{
			label.color = new Color32(28, 103, 156, byte.MaxValue);
			label.effectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
		else
		{
			label.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			label.effectColor = new Color32(0, 0, 0, byte.MaxValue);
		}
	}
}
