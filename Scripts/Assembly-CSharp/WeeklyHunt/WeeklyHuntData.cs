using System;
using System.Collections.Generic;
using System.Globalization;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace WeeklyHunt;

public static class WeeklyHuntData
{
	public enum WeeklyHuntPrize
	{
		NotAvailable = 0,
		Key = 1,
		SuperMysteryBox = 2,
		Hoverboard = 3,
		ScoreBooster = 4,
		Headstart = 5,
		ChristmasBoard = 6,
		RioBoard2 = 7,
		NorthpoleBoard2016 = 8,
		CherryBoard = 9,
		Coin = 10,
		TokenMysteryBox = 11
	}

	public enum PrizePeriodId
	{
		None = 0,
		PeriodOne = 1,
		PeriodTwo = 2,
		PeriodThree = 3,
		PeriodFour = 4
	}

	public enum PeriodStatus
	{
		Unstarted = 0,
		Expired = 1,
		Current = 2
	}

	public class WeeklyHuntPeriodData
	{
		public int itemsToCollect;

		public int itemsCollected;

		public bool hiddenUntilUnlocked;

		public string periodTitle;

		public DateTime periodStartDate;

		public DateTime periodEndDate;

		public PrizeData mainPrize;
	}

	public struct PrizeData
	{
		public int amount;

		public CelebrationRewardOrigin celebrationRewardOrigin;

		public CelebrationRewardType rewardType;

		public UpgradeType upgradeType;

		public Characters.CharacterId characterId;

		public Boards.BoardId boardId;

		public Trophies.Trophy trophyType;
	}

	public static bool hideLastPeriod = true;

	private static bool _hasInited = false;

	private const string DEBUG_TIME_OFFSET_PLAYER_PREFS_KEY = "WEEKLY_HUNT_TIME_OFFSET";

	private static Dictionary<int, WeeklyHuntPeriodData> _huntPeriodData = new Dictionary<int, WeeklyHuntPeriodData>
	{
		{
			1,
			new WeeklyHuntPeriodData
			{
				itemsToCollect = 10,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = "WEEKLY_HUNT_PERIOD_ONE_PERIOD_TITLE",
				periodStartDate = new DateTime(2019, 2, 1),
				periodEndDate = new DateTime(2019, 3, 14),
				mainPrize = new PrizeData
				{
					amount = 2,
					celebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal,
					rewardType = CelebrationRewardType.powerup,
					upgradeType = UpgradeType.scorebooster
				}
			}
		},
		{
			2,
			new WeeklyHuntPeriodData
			{
				itemsToCollect = 20,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = "WEEKLY_HUNT_PERIOD_TWO_PERIOD_TITLE",
				periodStartDate = new DateTime(2019, 3, 14),
				periodEndDate = new DateTime(2019, 3, 21),
				mainPrize = new PrizeData
				{
					amount = 5,
					celebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal,
					rewardType = CelebrationRewardType.powerup,
					upgradeType = UpgradeType.headstart2000
				}
			}
		},
		{
			3,
			new WeeklyHuntPeriodData
			{
				itemsToCollect = 40,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = "WEEKLY_HUNT_PERIOD_THREE_PERIOD_TITLE",
				periodStartDate = new DateTime(2019, 3, 21),
				periodEndDate = new DateTime(2019, 4, 4),
				mainPrize = new PrizeData
				{
					amount = 2,
					celebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox
				}
			}
		},
		{
			4,
			new WeeklyHuntPeriodData
			{
				itemsToCollect = 10,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = "WEEKLY_HUNT_PERIOD_FOUR_PERIOD_TITLE",
				periodStartDate = new DateTime(2016, 9, 2),
				periodEndDate = new DateTime(2016, 9, 2),
				mainPrize = new PrizeData
				{
					amount = 2,
					celebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal,
					rewardType = CelebrationRewardType.powerup,
					upgradeType = UpgradeType.scorebooster
				}
			}
		}
	};

	public static DateTime startChallengeDateTime
	{
		get
		{
			InitIfNeeded();
			return _huntPeriodData[1].periodStartDate;
		}
	}

	public static Dictionary<int, WeeklyHuntPeriodData> GetHuntData()
	{
		InitIfNeeded();
		return _huntPeriodData;
	}

