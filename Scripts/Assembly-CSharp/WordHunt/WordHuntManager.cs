using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using Missions;
using SYBO.Components.Utilities;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Pickers;
using UI.Screens.RewardManagement;
using UI.SlideIns;
using UnityEngine;

namespace WordHunt;

public class WordHuntManager
{
	private enum WordHuntServerData
	{
		wordIndex = 0,
		dailyWord = 1,
		year = 2,
		checksum = 3,
		month = 4,
		day = 5,
		hour = 6,
		minute = 7,
		second = 8,
		expireSeconds = 9
	}

	public class WordHuntPayload
	{
		public bool todayCompleted;

		public int activeDayIndex;
	}

	public enum WordHuntRewardType
	{
		Coins = 0,
		MysteryBox = 1
	}

	public struct WordHuntReward
	{
		public WordHuntRewardType type;

		public int coins;

		public MysteryBox.Type mysteryBoxType;
	}

	private static readonly WordHuntReward[] REWARDS;

	private string[] _lastRawData;

	private DateTime _lastRequestTime = DateTime.MinValue;

	private static WordHuntManager _instance;

	public static WordHuntManager instance
	{
		get
		{
			WordHuntManager result;
			if ((result = _instance) == null)
			{
				result = (_instance = new WordHuntManager());
			}
			return result;
		}
	}

	public static event Action OnPickedUpLetter;

	public static event Action OnDailyWordInRowChanged;

	public static event Action<int> OnDailyLetterCoinsRewarded;

	private WordHuntManager()
	{
		UIScreenController.OnChangedScreen += AutoSyncer;
		AppRefreshHandler.hasRefreshed += AppHasRefreshed;
	}

	private void AppHasRefreshed(RefreshType obj)
	{
		if (obj.HasFlag(RefreshType.Data))
		{
			if (!IsAllowedToMakeRequest(CDateTime.UtcNow) && _lastRawData != null)
			{
				LoadFromRawData(_lastRawData);
			}
			else
			{
				DownloadDaily();
			}
		}
	}

	private void AutoSyncer(string screenName)
	{
		if (screenName == "FrontScreen" || screenName == "GameOverScreen")
		{
			DownloadDailyIfExpired();
		}
	}

	public void DownloadDaily()
	{
		CoroutineUtil.StartCoroutine(DownloadDailyCoroutine());
	}

	public void DownloadDailyIfExpired()
	{
		if (EventsSave.wordHuntTimeLeft.TotalSeconds <= 0.0)
		{
			CoroutineUtil.StartCoroutine(DownloadDailyCoroutine());
		}
	}

	private IEnumerator DownloadDailyCoroutine()
	{
		DateTime utcNow = CDateTime.UtcNow;
		if (IsAllowedToMakeRequest(utcNow))
		{
			_lastRequestTime = utcNow;
			string[] array = new string[10]
			{
				"wordIndexValue",
				"exampleWord",
				DateTime.UtcNow.Year.ToString(),
				"checksumValue",
				DateTime.UtcNow.Month.ToString(),
				DateTime.UtcNow.Day.ToString(),
				DateTime.UtcNow.Hour.ToString(),
				DateTime.UtcNow.Minute.ToString(),
				DateTime.UtcNow.Second.ToString(),
				"3600"
			};
			if (SHA1HashCheck(array, GenerateKey()))
			{
				_lastRawData = array;
				LoadFromRawData(array);
			}
		}
		yield break;
	}

	private bool IsAllowedToMakeRequest(DateTime time)
	{
		return (time - _lastRequestTime).TotalSeconds > 120.0;
	}

	private void LoadFromRawData(string[] rawData)
	{
		string word = GetWord();
		int secondsLeft = 3600;
		CalculateAndSaveLastKnownServerTime(DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Day.ToString(), DateTime.UtcNow.Hour.ToString(), DateTime.UtcNow.Minute.ToString(), DateTime.UtcNow.Second.ToString());
		InitWordHunt(word, secondsLeft);
	}

