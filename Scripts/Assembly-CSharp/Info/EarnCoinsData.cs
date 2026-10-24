using System;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Kiloo.Common.Plugins.DeviceUtility;
using UI.Screens.RewardManagement;
using UI.VideoAdPrizes;
using UnityEngine;

namespace Info;

public static class EarnCoinsData
{
	public enum Id
	{
		FacebookKiloo = 0,
		FacebookSybo = 1,
		TwitterKiloo = 2,
		TwitterSybo = 3,
		YoutubeKiloo = 4,
		YoutubeSybo = 5,
		Review = 6,
		AdColony = 7,
		VideoAdCycle = 8,
		FacebookLogin = 9,
		FacebookWeeklyGift = 10,
		RewardForVideo = 11,
		CustomLink = 12
	}

	public enum Type
	{
		OpenURL = 0,
		VideoAd = 1,
		ClaimReward = 2
	}

	public enum Repeatability
	{
		Once = 0,
		OncePerVersion = 1,
		Forever = 2
	}

	public class EarnCurrencyProfile
	{
		public Id id;

		public Type type;

		public Repeatability repeatability;

		public int defaultAmountOfCoins;

		public string customIdentifier;

		public string title;

		public string desc;

		public string iOSDesc;

		public string iconName = string.Empty;

		public string url;

		public string overrideAmountOfCoinsOnlineSettingKey;

		public int GetAmountOfCoins()
		{
			if (!string.IsNullOrEmpty(overrideAmountOfCoinsOnlineSettingKey) && OnlineSettings.instance.TryGetValue(overrideAmountOfCoinsOnlineSettingKey, out var valueString))
			{
				if (int.TryParse(valueString, out var result))
				{
					if (result > 0)
					{
						return result;
					}
				}
				else
				{
					SDebug.LogError("EarnCurrencyProfile ERROR: Unable to parse int from onlinesetting " + overrideAmountOfCoinsOnlineSettingKey + ": " + valueString);
				}
			}
			return defaultAmountOfCoins;
		}
	}

	private const string ONLINESETTINGS_VIDEOADS_DEFAULTREWARD_KEY = "videoads_defaultreward";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string, int> OnRewardURLOpened__BackingField;

	public static readonly EarnCurrencyProfile[] profiles = new EarnCurrencyProfile[9]
	{
		new EarnCurrencyProfile
		{
			id = Id.VideoAdCycle,
			type = Type.VideoAd,
			repeatability = Repeatability.Forever,
			title = "EARN_PRIZE_SPONSORED_VIDEO",
			desc = "EARN_PRIZE_SPONSORED_VIDEO_DESCRIPTION",
			iOSDesc = "EARN_PRIZE_SPONSORED_VIDEO_DESCRIPTION",
			iconName = "icon_small_mini_mysteryBox_glow",
			overrideAmountOfCoinsOnlineSettingKey = "videoads_defaultreward"
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookLogin,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 5000,
			title = "EARN_COINS_FACEBOOK_LOGIN",
			desc = "EARN_COINS_FACEBOOK_LOGIN_DESCRIPTION",
			iOSDesc = "EARN_COINS_FACEBOOK_LOGIN_DESCRIPTION",
			iconName = "icon_coinPack_1",
			url = string.Empty
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookWeeklyGift,
			type = Type.ClaimReward,
			repeatability = Repeatability.Forever,
			title = "FACEBOOK_GIFT",
			desc = "EARN_COINS_FACEBOOKGIFT_DESCRIPTION",
			iOSDesc = "EARN_COINS_FACEBOOKGIFT_DESCRIPTION",
			iconName = "icon_upgrades_superMysteryBox"
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookKiloo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = "EARN_COINS_KILOO_FACEBOOK",
			desc = "EARN_COINS_FACEBOOK_DESCRIPTION",
			iOSDesc = "EARN_COINS_GENERIC_IOS_DESCRIPTION",
			iconName = "icon_coinPack_1",
			url = "http://m.facebook.com/kiloogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookSybo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = "EARN_COINS_SYBO_FACEBOOK",
			desc = "EARN_COINS_FACEBOOK_DESCRIPTION",
			iOSDesc = "EARN_COINS_GENERIC_IOS_DESCRIPTION",
			iconName = "icon_coinPack_1",
			url = "http://m.facebook.com/SubwaySurfersSybo"
		},
		new EarnCurrencyProfile
		{
			id = Id.YoutubeKiloo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = "EARN_COINS_KILOO_YOUTUBE",
			desc = "EARN_COINS_YOUTUBE_DESCRIPTION",
			iOSDesc = "EARN_COINS_GENERIC_IOS_DESCRIPTION",
			iconName = "icon_coinPack_1",
			url = "http://m.youtube.com/kiloomobile"
		},
		new EarnCurrencyProfile
		{
			id = Id.YoutubeSybo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = "EARN_COINS_SYBO_YOUTUBE",
			desc = "EARN_COINS_YOUTUBE_DESCRIPTION",
			iOSDesc = "EARN_COINS_GENERIC_IOS_DESCRIPTION",
			iconName = "icon_coinPack_1",
			url = "http://m.youtube.com/sybogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.TwitterKiloo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = "EARN_COINS_KILOO_TWITTER",
			desc = "EARN_COINS_TWITTER_DESCRIPTION",
			iOSDesc = "EARN_COINS_GENERIC_IOS_DESCRIPTION",
			iconName = "icon_coinPack_1",
			url = "https://mobile.twitter.com/kiloogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.TwitterSybo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = "EARN_COINS_SYBO_TWITTER",
			desc = "EARN_COINS_TWITTER_DESCRIPTION",
			iOSDesc = "EARN_COINS_GENERIC_IOS_DESCRIPTION",
			iconName = "icon_coinPack_1",
			url = "https://mobile.twitter.com/sybogames"
		}
	};