	private static void InitIfNeeded()
	{
		if (!_hasInited)
		{
			if (Globals.DEBUG)
			{
				ApplyDebugTimeOffset();
			}
			_hasInited = true;
			LoadProgressFromResources();
			AppRefreshHandler.hasRefreshed += OnCloudSyncAppRefresh;
		}
	}

	private static void OnCloudSyncAppRefresh(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.Data))
		{
			LoadProgressFromResources();
		}
	}

	private static void LoadProgressFromResources()
	{
		WeeklyHuntProgressData weeklyHuntProgressData = EventsSave.weeklyHuntProgressData;
		if (!ShouldResetChallengeData(weeklyHuntProgressData.huntStartVersion))
		{
			List<int> tokenProgress = weeklyHuntProgressData.tokenProgress;
			foreach (KeyValuePair<int, WeeklyHuntPeriodData> huntPeriodDatum in _huntPeriodData)
			{
				WeeklyHuntPeriodData value = huntPeriodDatum.Value;
				int index = huntPeriodDatum.Key - 1;
				value.itemsCollected = (tokenProgress.ValidIndex(index) ? tokenProgress[index] : 0);
			}
			FlurryLogCurrentUpdatesWeeklyHuntPeriodProgress();
		}
		else
		{
			FlurryLogPreviousUpdatesWeeklyHuntPeriodProgress();
			SaveNewProgress(shouldStartNewProgress: true);
		}
	}

	private static bool ShouldResetChallengeData(string currentStartDate)
	{
		if (!string.Equals(currentStartDate, startChallengeDateTime.ToString(CultureInfo.InvariantCulture)))
		{
			return true;
		}
		return false;
	}

	public static void SaveNewProgress(bool shouldStartNewProgress)
	{
		WeeklyHuntProgressData weeklyHuntProgressData = new WeeklyHuntProgressData();
		weeklyHuntProgressData.huntStartVersion = startChallengeDateTime.ToString(CultureInfo.InvariantCulture);
		weeklyHuntProgressData.tokenProgress = new List<int> { 0, 0, 0, 0 };
		WeeklyHuntProgressData weeklyHuntProgressData2 = weeklyHuntProgressData;
		if (shouldStartNewProgress)
		{
			EventsSave.weeklyHuntProgressVersion = DeviceUtility.GetBundleVersion();
		}
		foreach (KeyValuePair<int, WeeklyHuntPeriodData> huntPeriodDatum in _huntPeriodData)
		{
			WeeklyHuntPeriodData value = huntPeriodDatum.Value;
			int index = huntPeriodDatum.Key - 1;
			if (shouldStartNewProgress)
			{
				value.itemsCollected = 0;
				EventsSave.hasLoggedWeeklyHuntPeriod[index] = false;
			}
			else
			{
				weeklyHuntProgressData2.tokenProgress[index] = value.itemsCollected;
			}
		}
		EventsSave.weeklyHuntProgressData = weeklyHuntProgressData2;
	}

	private static void FlurryLogCurrentUpdatesWeeklyHuntPeriodProgress()
	{
		foreach (KeyValuePair<int, WeeklyHuntPeriodData> huntPeriodDatum in _huntPeriodData)
		{
			PrizePeriodId key = (PrizePeriodId)huntPeriodDatum.Key;
			WeeklyHuntPeriodData value = huntPeriodDatum.Value;
			bool flag = true;
			int index = huntPeriodDatum.Key - 1;
			List<bool> hasLoggedWeeklyHuntPeriod = EventsSave.hasLoggedWeeklyHuntPeriod;
			if (hasLoggedWeeklyHuntPeriod.ValidIndex(index))
			{
				flag = hasLoggedWeeklyHuntPeriod[index];
			}
			if (!flag && value.periodEndDate < CDateTime.UtcNow)
			{
				hasLoggedWeeklyHuntPeriod[index] = true;
				if (key != PrizePeriodId.PeriodFour || !hideLastPeriod)
				{
					Flurry.LogEventWithAParameter("WeeklyHunt Progress", EventsSave.weeklyHuntProgressVersion + key, value.itemsCollected.ToString());
				}
			}
		}
	}

	private static void FlurryLogPreviousUpdatesWeeklyHuntPeriodProgress()
	{
		List<int> tokenProgress = EventsSave.weeklyHuntProgressData.tokenProgress;
		if (!string.IsNullOrEmpty(EventsSave.weeklyHuntProgressVersion))
		{
			return;
		}
		List<bool> hasLoggedWeeklyHuntPeriod = EventsSave.hasLoggedWeeklyHuntPeriod;
		foreach (KeyValuePair<int, WeeklyHuntPeriodData> huntPeriodDatum in _huntPeriodData)
		{
			int index = huntPeriodDatum.Key - 1;
			if (!hasLoggedWeeklyHuntPeriod.ValidIndex(index) || !hasLoggedWeeklyHuntPeriod[index])
			{
				int num = 0;
				if (tokenProgress.ValidIndex(index))
				{
					num = tokenProgress[index];
				}
				Flurry.LogEventWithAParameter("WeeklyHunt Progress", EventsSave.weeklyHuntProgressVersion + (PrizePeriodId)huntPeriodDatum.Key, num.ToString());
			}
		}
	}

	private static void ApplyDebugTimeOffset()
	{
		int num = PlayerPrefs.GetInt("WEEKLY_HUNT_TIME_OFFSET", 0);
		if (num == 0)
		{
			return;
		}
		foreach (KeyValuePair<int, WeeklyHuntPeriodData> huntPeriodDatum in _huntPeriodData)
		{
			huntPeriodDatum.Value.periodStartDate = huntPeriodDatum.Value.periodStartDate.AddSeconds(num);
			huntPeriodDatum.Value.periodEndDate = huntPeriodDatum.Value.periodEndDate.AddSeconds(num);
		}
	}

	public static void DebugJumpToWeek(int period, int additionalOffset)
	{
		int num = PlayerPrefs.GetInt("WEEKLY_HUNT_TIME_OFFSET", 0);
		DateTime dateTime = _huntPeriodData[period].periodStartDate;
		if (additionalOffset < 0)
		{
			dateTime = _huntPeriodData[period].periodEndDate;
		}
		dateTime = dateTime.AddSeconds(additionalOffset);
		int num2 = Convert.ToInt32(-1.0 * (dateTime - CDateTime.UtcNow).TotalSeconds);
		PlayerPrefs.SetInt("WEEKLY_HUNT_TIME_OFFSET", num + num2);
		foreach (KeyValuePair<int, WeeklyHuntPeriodData> huntPeriodDatum in _huntPeriodData)
		{
			huntPeriodDatum.Value.periodStartDate = huntPeriodDatum.Value.periodStartDate.AddSeconds(num2);
			huntPeriodDatum.Value.periodEndDate = huntPeriodDatum.Value.periodEndDate.AddSeconds(num2);
		}
		PlayerPrefs.Save();
		SaveNewProgress(shouldStartNewProgress: false);
	}

	public static void DebugSetProgress(int period, int progress)
	{
		_huntPeriodData[period].itemsCollected = progress;
		SaveNewProgress(shouldStartNewProgress: false);
	}

	public static void DebugResetWeeklyHuntData()
	{
		int num = PlayerPrefs.GetInt("WEEKLY_HUNT_TIME_OFFSET", 0);
		foreach (KeyValuePair<int, WeeklyHuntPeriodData> huntPeriodDatum in _huntPeriodData)
		{
			huntPeriodDatum.Value.periodStartDate = huntPeriodDatum.Value.periodStartDate.AddSeconds(-1 * num);
			huntPeriodDatum.Value.periodEndDate = huntPeriodDatum.Value.periodEndDate.AddSeconds(-1 * num);
			huntPeriodDatum.Value.itemsCollected = 0;
		}
		PlayerPrefs.DeleteKey("WEEKLY_HUNT_TIME_OFFSET");
		SaveNewProgress(shouldStartNewProgress: false);
		PlayerPrefs.Save();
	}

	public static PrizePeriodId GetActivePeriod()
	{
		Dictionary<int, WeeklyHuntPeriodData>.Enumerator enumerator = _huntPeriodData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PrizePeriodId key = (PrizePeriodId)enumerator.Current.Key;
			WeeklyHuntPeriodData value = enumerator.Current.Value;
			DateTime periodEndDate = value.periodEndDate;
			DateTime periodStartDate = value.periodStartDate;
			if (DateTime.Compare(periodEndDate, CDateTime.UtcNow) >= 0 && DateTime.Compare(periodStartDate, CDateTime.UtcNow) <= 0 && (key != PrizePeriodId.PeriodFour || !hideLastPeriod))
			{
				return key;
			}
		}
		enumerator.Dispose();
		return PrizePeriodId.None;
	}
}