	private void CalculateAndSaveLastKnownServerTime(string year, string month, string day, string hour, string minute, string second)
	{
		try
		{
			UserSave.lastKnownServerTime = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second), DateTimeKind.Utc);
		}
		catch (Exception)
		{
		}
	}

	private bool SHA1HashCheck(string[] rawData, string key)
	{
		return true;
	}

	private static string RandomString(int size)
	{
		StringBuilder stringBuilder = new StringBuilder();
		System.Random random = new System.Random();
		for (int i = 0; i < size; i++)
		{
			char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	private static int RandomNumber(int min, int max)
	{
		return new System.Random().Next(min, max);
	}

	private static string GenerateKey()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(RandomString(4));
		stringBuilder.Append(RandomNumber(0, 999));
		stringBuilder.Append(RandomString(2));
		return stringBuilder.ToString();
	}

	public static WordHuntPayload GetWordHuntPayload()
	{
		int num = Mathf.Clamp(GetWordHuntDaysInARow(out var todayCompleted), 0, 5);
		int activeDayIndex = ((!todayCompleted) ? num : (num - 1));
		return new WordHuntPayload
		{
			activeDayIndex = activeDayIndex,
			todayCompleted = todayCompleted
		};
	}

	public static WordHuntReward GetRewardForDay(int dayIndex)
	{
		if (dayIndex >= REWARDS.Length)
		{
			dayIndex = REWARDS.Length - 1;
		}
		return REWARDS[dayIndex];
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_DAILYWORD_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}

	private static void InitWordHunt(string word, int secondsLeft)
	{
		if (Globals.DEBUG_USE_DEBUG_DAILYWORD)
		{
			word = "Day";
			secondsLeft++;
		}
		if (!EventsSave.wordHuntDailyWord.Equals(word))
		{
			EventsSave.wordHuntDailyWord = word;
			EventsSave.wordHuntPayedOut = false;
			EventsSave.wordHuntUnlockedMask = 0;
		}
		if (!EventsSave.wordHuntPayedOut)
		{
			SingletonMonoBehaviour<PickupManager>.Instance.SetPickerState(PickerType.LetterHuntDaily, PickerEnabledState.Enabled);
			DailyLetterPickupManager.Instance.UpdateLetter();
		}
		DateTime dateTime = UserSave.lastKnownServerTime.AddSeconds(secondsLeft);
		DateTime wordHuntExpireDeviceTime = CDateTime.UtcNow.AddSeconds(secondsLeft);
		if (!EventsSave.wordHuntExpireTime.Equals(dateTime))
		{
			EventsSave.wordHuntExpireTime = dateTime;
			EventsSave.wordHuntExpireDeviceTime = wordHuntExpireDeviceTime;
		}
	}

	public static void PickedupLetter(char letter)
	{
		for (int i = 0; i < EventsSave.wordHuntDailyWord.Length; i++)
		{
			if (EventsSave.wordHuntDailyWord[i] == letter && !EventsSave.wordHuntUnlockedMask[i])
			{
				IntMask wordHuntUnlockedMask = EventsSave.wordHuntUnlockedMask;
				wordHuntUnlockedMask[i] = true;
				EventsSave.wordHuntUnlockedMask = wordHuntUnlockedMask;
				OnPickedUpLetter.SafeInvoke();
				break;
			}
		}
		if (!IsDailyWordComplete() || EventsSave.wordHuntPayedOut)
		{
			return;
		}
		EventsSave.wordHuntWordsInRow = GetWordHuntDaysInARow(out var _) + 1;
		EventsSave.wordHuntLastPayoutDayOfYear = EventsSave.wordHuntExpireTime.DayOfYear;
		OnDailyWordInRowChanged?.Invoke();
		MissionsManager.Instance.PlayerDidThis(MissionTarget.DailyQuests);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.DailyQuestInRow);
		WordHuntReward rewardForDay = GetRewardForDay(EventsSave.wordHuntWordsInRow - 1);
		if (rewardForDay.type == WordHuntRewardType.Coins)
		{
			if (rewardForDay.coins > 0)
			{
				InventorySave.IncreaseCoins(rewardForDay.coins, "WeeklyHunt");
				UIScreenController.Instance.SetCoinRewardAmount(rewardForDay.coins);
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.LettersCompleteCoins
				});
				OnDailyLetterCoinsRewarded.SafeInvoke(rewardForDay.coins);
			}
		}
		else if (rewardForDay.type == WordHuntRewardType.MysteryBox)
		{
			RewardManager.AddRewardToUnlock((rewardForDay.mysteryBoxType == MysteryBox.Type.Normal) ? CelebrationRewardOrigin.MysteryBox : CelebrationRewardOrigin.SuperMysteryBox, ObtainableItemSource.DailyLetters);
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.LettersCompleteMysteryBox,
				mysteryBoxType = rewardForDay.mysteryBoxType
			});
			EventsSave.willShowSMBAfterDailyChallenge = true;
		}
		else
		{
			SDebug.LogError("Unhandled DailyWordRewardType: " + rewardForDay.type);
		}
		EventsSave.wordHuntPayedOut = true;
		Flurry.LogEventWithAParameter("Daily Challenge completed", "Id", EventsSave.wordHuntDailyWord);
	}

	public static char GetNewDailyLetter()
	{
		for (int i = 0; i < EventsSave.wordHuntDailyWord.Length; i++)
		{
			if (!EventsSave.wordHuntUnlockedMask[i])
			{
				return EventsSave.wordHuntDailyWord[i];
			}
		}
		return '\0';
	}

	public static bool IsDailyWordComplete()
	{
		if (!string.IsNullOrEmpty(EventsSave.wordHuntDailyWord))
		{
			return (1 << EventsSave.wordHuntDailyWord.Length) - 1 == (int)EventsSave.wordHuntUnlockedMask;
		}
		return false;
	}

	public static int GetWordHuntDaysInARow(out bool todayCompleted)
	{
		int dayOfYear = EventsSave.wordHuntExpireTime.DayOfYear;
		if (EventsSave.wordHuntLastPayoutDayOfYear == dayOfYear)
		{
			todayCompleted = true;
			return EventsSave.wordHuntWordsInRow;
		}
		if (EventsSave.wordHuntLastPayoutDayOfYear == dayOfYear - 1 || (dayOfYear == 1 && EventsSave.wordHuntLastPayoutDayOfYear >= 365))
		{
			todayCompleted = false;
			return EventsSave.wordHuntWordsInRow;
		}
		todayCompleted = false;
		return 0;
	}

	public static void CheckIfWeShouldRemoveProgressForDailyQuestInRow()
	{
		if (GetWordHuntDaysInARow(out var todayCompleted) == 0 && !todayCompleted)
		{
			MissionsManager.Instance.RemoveProgressForThis(MissionTarget.DailyQuestInRow);
		}
	}

	public static bool IsActiveDailyLetterEvent()
	{
		return GetNewDailyLetter() > '\0';
	}

	public static bool HasTimeLeft()
	{
		if (IsActiveDailyLetterEvent())
		{
			return EventsSave.wordHuntTimeLeft.TotalSeconds > 0.0;
		}
		return false;
	}

	static WordHuntManager()
	{
		REWARDS = new WordHuntReward[5]
		{
			new WordHuntReward
			{
				type = WordHuntRewardType.MysteryBox,
				mysteryBoxType = MysteryBox.Type.Normal
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.MysteryBox,
				mysteryBoxType = MysteryBox.Type.Normal
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.Coins,
				coins = 1050
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.Coins,
				coins = 1500
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.MysteryBox,
				mysteryBoxType = MysteryBox.Type.Super
			}
		};
		OnPickedUpLetter = null;
		OnDailyWordInRowChanged = null;
	}

	private string GetWord()
	{
		string[] array = new string[12]
		{
			"BLINDADO", "PLZ", "RANDOM", "BALDDI", "RAFAXZ", "BAKIZADA", "TAKESHI", "KAWBJJ", "KAUE", "BON",
			"TAVVKKJ", "DLZ"
		};
		return array[UnityEngine.Random.Range(0, array.Length)];
	}
}
