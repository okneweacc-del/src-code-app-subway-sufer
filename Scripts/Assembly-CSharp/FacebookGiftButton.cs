using System;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Timer;
using UnityEngine;

public class FacebookGiftButton : MonoBehaviour
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private UILabel _getLabel;

	[SerializeField]
	private GameObject _breadCrumb;

	private int earnCurrencyProfileIndex;

	private Action _onClickDelegate;

	private bool _didEnableBreadCrumb;

	public void Init(int earnCurrencyProfileIndex, string earnerTitle, string earnerDesc, Action onClickDelegate, UIScrollView _panel)
	{
		this.earnCurrencyProfileIndex = earnCurrencyProfileIndex;
		title.text = earnerTitle;
		description.text = earnerDesc;
		_onClickDelegate = onClickDelegate;
		_getLabel.text = Strings.Get("COIN_BUTTON_EARNER_GET");
		_breadCrumb.SetActive(value: false);
		_didEnableBreadCrumb = false;
		base.gameObject.GetComponent<UIDragScrollView>().scrollView = _panel;
	}

	[UsedImplicitly]
	private void OnClick()
	{
		EarnCoinsData.Trigger(earnCurrencyProfileIndex);
		if (_onClickDelegate != null)
		{
			_onClickDelegate();
		}
	}

	public void LateUpdate()
	{
		if (!_didEnableBreadCrumb)
		{
			_didEnableBreadCrumb = true;
			Timer.ExecuteNextFrame(() =>
			{
				_breadCrumb.SetActive(value: true);
			});
		}
	}
}