	private static bool isVideoAddsSupported => Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor;

	public static event Action<string, int> OnRewardURLOpened
	{
		add
		{
			Action<string, int> action = OnRewardURLOpened__BackingField;
			Action<string, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnRewardURLOpened__BackingField, (Action<string, int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string, int> action = OnRewardURLOpened__BackingField;
			Action<string, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnRewardURLOpened__BackingField, (Action<string, int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static EarnCurrencyProfile[] GetOnlineSettingsLinks()
	{
		return new EarnCurrencyProfile[0];
	}

	public static bool ShouldShowInGUI(int profileIndex)
	{
		return ShouldShowInGUI(profiles[profileIndex]);
	}

	public static bool ShouldShowInGUI(EarnCurrencyProfile profile)
	{
		switch (profile.id)
		{
		case Id.FacebookLogin:
			return !UserSave.hasPaidOutFacebookReward;
		case Id.FacebookWeeklyGift:
			return FacebookWeeklyGiftManager.IsGiftAvailable();
		case Id.FacebookKiloo:
		case Id.FacebookSybo:
		case Id.TwitterKiloo:
		case Id.TwitterSybo:
		case Id.YoutubeKiloo:
		case Id.YoutubeSybo:
			if (OnlineSettingsManager.data.socialLinkOverrides != null && OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(profile.id.ToString()) && !OnlineSettingsManager.data.socialLinkOverrides[profile.id.ToString()].enabled)
			{
				return false;
			}
			break;
		case Id.CustomLink:
			if (OnlineSettingsManager.data.socialLinkOverrides != null && OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(profile.customIdentifier) && !OnlineSettingsManager.data.socialLinkOverrides[profile.customIdentifier].enabled)
			{
				return false;
			}
			break;
		}
		if (profile.repeatability == Repeatability.Once || profile.repeatability == Repeatability.OncePerVersion)
		{
			if ((profile.id == Id.CustomLink) ? UserSave.earnCurrencyCustomSocialLinkData.TryGetValue(profile.customIdentifier, out var value) : UserSave.earnCurrencyData.TryGetValue(profile.id, out value))
			{
				if (profile.repeatability == Repeatability.OncePerVersion)
				{
					string bundleVersion = DeviceUtility.GetBundleVersion();
					if (value != bundleVersion)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
		if (profile.id == Id.VideoAdCycle)
		{
			return isVideoAddsSupported;
		}
		return true;
	}

	public static int GetNumberOfDisplayedNonOfferEarners()
	{
		int num = 0;
		for (int i = 0; i < profiles.Length; i++)
		{
			if (ShouldShowInGUI(i))
			{
				num++;
			}
		}
		return num;
	}

	public static void Trigger(int profileIndex)
	{
		EarnCurrencyProfile profile = profiles[profileIndex];
		Trigger(profile);
	}

	public static void Trigger(EarnCurrencyProfile profile)
	{
		if (!ShouldShowInGUI(profile))
		{
			SDebug.LogError("An already trigerred coin earner button has been pressed!");
			return;
		}
		Flurry.LogEventWithAParameter("Earn Coins item clicked", "Id", (profile.id != Id.CustomLink) ? profile.id.ToString() : profile.customIdentifier);
		if (profile.id == Id.FacebookLogin && SocialManager.instance.FacebookLogin(null))
		{
			UIScreenController.Instance.FacebookLogIn(loggedIn: true);
		}
		if (profile.type == Type.OpenURL)
		{
			int amountOfCoins = profile.GetAmountOfCoins();
			if (amountOfCoins > 0)
			{
				InventorySave.IncreaseCoins(amountOfCoins, "OpenUrl");
				SaveManager.Save();
				if (OnRewardURLOpened__BackingField != null)
				{
					OnRewardURLOpened__BackingField((profile.id != Id.CustomLink) ? profile.id.ToString() : profile.customIdentifier, amountOfCoins);
				}
			}
		}
		else if (profile.type != Type.VideoAd)
		{
			if (profile.type == Type.ClaimReward)
			{
				if (profile.id == Id.FacebookWeeklyGift)
				{
					RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox, ObtainableItemSource.SocialMedia, "FacebookGift", Id.FacebookWeeklyGift.ToString().ToLowerInvariant());
					FacebookWeeklyGiftManager.GiftWasClaimed();
				}
			}
			else
			{
				SDebug.LogError("Unhandled earner type: " + profile.type);
			}
		}
		if (profile.repeatability == Repeatability.Once || profile.repeatability == Repeatability.OncePerVersion)
		{
			string bundleVersion = DeviceUtility.GetBundleVersion();
			if (profile.id != Id.CustomLink)
			{
				UserSave.earnCurrencyData[profile.id] = bundleVersion;
			}
			else
			{
				UserSave.earnCurrencyCustomSocialLinkData[profile.customIdentifier] = bundleVersion;
			}
		}
		if (profile.type == Type.OpenURL)
		{
			Application.OpenURL(profile.url);
		}
		else if (profile.type == Type.VideoAd)
		{
			if (AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS))
			{
				AdSystemManager.Show(AdLocations.EARN_COINS_VIDEOS, () =>
				{
					VideoAdPrizeManager.instance.GiveReward(UnlockItemSource.VideoAdPrizeEarn);
				});
			}
		}
		else if (profile.type == Type.ClaimReward)
		{
			if (profile.id == Id.FacebookWeeklyGift)
			{
				UIScreenController.Instance.PushCelebrationPopup();
			}
		}
		else
		{
			SDebug.LogError("Unhandled earner type: " + profile.type);
		}
	}
}
