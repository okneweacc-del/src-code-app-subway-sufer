using System.Collections.Generic;
using Cloud;
using Cloud.Internal;
using Data.Save;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Plugins.Notifications;
using NaughtyAttributes;
using SYBO.Subway;
using TopRun;
using UI.Breadcrumbs;
using UI.Generic.Layout;
using UI.Popups.SettingsPopupInternal;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups;

public class SettingsPopup : UIPopup
{
	[SerializeField]
	private UISprite _fill;

	[SerializeField]
	private UISprite _outline;

	[SerializeField]
	private VerticalLayout _layout;

	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private UILabel _version;

	[SerializeField]
	private UILabel _contactInfo;

	[SerializeField]
	private UIScrollView _scrollView;

	[SerializeField]
	private GameObject _debugButton;

	[SerializeField]
	private SettingsButton _settingsButtonPrefab;

	[SerializeField]
	private SettingsLogInFacebookButton _facebookLoginButtonPrefab;

	[SerializeField]
	private UISprite _settingsDividerPrefab;

	[SerializeField]
	private UIPanel _panel;

	private SettingsButton _topRunChallengersButton;

	private SettingsButton _facebookLogoutButton;

	private SettingsLogInFacebookButton _facebookLoginButton;

	private SettingsButton _reminderButton;

	private SettingsButton _supportButton;

	private SettingsButton _languageSelectionButton;

	private SettingsButton _withdrawConsentButton;

	private SettingsButton _onlineSaveButton;

	private SettingsButton _promoCodeButton;

	private List<UISprite> _extraDividers = new List<UISprite>();

	private const int BACKGROUND_FILL_HEIGHT_NORMAL = 432;

	private const int BACKGROUND_FILL_HEIGHT_TUTORIAL = 310;

	private const int BACKGROUND_FILL_HEIGHT_TUTORIAL_OFFSET = 60;

	private float _lastScrollPositionY;

	private float _instantiatedPanelYPosition;

