using System;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using Info;
using JetBrains.Annotations;
using UnityEngine;

namespace UI;

public class FacebookRewardsButton : MonoBehaviour
{
	[SerializeField]
	private GameObject activeContent;

	[SerializeField]
	private GameObject connectingContent;

	[SerializeField]
	private UILabel _connectingLabel;

	[SerializeField]
	private UIAnchor contentAnchor;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _priceLabel;

	[SerializeField]
	private UILabel _getLabel;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action onFacebookClick__BackingField;

	public event Action onFacebookClick
	{
		add
		{
			Action action = onFacebookClick__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFacebookClick__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onFacebookClick__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFacebookClick__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void OnEnable()
	{
		if (UserSave.hasPaidOutFacebookReward)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		InitializeButton();
		GoToActiveState();
	}

	private void InitializeButton()
	{
		titleLabel.text = Strings.Get("FACEBOOK_LOGIN");
		_description.text = Strings.Get("FACEBOOK_EARN_COINS_DESCRIPTION");
		_priceLabel.text = FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Coins].ToString();
		_connectingLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
		if (_getLabel != null)
		{
			_getLabel.text = Strings.Get("COIN_BUTTON_EARNER_GET");
		}
		if (UserSave.hasPaidOutFacebookReward)
		{
			contentAnchor.pixelOffset = new Vector2(34f, -2f);
		}
		else
		{
			contentAnchor.pixelOffset = new Vector2(64f, -2f);
		}
	}

	[UsedImplicitly]
	private void OnClick()
	{
		GoToConnectingState();
		if (onFacebookClick__BackingField != null)
		{
			onFacebookClick__BackingField();
		}
	}

	public void GoToActiveState()
	{
		activeContent.SetActive(value: true);
		connectingContent.SetActive(value: false);
		GetComponent<BoxCollider>().enabled = true;
	}

	public void GoToConnectingState()
	{
		activeContent.SetActive(value: false);
		connectingContent.SetActive(value: true);
		GetComponent<BoxCollider>().enabled = false;
	}
}
