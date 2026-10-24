using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Managers;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.FreeStuff;
using UI.SlideIns;
using UnityEngine;
using Utilities;

namespace UI.Screens;

public class EarnCoinsScreen : UIScreen
{
	[SerializeField]
	private GameObject _watchVideoPrefab;

	[SerializeField]
	private GameObject _coinEarnerPrefab;

	[SerializeField]
	private GameObject _facebookGiftPrefab;

	[SerializeField]
	private GameObject _rewardForVideoButtonPrefab;

	[SerializeField]
	private GameObject _facebookRewardsButtonPrefab;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIScrollView _scrollView;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private GameObject _loadingOffersPrefab;

	[SerializeField]
	private GameObject _listTitleComponent;

	[SerializeField]
	private GameObject _scrollBar;

	[SerializeField]
	private GameObject _noOffersLabelGameObject;

	private CoinEarnerLoadingOffer _coinEarnerLoadingOffer;

	private GameObject _facebookRewardsButton;

	private const int TITLE_LABEL_HEIGHT = 39;

	private const int SMALL_CELL_HEIGHT = 65;

	private const int LARGE_CELL_HEIGHT = 79;

	private static bool _facebookLoginRunning;

	private static bool _shouldRefreshCurrencyEarners;

	private int _tableContentCombinedHeight;

	private bool isVideoAdsSupported => Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Globals.DEBUG_SKIP_VIDEO_ADS;