	private int _previousButtonAmount;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
		SetupBasic();
		_version.text = "v" + DeviceUtility.GetBundleVersion();
		_contactInfo.text = Globals.GetContactSupportShortUrl();
		if (!Globals.DEBUG_POPUP || !Globals.DEBUG)
		{
			Object.Destroy(_debugButton);
		}
		LoadAllButtons();
	}

	[Button(null)]
	public override void GainFocus()
	{
		_layout.gameObject.SetActive(value: false);
		_layout.gameObject.SetActive(value: true);
	}

	[Button(null)]
	private void SetupBasic()
	{
		_titleLabel.text = "by random";
		_titleLabel.color = ColorData.instance.midnight;
	}

	private UISprite AddDivider()
	{
		UISprite uISprite = Object.Instantiate(_settingsDividerPrefab, _layout.transform);
		uISprite.color = ColorData.instance.push;
		uISprite.alpha = 0.5f;
		return uISprite;
	}

	private void LoadAllButtons()
	{
		SettingsButton settingsButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		settingsButton.name = "musicButton";
		settingsButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_musicOn",
			iconOffset = new Vector3(-2f, 0f, 0f),
			onClick = Settings.ToggleCurrentMusicOption,
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(!Settings.optionMusic);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_MUSIC");
			},
			setDescription = (UILabel desc) =>
			{
				desc.text = Strings.Get((!Settings.optionMusic) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
			}
		});
		SettingsButton settingsButton2 = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		settingsButton2.name = "sfxButton";
		settingsButton2.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_soundOn",
			onClick = Settings.ToggleCurrentSfxOption,
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(!Settings.optionSfx);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_SFX");
			},
			setDescription = (UILabel desc) =>
			{
				desc.text = Strings.Get((!Settings.optionSfx) ? "SETTINGS_POPUP_OFF" : "SETTINGS_POPUP_ON");
			}
		});
		AddDivider();
		SettingsButton settingsButton3 = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		settingsButton3.name = "countryButton";
		settingsButton3.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_country_settings",
			iconOffset = new Vector3(-1f, 0f, 0f),
			onClick = () =>
			{
				UIScreenController.Instance.PushPopup("CountryPopup");
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_COUNTRY_TITLE");
			},
			setDescription = (UILabel desc) =>
			{
				desc.text = Strings.Get(CountryManager.instance.GetCurrentTournamentCountry().displayNameID);
			}
		});
		_languageSelectionButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_languageSelectionButton.name = "languageSelectionButton";
		_languageSelectionButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_language_settings",
			iconOffset = new Vector3(-1f, 0f, 0f),
			onClick = () =>
			{
				UIScreenController.Instance.PushPopup("LanguageSelectionPopup");
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_LANGUAGE_TITLE");
			},
			setDescription = (UILabel desc) =>
			{
				desc.text = Strings.language.locale.nativeName;
			}
		});
		if (CloudSave.isAvailable)
		{
			AddDivider();
		}
		_onlineSaveButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_onlineSaveButton.name = "onlineSaveButton";
		_onlineSaveButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			descriptionID = "SETTINGS_POPUP_ONLINESAVE_DESCRIPTION",
			iconName = "ic_cloudsave_blank",
			onClick = () =>
			{
				string popupName = ((CloudSave.state != CloudSaveState.Inactive && CloudSave.state != CloudSaveState.OptedOut) ? "CloudSaveStatusPopup" : "CloudSaveDisabledStatusPopup");
				UIScreenController.Instance.PushPopup(popupName);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string value = CloudSave.state.ToString();
				if (CloudSave.state == CloudSaveState.Error)
				{
					value = CloudSave.errorState.ToString();
				}
				dictionary.Add("Source", "FromSettingsPopup");
				dictionary.Add("FromSettingsPopup", value);
				Flurry.LogEventWithSeveralParameters("CloudSave Status Popup", dictionary);
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_ONLINESAVE_TITLE");
			},
			setDescription = (UILabel d) =>
			{
				d.text = Strings.Get((!Game.Instance.IsInGame.Value) ? "SETTINGS_POPUP_ONLINESAVE_DESCRIPTION" : "SETTINGS_POPUP_ONLINESAVE_DESCRIPTION_DISABLED");
			},
			breadcrumbEvents = new Breadcrumb.BreadcrumbEvent[2]
			{
				Breadcrumb.BreadcrumbEvent.CloudSaveConflict,
				Breadcrumb.BreadcrumbEvent.CloudSaveError
			},
			shouldDisable = () => Game.Instance.IsInGame.Value,
			setIcon = (UISprite s) =>
			{
				s.spriteName = ((CloudSave.state != CloudSaveState.Active) ? "ic_cloudsave_blank" : "ic_cloudsave_connected");
			}
		});
		CloudSave.stateChanged += _onlineSaveButton.Refresh;
		_extraDividers.Add(AddDivider());
		_facebookLogoutButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_facebookLogoutButton.name = "fbLogOutButton";
		_facebookLogoutButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "settings_facebook_icon",
			onClick = () =>
			{
				SocialManager.instance.FacebookLogout();
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.Facebook,
					facebookState = UISlideInFacebook.FacebookSlideInState.LoggedOut
				});
				TopRunRoot.manager?.RefreshTournament();
				LoggedOutOfFacebook();
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_FB_TITLE");
			},
			descriptionID = "SETTINGS_POPUP_FB_LOG_OUT_DESC"
		});
		_facebookLoginButton = Object.Instantiate(_facebookLoginButtonPrefab, _layout.transform);
		_facebookLoginButton.name = "facebookLoginButton";
		_facebookLoginButton.InitButton(this);
		_topRunChallengersButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_topRunChallengersButton.name = "topRunChallengersButton";
		_topRunChallengersButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_top_run_settings",
			onClick = () =>
			{
				Settings.optionTopRunChallengers = !Settings.optionTopRunChallengers;
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(!Settings.optionTopRunChallengers);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_TOPRUNFRIENDS");
			},
			setDescription = (UILabel desc) =>
			{
				desc.text = Strings.Get((!Settings.optionTopRunChallengers) ? "SETTINGS_POPUP_TOPRUNFRIENDS_HIDDEN" : "SETTINGS_POPUP_TOPRUNFRIENDS_SHOWN");
			}
		});
		_reminderButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_reminderButton.name = "reminderButton";
		_reminderButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_reminder",
			onClick = Settings.ToggleOptionReminders,
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(!Settings.optionReminder);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_REMINDERS");
			},
			setDescription = (UILabel desc) =>
			{
				desc.text = Strings.Get((!Settings.optionReminder) ? "SETTINGS_POPUP_DISABLED" : "SETTINGS_POPUP_ENABLED");
			}
		});
		AddDivider();
		_supportButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_supportButton.name = "contactSupportButton";
		_supportButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_support",
			onClick = () =>
			{
				ConfirmationPopup.variant = ConfirmationPopupVariant.ContactSupport;
				UIScreenController.Instance.PushPopup("ConfirmationPopup");
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_CONTACT_SUPPORT_TITLE");
			},
			descriptionID = "SETTINGS_POPUP_SUPPORT_DESCRIPTION"
		});
		_promoCodeButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_promoCodeButton.name = "promoCodeButton";
		_promoCodeButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "ic_promo_codes",
			iconOffset = new Vector3(-1f, 0f, 0f),
			onClick = () =>
			{
				UIScreenController.Instance.PushPopup("PromoCodePopup");
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("PROMO_CODES_SETTINGS_TITLE");
			},
			setDescription = (UILabel desc) =>
			{
				desc.text = Strings.Get((!Game.Instance.IsInGame.Value) ? "PROMO_CODES_SETTINGS_SUBTITLE" : "SETTINGS_POPUP_ONLINESAVE_DESCRIPTION_DISABLED");
			},
			shouldDisable = () => Game.Instance.IsInGame.Value
		});
		_withdrawConsentButton = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		_withdrawConsentButton.name = "withdrawConsentButton";
		_withdrawConsentButton.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "icon_consent",
			iconOffset = new Vector3(0f, 1f, 0f),
			onClick = () =>
			{
				UIScreenController.Instance.PushPopup("WithdrawConsentPopup");
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_WITHDRAW_CONSENT");
			},
			setDescription = (UILabel desc) =>
			{
				desc.gameObject.SetActive(value: false);
			}
		});
		SettingsButton settingsButton4 = Object.Instantiate(_settingsButtonPrefab, _layout.transform);
		settingsButton4.name = "privacyPolicyButton";
		settingsButton4.InitButton(new SettingsButton.SettingsButtonPayload
		{
			iconName = "popup_settings_i",
			iconOffset = new Vector3(0f, 1f, 0f),
			onClick = () =>
			{
				ConfirmationPopup.variant = ConfirmationPopupVariant.OpenPrivacyPolicy;
				UIScreenController.Instance.PushPopup("ConfirmationPopup");
			},
			setOfflineIndicator = (GameObject go) =>
			{
				go.SetActive(value: false);
			},
			setTitle = (UILabel label) =>
			{
				label.text = Strings.Get("SETTINGS_POPUP_PRIVACY");
			},
			descriptionID = "SETTINGS_POPUP_OPEN_BROWSER"
		});
	}

	protected override void Show()
	{
		bool tutorialCompleted = OnboardingSave.isTutorialCompleted;
		AddHooks();
		_reminderButton.gameObject.SetActive(tutorialCompleted && Notifications.instance.areNotificationsEnabled);
		_topRunChallengersButton.gameObject.SetActive(tutorialCompleted);
		_supportButton.gameObject.SetActive(tutorialCompleted);
		_withdrawConsentButton.gameObject.SetActive(!AgeRestriction.isRestricted);
		_languageSelectionButton.gameObject.SetActive(Game.IsInstantiated && !Game.Instance.IsInGame.Value);
		_onlineSaveButton.gameObject.SetActive(CloudSave.isAvailable);
		_extraDividers.ForEach((UISprite s) =>
		{
			s.gameObject.SetActive(tutorialCompleted);
		});
		_promoCodeButton.gameObject.SetActive(tutorialCompleted && Game.IsInstantiated);
		if (!tutorialCompleted || AgeRestriction.isRestricted || Game.Instance.IsInGame.Value)
		{
			_facebookLogoutButton.gameObject.SetActive(value: false);
			_facebookLoginButton.gameObject.SetActive(value: false);
		}
		else
		{
			Globals.LogInState loginState = Globals.GetLoginState();
			if (loginState != Globals.LogInState.Facebook && loginState != Globals.LogInState.Both)
			{
				_facebookLogoutButton.gameObject.SetActive(value: false);
				_facebookLoginButton.gameObject.SetActive(value: true);
			}
			else
			{
				_facebookLogoutButton.gameObject.SetActive(value: true);
				_facebookLoginButton.gameObject.SetActive(value: false);
			}
		}
		UpdatePopupDimension(tutorialCompleted);
		_outline.UpdateAnchors();
		_version.UpdateAnchors();
		_contactInfo.UpdateAnchors();
		_layout.Align();
		_scrollView.ResetPosition();
		ScrollToPreviousPositionIfNeeded();
	}

	private void AddHooks()
	{
		UIScreenController.OnApplicationResumed += OnApplicationResumed;
	}

	private void UpdatePopupDimension(bool tutorialCompleted)
	{
		int num;
		if (!tutorialCompleted)
		{
			num = 310;
			if (_languageSelectionButton.gameObject.activeSelf)
			{
				num += 60;
			}
			if (_withdrawConsentButton.gameObject.activeSelf)
			{
				num += 60;
			}
			if (_onlineSaveButton.gameObject.activeSelf)
			{
				num += 60;
			}
		}
		else
		{
			num = 432;
		}
		num = Mathf.Clamp(num, 310, 432);
		_fill.height = num;
		UIRect[] componentsInChildren = _fill.GetComponentsInChildren<UIRect>();
		UIRect[] array = componentsInChildren;
		foreach (UIRect uIRect in array)
		{
			uIRect.ResetAndUpdateAnchors();
		}
	}

	protected override void Hide()
	{
		UpdatePopupDimension(OnboardingSave.isTutorialCompleted);
		RemoveHooks();
		SaveCurrentScrollPosition();
	}

	private void RemoveHooks()
	{
		UIScreenController.OnApplicationResumed -= OnApplicationResumed;
	}

	private void OnApplicationResumed()
	{
		_reminderButton.gameObject.SetActive(OnboardingSave.isTutorialCompleted && Notifications.instance.areNotificationsEnabled);
		_layout.Align();
	}

	public void LoggedOutOfFacebook()
	{
		if (_facebookLogoutButton != null)
		{
			_facebookLogoutButton.gameObject.SetActive(value: false);
		}
		if (_facebookLoginButton != null)
		{
			_facebookLoginButton.gameObject.SetActive(value: true);
		}
		_layout.Align();
	}

	public void ConnectedToFacebook()
	{
		if (_facebookLogoutButton != null)
		{
			_facebookLogoutButton.gameObject.SetActive(value: true);
		}
		if (_facebookLoginButton != null)
		{
			_facebookLoginButton.gameObject.SetActive(value: false);
		}
		_layout.Align();
	}

	private void SaveCurrentScrollPosition()
	{
		_lastScrollPositionY = _instantiatedPanelYPosition + _panel.clipOffset.y;
		_previousButtonAmount = GetAmountOfButtons();
	}

	private void ScrollToPreviousPositionIfNeeded()
	{
		_instantiatedPanelYPosition = _panel.transform.localPosition.y;
		if (GetAmountOfButtons() == _previousButtonAmount)
		{
			_scrollView.MoveRelative(new Vector3(0f, 0f - _lastScrollPositionY, 0f));
		}
	}

	private int GetAmountOfButtons()
	{
		SettingsButton[] componentsInChildren = _layout.GetComponentsInChildren<SettingsButton>(includeInactive: false);
		if (componentsInChildren != null)
		{
			return componentsInChildren.Length;
		}
		return -1;
	}
}
