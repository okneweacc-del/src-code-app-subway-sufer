using ABTest;
using Data.Save;
using Info;
using Missions;
using SYBO.Subway;
using UI;

public static class SaveMeManager
{
	public enum ClickAction
	{
		WatchAd = 0,
		SpendKeys = 1
	}

	private const int NUMBER_OF_RUNS_BEFORE_SAVE_ME_FUNCTIONALITY = 4;

	private const string ONLINESETTINGS_ALWAYS_ON_AD_BUTTON_SAVE_ME = "ad_button_save_me_always_on";

	private const string ONLINESETTINGS_MAX_TIMES_TO_REVIVE_WITH_ADS_IN_A_RUN = "max_times_to_revive_with_ads_in_a_run";

	private const bool ONLINESETTINGS_ALWAYS_ON_AD_BUTTON_DEFAULT_VALUE = true;

	private const int ONLINESETTINGS_MAX_TIMES_TO_REVIVE_WITH_ADS_IN_A_RUN_DEFAULT_VALUE = 2;

	private static int _numberOfUsedKeysInCurrentRun;

	private static int _timesRevivedWithAds;

	private static bool _hasReviveWithAdsBeenAvailableInCurrentRun;

	public static bool isPurchaseMadeFromIngame;

	public static bool isPurchaseRunningIngame;

	public static bool videoIsPlaying { get; set; }

	public static int saveMePopupCountInCurrentRun { get; private set; }

	public static int reviveCountInCurrentRun { get; private set; }

	public static bool revivedWithAds { get; set; }

	public static bool adsFailed { get; set; }

	public static bool adsNotAvailable { get; set; }

	public static bool isRevivalVideoCached => AdSystemManager.IsCached(AdLocations.REVIVAL_VIDEOS);

	public static bool buttonAlwaysOnEnabled => OnlineSettings.instance.GetValue("ad_button_save_me_always_on", defaultValue: true);

	static SaveMeManager()
	{
		_numberOfUsedKeysInCurrentRun = 1;
		videoIsPlaying = false;
		Game.OnMenuScreenShown += HandleOnMenuScreenShown;
	}

	public static void IncrementNumberOfUsedKeys()
	{
		if (_numberOfUsedKeysInCurrentRun <= 0)
		{
			_numberOfUsedKeysInCurrentRun = 1;
		}
		else
		{
			_numberOfUsedKeysInCurrentRun *= 2;
		}
	}

	public static float GetTimeOutAnimationDuration()
	{
		return ABTestManager.GetPayloadInt("slowrevivetimer");
	}

	public static void IncrementSaveMePopupCountInCurrentRun()
	{
		saveMePopupCountInCurrentRun++;
	}

	public static void IncrementRevivalCountInCurrentRun()
	{
		reviveCountInCurrentRun++;
	}

	public static int GetNumberOfKeysToSaveMe()
	{
		return _numberOfUsedKeysInCurrentRun;
	}

	public static bool IsReviveWithAdsAvailable()
	{
		bool flag = ((!buttonAlwaysOnEnabled) ? (isRevivalVideoCached && HasNotReachedMaxNumberOfTimesToReviveWithAdsInCurrentRun()) : HasNotReachedMaxNumberOfTimesToReviveWithAdsInCurrentRun());
		if (flag)
		{
			_hasReviveWithAdsBeenAvailableInCurrentRun = true;
		}
		return flag;
	}

	public static bool HasNotReachedMaxNumberOfTimesToReviveWithAdsInCurrentRun()
	{
		int value = OnlineSettings.instance.GetValue("max_times_to_revive_with_ads_in_a_run", 2);
		return _timesRevivedWithAds < value;
	}

	public static void ResetSaveMeForNewRun()
	{
		_numberOfUsedKeysInCurrentRun = 1;
		_hasReviveWithAdsBeenAvailableInCurrentRun = false;
		saveMePopupCountInCurrentRun = 0;
		reviveCountInCurrentRun = 0;
		_timesRevivedWithAds = 0;
	}

	public static bool IsSaveMePopupEnabled()
	{
		return PlayerSave.completedRunCount >= 4;
	}

	public static void SkipReviveIfPurchaseFailed()
	{
		if (UIScreenController.IsInstanced && UIScreenController.Instance.GetTopScreenName() == "IngameUI" && isPurchaseMadeFromIngame)
		{
			isPurchaseMadeFromIngame = false;
			isPurchaseRunningIngame = false;
			Revive instance = Revive.Instance;
			if (instance != null)
			{
				instance.SendSkipRevive();
			}
		}
	}

	public static void PostSaveMeClickEvent(ClickAction action)
	{
		int numberOfKeysToSaveMe = GetNumberOfKeysToSaveMe();
		string empty = string.Empty;
		empty = ((action != ClickAction.SpendKeys) ? "Video" : ((!IsReviveWithAdsAvailable()) ? "KeysNoVideo" : "KeysVideo"));
		Flurry.LogEventWithAParameter("Save Me clicked", "Action;" + empty, empty + ";" + numberOfKeysToSaveMe);
	}

	public static void RevivePlayer()
	{
		IncrementRevivalCountInCurrentRun();
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			SDebug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(isActive: true);
		}
		Revive.Instance.SendRevive(revivedWithAds, adsNotAvailable, adsFailed);
	}

	public static void OnFinishedWatchingVideo(string sourcePopup)
	{
		videoIsPlaying = false;
		revivedWithAds = true;
		_timesRevivedWithAds++;
		Flurry.LogEvent("Resumed run after revive with video");
		CountdownPopupManager.instance.StartCountdown(5f, RevivePlayer);
		UIScreenController.Instance.CloseActivePopups(sourcePopup);
	}

	public static void SendReviveIfPurchaseSucceeded()
	{
		if (!UIScreenController.IsInstanced)
		{
			return;
		}
		string topScreenName = UIScreenController.Instance.GetTopScreenName();
		if (!string.IsNullOrEmpty(topScreenName) && topScreenName.Equals("IngameUI") && isPurchaseMadeFromIngame)
		{
			if (InventorySave.keys - GetNumberOfKeysToSaveMe() >= 0)
			{
				InventorySave.DecreaseKeys(GetNumberOfKeysToSaveMe(), "InRun");
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, GetNumberOfKeysToSaveMe());
			}
			isPurchaseMadeFromIngame = false;
			isPurchaseRunningIngame = false;
			Revive instance = Revive.Instance;
			if (instance != null)
			{
				instance.SendRevive(revivedWithAds: false, IsReviveWithAdsAvailable(), adsFailed: false);
			}
			IncrementNumberOfUsedKeys();
			IncrementRevivalCountInCurrentRun();
			UIScreenController instance2 = UIScreenController.Instance;
			IngameScreen ingameScreen = instance2.GetScreenFromCache(instance2.GetTopScreenName()) as IngameScreen;
			if (ingameScreen == null)
			{
				SDebug.LogError("IngameScreen == NULL");
			}
			else
			{
				ingameScreen.SetPauseButtonVisibility(isActive: true);
			}
		}
	}

	private static void HandleOnMenuScreenShown()
	{
		if (PlayerSave.completedRunCount > 4)
		{
			string text = "Video not available";
			if (_hasReviveWithAdsBeenAvailableInCurrentRun)
			{
				text = revivedWithAds.ToString();
			}
			Flurry.LogEventWithSeveralParameters("End of run", "Number of revives;Used video", reviveCountInCurrentRun + ";" + text);
		}
		ResetSaveMeForNewRun();
	}
}
