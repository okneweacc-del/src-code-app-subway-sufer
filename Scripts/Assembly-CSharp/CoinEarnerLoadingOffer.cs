using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class CoinEarnerLoadingOffer : MonoBehaviour
{
	public enum OffersState
	{
		Loading = 0,
		NoOffersAvailable = 1,
		OfferWall = 2
	}

	[SerializeField]
	private GameObject _loadingGameObject;

	[SerializeField]
	private GameObject _noOffersAvailableGameObject;

	[SerializeField]
	private GameObject _offerWallGameObject;

	[SerializeField]
	private UISprite _fillOverlay;

	[SerializeField]
	private UISprite _backgroundFill;

	[SerializeField]
	private GameObject _coinEarnerPrefab;

	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private UILabel _getLabel;

	[SerializeField]
	private UILabel _descriptionLabel;

	[SerializeField]
	private UILabelLimiter labelLimiter;

	[SerializeField]
	private UILabel _loadingAppOffers;

	[SerializeField]
	private UILabel _couldNotLoad;

	[SerializeField]
	private UILabel _tryAgain;

	private const string BG_SPRITENAME_ACTIVE_DEFAULT = "button_fill_shopItem_special_main";

	private const string BG_SPRITENAME_OVERLAY_DEFAULT = "button_fill_shopItem_special_highlight";

	private const string BG_SPRITENAME_INACTIVE = "button_generic_1";

	private OffersState _currentState;

	private const int LOADING_TIME_LIMIT = 10;

	private bool _isLodingTimerStarted;

	private Action _onClickHandler;

	private int _indexOnScreen;

	public void Init(Action onClickDelegate)
	{
		if (onClickDelegate != null)
		{
			_onClickHandler = (Action)Delegate.Combine(_onClickHandler, onClickDelegate);
		}
		if (labelLimiter != null)
		{
			labelLimiter.LimitLabel();
		}
		_titleLabel.text = Strings.Get("TAPJOY_OFFER_TITLE");
		_descriptionLabel.text = Strings.Get("TAPJOY_OFFER_DESCRIPTION");
	}

	private void Awake()
	{
		SetOffersState(OffersState.Loading);
		CheckIfNullAndSetTextWhereIsTheCase();
	}

	private void OnEnable()
	{
		OfferWallManager.instance.onOfferWallReceived += OnOfferWallReceived;
	}

	private void OnOfferWallReceived()
	{
		SetOffersState(OffersState.OfferWall);
	}

	private void OnDisable()
	{
		OfferWallManager.instance.onOfferWallReceived -= OnOfferWallReceived;
	}

	public void SetOffersState(OffersState state)
	{
		_currentState = state;
		switch (state)
		{
		case OffersState.Loading:
			SetStateGameObjectActive(_loadingGameObject);
			break;
		case OffersState.NoOffersAvailable:
			SetStateGameObjectActive(_noOffersAvailableGameObject);
			break;
		case OffersState.OfferWall:
			SetStateGameObjectActive(_offerWallGameObject);
			break;
		default:
			SetStateGameObjectActive(_loadingGameObject);
			break;
		}
	}

	private void UpdateNoOfferState()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			_couldNotLoad.text = Strings.Get("CLOUDSAVE_STATUS_OFFLINE_INFO");
		}
		else
		{
			_couldNotLoad.text = Strings.Get("EARN_COINS_LOADING_FAILED");
		}
	}

	public OffersState GetOffersState()
	{
		return _currentState;
	}

	private void SetStateGameObjectActive(GameObject go)
	{
		if (_loadingGameObject != null)
		{
			_loadingGameObject.SetActive(value: false);
		}
		else
		{
			LogError("Loading Game object not connected in CoinEarnerLoadingOffer");
		}
		if (_noOffersAvailableGameObject != null)
		{
			_noOffersAvailableGameObject.SetActive(value: false);
		}
		else
		{
			LogError("No Offers available Game object not connected in CoinEarnerLoadingOffer");
		}
		if (_offerWallGameObject != null)
		{
			_offerWallGameObject.SetActive(value: false);
		}
		else
		{
			LogError("Offers Game object not connected in CoinEarnerLoadingOffer");
		}
		if (!(go != null))
		{
			return;
		}
		if (go == _noOffersAvailableGameObject || go == _offerWallGameObject)
		{
			_backgroundFill.spriteName = "button_fill_shopItem_special_main";
			_backgroundFill.color = GlobalColors.COLOR_WHITE;
			_fillOverlay.spriteName = "button_fill_shopItem_special_highlight";
			_fillOverlay.gameObject.SetActive(value: true);
			_coinEarnerPrefab.GetComponent<Collider>().enabled = true;
			StopCoroutine("StartLoadingCoroutine");
			_isLodingTimerStarted = false;
			UpdateNoOfferState();
		}
		else if (go == _loadingGameObject)
		{
			_fillOverlay.gameObject.SetActive(value: false);
			_coinEarnerPrefab.GetComponent<Collider>().enabled = false;
			_backgroundFill.spriteName = "button_generic_1";
			_backgroundFill.color = GlobalColors.PRIMARY_ACTION_COLOR_UNAVAILABLE;
			if (!_isLodingTimerStarted)
			{
				StartCoroutine("StartLoadingCoroutine");
			}
		}
		go.SetActive(value: true);
	}

	[UsedImplicitly]
	private void OnClick()
	{
		if (_noOffersAvailableGameObject.activeInHierarchy)
		{
			SetOffersState(OffersState.Loading);
			OfferWallManager.instance.RequestOfferWall();
		}
		if (_offerWallGameObject.activeInHierarchy)
		{
			OfferWallManager.instance.ShowOfferWall("Earn Coins Screen");
			if (_onClickHandler != null)
			{
				_onClickHandler();
			}
		}
	}

	private IEnumerator StartLoadingCoroutine()
	{
		_isLodingTimerStarted = true;
		yield return new WaitForSeconds(10f);
		_isLodingTimerStarted = false;
		SetOffersState(OffersState.NoOffersAvailable);
	}

	private void CheckIfNullAndSetTextWhereIsTheCase()
	{
		if (_loadingAppOffers == null)
		{
			LogError("_loadingAppOffers not set in the Coin earner Offer Button Helper prefab");
		}
		else
		{
			_loadingAppOffers.text = Strings.Get("OFFERWALL_BUTTON_LOADING");
		}
		if (_couldNotLoad == null)
		{
			LogError("_couldNotLoad not set in the Coin earner Offer Button Helper prefab");
		}
		else
		{
			_couldNotLoad.text = Strings.Get("OFFERWALL_BUTTON_COULD_NOT_LOAD");
		}
		if (_tryAgain == null)
		{
			LogError("_couldNotLoad not set in the Coin earner Offer Button Helper prefab");
		}
		else
		{
			_tryAgain.text = Strings.Get("BUTTON_TRY_AGAIN");
		}
		if (_titleLabel == null)
		{
			LogError("Title Label not set in the Coin earner Offer Button Helper prefab");
		}
		if (_getLabel == null)
		{
			LogError("Get Label not set in the Coin earner Offer Button Helper prefab");
		}
		else
		{
			_getLabel.text = Strings.Get("COIN_BUTTON_EARNER_GET");
		}
		if (_descriptionLabel == null)
		{
			LogError("Description label not set in the Coin earner Offer Button Helper prefab");
		}
	}

	private void LogError(string message)
	{
		SDebug.LogError(message);
	}
}
