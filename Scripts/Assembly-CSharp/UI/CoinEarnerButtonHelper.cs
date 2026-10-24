using System;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Timer;
using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI;

public class CoinEarnerButtonHelper : MonoBehaviour
{
	[SerializeField]
	private GameObject _normalContent;

	[SerializeField]
	private GameObject _connectingContent;

	[SerializeField]
	private Layout _contentLayout;

	[SerializeField]
	private Layout _connectingLayout;

	[SerializeField]
	private BoxCollider _collider;

	[SerializeField]
	private UILabel _connectingLabel;

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _priceLabel;

	[SerializeField]
	private UILabel _getLabel;

	private EarnCoinsData.EarnCurrencyProfile _earnCurrencyProfile;

	private Action _onClickDelegate;

	public void Init(int earnCurrencyProfileIndex, string title, string desc, string iconName, int price, Action onClickDelegate)
	{
		Init(EarnCoinsData.profiles[earnCurrencyProfileIndex], title, desc, iconName, price, onClickDelegate);
	}

	public void Init(EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile, string title, string desc, string iconName, int price, Action onClickDelegate)
	{
		_earnCurrencyProfile = earnCurrencyProfile;
		_title.text = title;
		_onClickDelegate = onClickDelegate;
		_icon.spriteName = iconName;
		_description.text = desc;
		if (_getLabel != null)
		{
			_getLabel.text = Strings.Get("COIN_BUTTON_EARNER_GET");
		}
		if (_connectingLabel != null)
		{
			_connectingLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
		}
		if (_priceLabel != null)
		{
			_priceLabel.text = price.ToString();
		}
		FitSpriteToWidget component = _icon.GetComponent<FitSpriteToWidget>();
		if (component != null)
		{
			component.FitNow();
			Timer.ExecuteNextFrame(component.FitNow);
		}
		GoToNormalState();
	}

	[UsedImplicitly]
	private void OnClick()
	{
		EarnCoinsData.Trigger(_earnCurrencyProfile);
		if (_onClickDelegate != null)
		{
			_onClickDelegate();
		}
	}

	private void GoToNormalState()
	{
		_normalContent.SetActive(value: true);
		_connectingContent.SetActive(value: false);
		_collider.enabled = true;
		_contentLayout.Align();
	}

	private void GoToConnectingState()
	{
		_normalContent.SetActive(value: false);
		_connectingContent.SetActive(value: true);
		_collider.enabled = false;
		_connectingLayout.Align();
	}
}