	[UsedImplicitly]
	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			RefreshCurrencyEarners();
		}
	}

	protected override void Init()
	{
		HandleSafeArea();
	}

	private void HandleSafeArea()
	{
		if (SafeArea.IsRestricted)
		{
			_scrollPanel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
			_scrollPanel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
			_scrollPanel.ResetAndUpdateAnchors();
		}
	}

	protected override void Show()
	{
		if (_footerHandler != null)
		{
			_footerHandler.HandleStickers();
		}
		OfferWallManager.instance.onOfferWallReceived += OfferWallRequestSuccess;
		if (OfferWallManager.instance.IsOfferWallAllowed() && !OfferWallManager.instance.IsOfferWallReady())
		{
			OfferWallManager.instance.RequestOfferWall();
			OfferWallManager.instance.GetPendingRewards();
		}
		Flurry.LogEventWithAParameter("Earn coins screen opened", "State", AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS).ToString());
		RefreshCurrencyEarners();
	}

	protected override void Hide()
	{
		OfferWallManager.instance.onOfferWallReceived -= OfferWallRequestSuccess;
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	public override void GainFocus()
	{
		base.GainFocus();
		RefreshCurrencyEarners();
	}

	public void RefreshCurrencyEarners()
	{
		FillTable();
	}

	private void Update()
	{
		if (_shouldRefreshCurrencyEarners)
		{
			RefreshCurrencyEarners();
		}
	}

	private void FillTable()
	{
		_tableContentCombinedHeight = 0;
		foreach (Transform item in _table.transform)
		{
			NGUITools.SetActive(item.gameObject, state: false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		int num = 0;
		_noOffersLabelGameObject.SetActive(value: false);
		UIWidget uIWidget = _table.gameObject.AddWidget<UIWidget>();
		uIWidget.height = 10;
		uIWidget.name = $"{num:000}";
		num++;
		GameObject gameObject = _table.gameObject.AddChild(_listTitleComponent);
		ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
		gameObject.name = $"{num:000}";
		component.Setup(Strings.Get("EARN_COINS_TITLE"), ColorData.instance.midnight);
		num++;
		_tableContentCombinedHeight += 39;
		if (!AgeRestriction.isRestricted)
		{
			for (int i = 0; i < EarnCoinsData.profiles.Length; i++)
			{
				if (EarnCoinsData.profiles[i].id == EarnCoinsData.Id.FacebookWeeklyGift)
				{
					InsertEarner(i, ref num);
					break;
				}
			}
		}
		for (int j = 0; j < EarnCoinsData.profiles.Length; j++)
		{
			if (EarnCoinsData.profiles[j].id == EarnCoinsData.Id.VideoAdCycle)
			{
				InsertEarner(j, ref num);
				break;
			}
		}
		InsertVideoButtons(ref num);
		if (!AgeRestriction.isRestricted)
		{
			if (OfferWallManager.instance.IsOfferWallAllowed() && !OfferWallManager.instance.isOfferWallDisabled)
			{
				if (OfferWallManager.instance.IsOfferWallReady())
				{
					InsertOffer(ref num, CoinEarnerLoadingOffer.OffersState.OfferWall);
				}
				else
				{
					InsertOffer(ref num, CoinEarnerLoadingOffer.OffersState.Loading);
				}
			}
			if (!AgeRestriction.isRestricted && !UserSave.hasPaidOutFacebookReward)
			{
				_facebookRewardsButton = _table.gameObject.AddChild(_facebookRewardsButtonPrefab);
				_facebookRewardsButton.name = $"{num:000}";
				_facebookRewardsButton.GetComponent<UIDragScrollView>().scrollView = _scrollView;
				_facebookRewardsButton.GetComponent<FacebookRewardsButton>().onFacebookClick += FacebookLoginClicked;
				_tableContentCombinedHeight += 79;
				num++;
			}
			for (int k = 0; k < EarnCoinsData.profiles.Length; k++)
			{
				EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile = EarnCoinsData.profiles[k];
				switch (earnCurrencyProfile.id)
				{
				case EarnCoinsData.Id.VideoAdCycle:
				case EarnCoinsData.Id.FacebookLogin:
				case EarnCoinsData.Id.FacebookWeeklyGift:
					continue;
				}
				InsertEarner(k, ref num);
			}
			if (OnlineSettingsManager.data.socialLinkOverrides != null)
			{
				List<string> list = OnlineSettingsManager.data.socialLinkOverrides.Keys.Where((string x) => !Enum.GetNames(typeof(EarnCoinsData.Id)).Contains(x)).ToList();
				foreach (string item2 in list)
				{
					InsertCustomLink(ref num, item2, OnlineSettingsManager.data.socialLinkOverrides[item2]);
				}
			}
		}
		if (_scrollPanel != null)
		{
			if (Math.Abs(Math.Abs(_scrollPanel.baseClipRegion.w)) < 1f)
			{
				StartCoroutine(DelayedSetScrollViewCenterOffset());
			}
			else
			{
				SetScrollViewCenterOffset();
			}
		}
		_shouldRefreshCurrencyEarners = false;
	}

	private void InsertEarner(int index, ref int numberOfObjects)
	{
		if (EarnCoinsData.profiles.Length <= index || !EarnCoinsData.ShouldShowInGUI(index))
		{
			return;
		}
		EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile = EarnCoinsData.profiles[index];
		if (earnCurrencyProfile.id == EarnCoinsData.Id.VideoAdCycle)
		{
			if (isVideoAdsSupported)
			{
				GameObject gameObject = _table.gameObject.AddChild(_watchVideoPrefab);
				gameObject.name = $"{numberOfObjects:000}";
				string desc = string.Format(Strings.Get(earnCurrencyProfile.desc), earnCurrencyProfile.GetAmountOfCoins());
				gameObject.GetComponent<CoinEarnerVideoAds>().Init(index, Strings.Get(earnCurrencyProfile.title), desc, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), null, _scrollView);
				gameObject.GetComponent<UIDragScrollView>().scrollView = _scrollView;
				NGUITools.AddWidgetCollider(gameObject);
				UIPlaySound component = gameObject.GetComponent<UIPlaySound>();
				if (component != null)
				{
					component.enabled = false;
				}
				_tableContentCombinedHeight += 65;
				numberOfObjects++;
			}
		}
		else if (earnCurrencyProfile.id == EarnCoinsData.Id.FacebookWeeklyGift)
		{
			if (FacebookWeeklyGiftManager.IsGiftAvailable())
			{
				GameObject gameObject2 = _table.gameObject.AddChild(_facebookGiftPrefab);
				gameObject2.name = $"{numberOfObjects:000}";
				gameObject2.GetComponent<FacebookGiftButton>().Init(index, Strings.Get(earnCurrencyProfile.title), Strings.Get(earnCurrencyProfile.desc), null, _scrollView);
				gameObject2.GetComponent<UIDragScrollView>().scrollView = _scrollView;
				NGUITools.AddWidgetCollider(gameObject2);
				UIPlaySound component2 = gameObject2.GetComponent<UIPlaySound>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
				_tableContentCombinedHeight += 65;
				numberOfObjects++;
			}
		}
		else if (earnCurrencyProfile.id == EarnCoinsData.Id.FacebookKiloo || earnCurrencyProfile.id == EarnCoinsData.Id.FacebookSybo || earnCurrencyProfile.id == EarnCoinsData.Id.YoutubeKiloo || earnCurrencyProfile.id == EarnCoinsData.Id.YoutubeSybo || earnCurrencyProfile.id == EarnCoinsData.Id.TwitterKiloo || earnCurrencyProfile.id == EarnCoinsData.Id.TwitterSybo || earnCurrencyProfile.id == EarnCoinsData.Id.CustomLink)
		{
			InsertEarner(ref numberOfObjects, earnCurrencyProfile);
		}
		else
		{
			GameObject gameObject3 = _table.gameObject.AddChild(_coinEarnerPrefab);
			gameObject3.name = $"{numberOfObjects:000}";
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				string desc2 = string.Format(Strings.Get(earnCurrencyProfile.iOSDesc), earnCurrencyProfile.GetAmountOfCoins());
				gameObject3.GetComponent<CoinEarnerButtonHelper>().Init(index, Strings.Get(earnCurrencyProfile.title), desc2, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), null);
			}
			else
			{
				string desc3 = string.Format(Strings.Get(earnCurrencyProfile.desc), earnCurrencyProfile.GetAmountOfCoins());
				gameObject3.GetComponent<CoinEarnerButtonHelper>().Init(index, Strings.Get(earnCurrencyProfile.title), desc3, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), null);
			}
			gameObject3.GetComponent<UIDragScrollView>().scrollView = _scrollView;
			NGUITools.AddWidgetCollider(gameObject3);
			_tableContentCombinedHeight += 65;
			numberOfObjects++;
		}
	}

	private void InsertEarner(ref int numberOfObjects, EarnCoinsData.EarnCurrencyProfile profile)
	{
		if (!EarnCoinsData.ShouldShowInGUI(profile))
		{
			return;
		}
		switch (profile.id)
		{
		case EarnCoinsData.Id.FacebookKiloo:
		case EarnCoinsData.Id.FacebookSybo:
		case EarnCoinsData.Id.TwitterKiloo:
		case EarnCoinsData.Id.TwitterSybo:
		case EarnCoinsData.Id.YoutubeKiloo:
		case EarnCoinsData.Id.YoutubeSybo:
		{
			string desc = string.Format(Strings.Get(profile.iOSDesc), profile.GetAmountOfCoins());
			string desc2 = string.Format(Strings.Get(profile.desc), profile.GetAmountOfCoins());
			string title = Strings.Get(profile.title);
			if (OnlineSettingsManager.data.socialLinkOverrides != null && OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(profile.id.ToString()))
			{
				SocialLinkOverrideInfo socialLinkOverrideInfo2 = OnlineSettingsManager.data.socialLinkOverrides[profile.id.ToString()];
				if (socialLinkOverrideInfo2.title != null)
				{
					string linkLocalizedTitle2 = OnlineSettingsManager.localizedData.getLinkLocalizedTitle(profile.id.ToString());
					if (linkLocalizedTitle2.IsNullOrEmpty())
					{
						break;
					}
					title = linkLocalizedTitle2;
				}
				if (socialLinkOverrideInfo2.desc != null)
				{
					string linkLocalizedDesc = OnlineSettingsManager.localizedData.getLinkLocalizedDesc(profile.id.ToString());
					if (linkLocalizedDesc.IsNullOrEmpty())
					{
						break;
					}
					desc2 = linkLocalizedDesc;
				}
				if (socialLinkOverrideInfo2.iosDesc != null)
				{
					string linkLocalizediOSDesc = OnlineSettingsManager.localizedData.getLinkLocalizediOSDesc(profile.id.ToString());
					if (linkLocalizediOSDesc.IsNullOrEmpty())
					{
						break;
					}
					desc = linkLocalizediOSDesc;
				}
				if (!socialLinkOverrideInfo2.link.IsNullOrEmpty() && Uri.TryCreate(socialLinkOverrideInfo2.link, UriKind.Absolute, out var result2) && result2.Scheme == Uri.UriSchemeHttps)
				{
					profile.url = socialLinkOverrideInfo2.link;
				}
				if (socialLinkOverrideInfo2.reward != -1)
				{
					if (socialLinkOverrideInfo2.reward <= 0)
					{
						break;
					}
					profile.defaultAmountOfCoins = socialLinkOverrideInfo2.reward;
				}
			}
			GameObject gameObject2 = _table.gameObject.AddChild(_coinEarnerPrefab);
			gameObject2.name = $"{numberOfObjects:000}";
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				gameObject2.GetComponent<CoinEarnerButtonHelper>().Init(profile, title, desc, profile.iconName, profile.GetAmountOfCoins(), null);
			}
			else
			{
				gameObject2.GetComponent<CoinEarnerButtonHelper>().Init(profile, title, desc2, profile.iconName, profile.GetAmountOfCoins(), null);
			}
			gameObject2.GetComponent<UIDragScrollView>().scrollView = _scrollView;
			NGUITools.AddWidgetCollider(gameObject2);
			_tableContentCombinedHeight += 65;
			numberOfObjects++;
			break;
		}
		case EarnCoinsData.Id.CustomLink:
		{
			if (OnlineSettingsManager.data.socialLinkOverrides == null || !OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(profile.customIdentifier))
			{
				break;
			}
			SocialLinkOverrideInfo socialLinkOverrideInfo = OnlineSettingsManager.data.socialLinkOverrides[profile.customIdentifier];
			if (socialLinkOverrideInfo.title == null || socialLinkOverrideInfo.desc == null || socialLinkOverrideInfo.iosDesc == null || socialLinkOverrideInfo.link.IsNullOrEmpty() || socialLinkOverrideInfo.reward == -1)
			{
				break;
			}
			string linkLocalizedTitle = OnlineSettingsManager.localizedData.getLinkLocalizedTitle(profile.customIdentifier);
			string text = null;
			string text2 = null;
			if (linkLocalizedTitle.IsNullOrEmpty())
			{
				break;
			}
			text = linkLocalizedTitle;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				linkLocalizedTitle = OnlineSettingsManager.localizedData.getLinkLocalizediOSDesc(profile.customIdentifier);
				if (linkLocalizedTitle.IsNullOrEmpty())
				{
					break;
				}
				text2 = linkLocalizedTitle;
			}
			else
			{
				linkLocalizedTitle = OnlineSettingsManager.localizedData.getLinkLocalizedDesc(profile.customIdentifier);
				if (linkLocalizedTitle.IsNullOrEmpty())
				{
					break;
				}
				text2 = linkLocalizedTitle;
			}
			if (Uri.TryCreate(socialLinkOverrideInfo.link, UriKind.Absolute, out var result) && result.Scheme == Uri.UriSchemeHttps)
			{
				profile.url = socialLinkOverrideInfo.link;
				if (socialLinkOverrideInfo.reward > 0)
				{
					profile.defaultAmountOfCoins = socialLinkOverrideInfo.reward;
					GameObject gameObject = _table.gameObject.AddChild(_coinEarnerPrefab);
					gameObject.name = $"{numberOfObjects:000}";
					gameObject.GetComponent<CoinEarnerButtonHelper>().Init(profile, text, text2, profile.iconName, profile.GetAmountOfCoins(), null);
					gameObject.GetComponent<UIDragScrollView>().scrollView = _scrollView;
					NGUITools.AddWidgetCollider(gameObject);
					_tableContentCombinedHeight += 65;
					numberOfObjects++;
				}
			}
			break;
		}
		case EarnCoinsData.Id.Review:
		case EarnCoinsData.Id.AdColony:
		case EarnCoinsData.Id.VideoAdCycle:
		case EarnCoinsData.Id.FacebookLogin:
		case EarnCoinsData.Id.FacebookWeeklyGift:
		case EarnCoinsData.Id.RewardForVideo:
			break;
		}
	}

	private void InsertOffer(ref int numberOfObjects, CoinEarnerLoadingOffer.OffersState offerState)
	{
		GameObject gameObject = _table.gameObject.AddChild(_loadingOffersPrefab);
		gameObject.name = $"{numberOfObjects:000}";
		gameObject.GetComponent<UIDragScrollView>().scrollView = _scrollView;
		_coinEarnerLoadingOffer = gameObject.GetComponent<CoinEarnerLoadingOffer>();
		NGUITools.AddWidgetCollider(gameObject);
		_tableContentCombinedHeight += 79;
		numberOfObjects++;
		if (_coinEarnerLoadingOffer != null)
		{
			_coinEarnerLoadingOffer.Init(ForceRefreshOfferWall);
			_coinEarnerLoadingOffer.SetOffersState(offerState);
		}
	}

	private void InsertVideoButtons(ref int numberOfObjects)
	{
		VideoButtonPayload[] rewards = RewardForVideoButtonHelper.rewards;
		VideoButtonPayload[] array = rewards;
		foreach (VideoButtonPayload videoButtonPayload in array)
		{
			if (videoButtonPayload.isAvailable())
			{
				GameObject gameObject = _table.gameObject.AddChild(_rewardForVideoButtonPrefab);
				gameObject.name = $"{numberOfObjects++:000}";
				gameObject.GetComponent<UIDragScrollView>().scrollView = _scrollView;
				gameObject.GetComponent<RewardForVideoButton>().Init(videoButtonPayload);
				_tableContentCombinedHeight += 65;
			}
		}
	}

	private void InsertCustomLink(ref int numberOfObjects, string linkIdentifier, SocialLinkOverrideInfo socialLinkData)
	{
		EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile = new EarnCoinsData.EarnCurrencyProfile();
		earnCurrencyProfile.id = EarnCoinsData.Id.CustomLink;
		earnCurrencyProfile.customIdentifier = linkIdentifier;
		earnCurrencyProfile.iconName = "icon_coinPack_1";
		EarnCoinsData.EarnCurrencyProfile profile = earnCurrencyProfile;
		InsertEarner(ref numberOfObjects, profile);
	}

	private void ForceRefreshOfferWall()
	{
		OfferWallManager.instance.RequestOfferWall();
	}

	private void OfferWallRequestSuccess()
	{
		RefreshCurrencyEarners();
	}

	public void FacebookLoginClicked()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(loggedIn: true);
		}
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private static void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new SlideIn
		{
			type = SlideInData.SlideInType.Facebook,
			facebookState = UISlideInFacebook.FacebookSlideInState.ConnectFailed,
			onDidShowCallback = onFinishCallback
		});
	}

	private void OnFailSlideInFinish()
	{
		FacebookStatusChanged(success: false);
	}

	private void FacebookStatusChanged(bool success)
	{
		if (success)
		{
			if (!FacebookRewardsInfo.PayoutRewardIfPossible())
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.Facebook,
					facebookState = UISlideInFacebook.FacebookSlideInState.ConnectSucceeded
				});
			}
			UnityEngine.Object.Destroy(_facebookRewardsButton);
		}
		else
		{
			if (_facebookLoginRunning)
			{
				OnFacebookLoginResult(loggedIn: false);
			}
			_facebookRewardsButton.GetComponent<FacebookRewardsButton>().GoToActiveState();
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_shouldRefreshCurrencyEarners = true;
		_facebookLoginRunning = false;
	}

	private IEnumerator DelayedSetScrollViewCenterOffset()
	{
		yield return new WaitForEndOfFrame();
		SetScrollViewCenterOffset();
	}

	private void SetScrollViewCenterOffset()
	{
		if (IsListBiggerThanClip())
		{
			_scrollView.enabled = true;
			_scrollBar.SetActive(value: true);
		}
		else
		{
			UIWidget uIWidget = _table.gameObject.AddWidget<UIWidget>();
			uIWidget.height = 1000;
			_scrollView.ResetPosition();
			_scrollView.enabled = false;
			_scrollBar.SetActive(value: false);
		}
		_table.Reposition();
	}

	private bool IsListBiggerThanClip()
	{
		bool result = false;
		if (_scrollPanel != null && (float)_tableContentCombinedHeight > _scrollPanel.baseClipRegion.w)
		{
			result = true;
		}
		return result;
	}
}
