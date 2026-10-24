using System;
using AppRefresh;
using Data.Save;
using Kiloo.Common.Plugins.Notifications;
using UnityEngine;

namespace Info;

public static class Settings
{
	public static bool optionSfx
	{
		get
		{
			return PreferencesSave.soundEffectsEnabled;
		}
		set
		{
			PreferencesSave.soundEffectsEnabled = value;
			onSfxSettingChanged?.Invoke();
			NGUITools.soundVolume = (value ? 1f : 0f);
		}
	}

	public static bool optionMusic
	{
		get
		{
			return PreferencesSave.musicEnabled;
		}
		set
		{
			PreferencesSave.musicEnabled = value;
			onMusicSettingsChanged?.Invoke();
		}
	}

	public static bool optionTopRunChallengers
	{
		get
		{
			return PreferencesSave.topRunChallengersEnabled;
		}
		set
		{
			PreferencesSave.topRunChallengersEnabled = value;
			onTopRunChallengerSettingChanged?.Invoke(value);
		}
	}

	public static bool anySoundEnabled
	{
		get
		{
			if (!optionMusic)
			{
				return optionSfx;
			}
			return true;
		}
	}

	public static bool optionReminder
	{
		get
		{
			EnableReminderIfOSAllowsNotifications();
			return PreferencesSave.localNotificationsEnabled > 0;
		}
		set
		{
			PreferencesSave.localNotificationsEnabled = (value ? 1 : 0);
		}
	}

	public static event Action<bool> onTopRunChallengerSettingChanged;

	public static event Action onSfxSettingChanged;

	public static event Action onMusicSettingsChanged;

	static Settings()
	{
		CheckApplication();
		RefreshSettings();
		AppRefreshHandler.hasRefreshed += (RefreshType refreshType) =>
		{
			if (refreshType.HasFlag(RefreshType.Data))
			{
				RefreshSettings();
			}
		};
		HookUpEvents();
		InitNotificationsIfNotInited();
	}

	private static void HookUpEvents()
	{
		if (PreferencesSave.localNotificationsEnabled == -1)
		{
			UIScreenController.OnAwake += UpdateLocalNotificationsEnabledState;
		}
		UIScreenController.OnApplicationPaused += OnApplicationPaused;
	}

	private static void InitNotificationsIfNotInited()
	{
		if (Notifications.instance == null && Application.isEditor)
		{
			Notifications.Init(receiveNotifications: false);
		}
	}

	private static void OnApplicationPaused()
	{
		UserSave.wereNotificationsEnabled = Notifications.instance.areNotificationsEnabled;
	}

	private static void UpdateLocalNotificationsEnabledState()
	{
		optionReminder = Notifications.instance.areNotificationsEnabled;
	}

	private static void RefreshSettings()
	{
		optionSfx = PreferencesSave.soundEffectsEnabled;
		optionMusic = PreferencesSave.musicEnabled;
		optionTopRunChallengers = PreferencesSave.topRunChallengersEnabled;
	}

	private static void EnableReminderIfOSAllowsNotifications()
	{
		if (!UserSave.wereNotificationsEnabled && Notifications.instance.areNotificationsEnabled)
		{
			optionReminder = true;
		}
		UserSave.wereNotificationsEnabled = Notifications.instance.areNotificationsEnabled;
	}

	public static void ToggleCurrentSfxOption()
	{
		optionSfx = !optionSfx;
	}

	public static void ToggleCurrentMusicOption()
	{
		optionMusic = !optionMusic;
	}

	public static void ToggleOptionReminders()
	{
		optionReminder = !optionReminder;
	}

	public static void EnableAllSoundOptions()
	{
		optionSfx = true;
		optionMusic = true;
	}

	public static void DisableAllSoundOptions()
	{
		optionSfx = false;
		optionMusic = false;
	}

	private static void CheckApplication()
	{
		if (!new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]).Contains("com.random."))
		{
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.QueueMessage("Stopping Playmode...");
		}
	}
}
