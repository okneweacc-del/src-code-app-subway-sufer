using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using JetBrains.Annotations;
using Kiloo.Common;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Serialization.Parsing;
using Kiloo.Common.Utility;
using SYBO.Components.Analytics.Integration;
using SYBO.Subway.Info;
using TopRun.Internal;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace Data.Migration;

public static class PlayerInfoMigrator
{
	private class JsonObjectWrapper
	{
		private JsonObject _jsonObject;

		public static string lastLoadedKey { get; set; }

		public JsonObjectWrapper(JsonObject jsonObject)
		{
			_jsonObject = jsonObject;
		}

		public T Get<T>([NotNull] string key, T defaultValue)
		{
			lastLoadedKey = key;
			return _jsonObject.Get(key, defaultValue);
		}
	}

	private const string PLAYER_INFO_SECRET = "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn";

	private const int VERSION = 1;

	private static Key _lastKeyRead;

	private const string TOPRUN_FILENAME_MANAGER = "toprun_manager";

	private const string TOPRUN_FILENAME_PLAYER_IDS = "toprun_playerids";

	private const string TOPRUN_FILENAME_RANKS = "toprun_ranks";

	private const string TOPRUN_SALT = "GhgtrRFRfPLJhFDsWe#dEdrt5rfg56";

	private const string LAST_SEEN_BUNDLE_VERSION_KEY = "lastSeenBundleVersionKey";

	private const byte SOCIALDATA_VERSION = 2;

	private const string ADSYSTEM_FILE_SECRET = "KilooAdSystemInterstitialStatsSecret";

	private static string _lastInterstitialStatsKey;

	public static PlayerInfoData LoadPlayerInfoData()
	{
		if (!TryGetLoadPaths(out var path))
		{
			throw new Exception("File path invalid");
		}
		byte[] buffer = FileUtil.Load(path, "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn");
		MemoryStream memoryStream = new MemoryStream(buffer);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		binaryReader.ReadInt32();
		Dictionary<Key, string> mainPlayerInfoData = FileUtil.ReadEnumStringDictionary<Key>(binaryReader);
		Dictionary<PowerupTypeLegacy, int> upgradeAmounts = FileUtil.ReadEnumIntDictionary<PowerupTypeLegacy>(binaryReader);
		Dictionary<PowerupTypeLegacy, int> upgradeTiers = FileUtil.ReadEnumIntDictionary<PowerupTypeLegacy>(binaryReader);
		IDictionary topRunManager = LoadTopRunJsonFile("toprun_manager") as IDictionary;
		IDictionary topRunPlayerIds = LoadTopRunJsonFile("toprun_playerids") as IDictionary;
		IDictionary topRunRanks = LoadTopRunJsonFile("toprun_ranks") as IDictionary;
		Dictionary<string, Friend.Status> socialData = LoadSocialDataFile();
		IDictionary<string, object> adSystemData = LoadAdSystemDataFile();
		PlayerInfoData playerInfoData = new PlayerInfoData();
		playerInfoData.mainPlayerInfoData = mainPlayerInfoData;
		playerInfoData.upgradeAmounts = upgradeAmounts;
		playerInfoData.upgradeTiers = upgradeTiers;
		playerInfoData.topRunManager = topRunManager;
		playerInfoData.topRunPlayerIds = topRunPlayerIds;
		playerInfoData.topRunRanks = topRunRanks;
		playerInfoData.socialData = socialData;
		playerInfoData.adSystemData = adSystemData;
		PlayerInfoData result = playerInfoData;
		memoryStream.Close();
		return result;
	}

	public static void SavePlayerInfoData(PlayerInfoData playerInfoData)
	{
		MemoryStream memoryStream = new MemoryStream(8192);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(1);
		FileUtil.WriteEnumStringDictionary(binaryWriter, playerInfoData.mainPlayerInfoData);
		FileUtil.WriteEnumIntDictionary(binaryWriter, playerInfoData.upgradeAmounts);
		FileUtil.WriteEnumIntDictionary(binaryWriter, playerInfoData.upgradeTiers);
		FileUtil.Save(GetSavePath(), memoryStream.GetBuffer(), "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn", 0, (int)memoryStream.Length, 1, 5);
		SaveTopRunJsonFile("toprun_manager", playerInfoData.topRunManager);
		SaveTopRunJsonFile("toprun_playerids", playerInfoData.topRunPlayerIds);
		SaveTopRunJsonFile("toprun_ranks", playerInfoData.topRunRanks);
		SaveSocialDataFile(playerInfoData.socialData);
		SaveAdSystemDataFile(playerInfoData.adSystemData);
		memoryStream.Close();
	}

	private static void LogMigrationFail(string lastKey, Exception exception)
	{
		SDebug.LogError($"Exception while migrating, lastKey: '{lastKey}'\n{exception}");
		string value = string.Empty;
		if (PlayerPrefs.HasKey("lastSeenBundleVersionKey"))
		{
			value = PlayerPrefs.GetString("lastSeenBundleVersionKey", string.Empty);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Exception Type", exception.ToString());
		dictionary.Add("Last loaded key", lastKey);
		dictionary.Add("Migrated from", value);
		dictionary.Add(exception.GetType().ToString(), lastKey);
		Dictionary<string, string> parameters = dictionary;
		Flurry.LogEventWithSeveralParameters("PlayerInfo Migration error", parameters);
	}

	public static void Migrate()
	{
		bool flag = true;
		try
		{
			PlayerInfoData playerInfoData = LoadPlayerInfoData();
			Dictionary<Key, string> mainPlayerInfoData = playerInfoData.mainPlayerInfoData;
			Dictionary<PowerupTypeLegacy, int> upgradeAmounts = playerInfoData.upgradeAmounts;
			Dictionary<PowerupTypeLegacy, int> upgradeTiers = playerInfoData.upgradeTiers;
			flag &= MigrateInventorySave(mainPlayerInfoData);
			flag &= MigratePowerups(upgradeAmounts);
			flag &= MigrateUpgrades(upgradeTiers);
			flag &= MigratePlayerSave(mainPlayerInfoData);
			flag &= MigrateSyboAnalyticsSave(mainPlayerInfoData);
			flag &= MigrateAdsSave(mainPlayerInfoData, playerInfoData.adSystemData);
			flag &= MigrateEventsSave(mainPlayerInfoData);
			flag &= MigratePreferencesSave(mainPlayerInfoData);
			flag &= MigrateAnalyticsSave(mainPlayerInfoData);
			flag &= MigrateUserSave(mainPlayerInfoData);
			flag &= MigrateOnboardingSave(mainPlayerInfoData);
			MigrateTopRunSave(mainPlayerInfoData, playerInfoData.topRunManager, playerInfoData.topRunPlayerIds, playerInfoData.topRunRanks);
			MigrateSocialSave(playerInfoData.socialData);
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			flag = false;
		}
		if (flag)
		{
			UserSave.hasMigratedPlayerInfo = true;
			SaveManager.Save();
		}
	}

	private static bool MigrateInventorySave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadInt(mainPlayerData, Key.AmountOfCoins, out var intToSet))
			{
				InventorySave.IncreaseCoins(intToSet - InventorySave.coins, "PlayerInfoMigration");
			}
			if (TryLoadInt(mainPlayerData, Key.AmountOfKeys, out var intToSet2))
			{
				InventorySave.IncreaseKeys(intToSet2 - InventorySave.keys, "PlayerInfoMigration");
			}
			if (TryLoadInt(mainPlayerData, Key.AmountOfUnrewardedCoins, out var intToSet3))
			{
				InventorySave.unrewardedCoins = intToSet3;
			}
			if (TryLoadString(mainPlayerData, Key.CharacterLastSelectedThemes, out var stringToSet))
			{
				Dictionary<Characters.CharacterId, int> dictionary = Globals.convertStringToEnumIntDictionary<Characters.CharacterId>(stringToSet);
				foreach (KeyValuePair<Characters.CharacterId, int> item in dictionary)
				{
					CharacterManager.SetOutfitSelectionIndex(item.Key, item.Value);
				}
			}
			if (TryLoadString(mainPlayerData, Key.CharacterThemesUnlocked, out var stringToSet2))
			{
				Dictionary<Characters.CharacterId, int[]> migrateDict = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterId>(stringToSet2);
				MigrationSave.MigrateUnlockedOutfits(migrateDict);
			}
			int[] characterTokens = LoadIntArray(mainPlayerData, Key.CollectedCharacterTokens, new int[0]);
			MigrationSave.MigrateCharacters(characterTokens);
			if (TryLoadString(mainPlayerData, Key.UnlockedHoverboardTypes, out var stringToSet3))
			{
				MigrationSave.MigrateBoards(stringToSet3);
			}
			if (TryLoadString(mainPlayerData, Key.PendingRewards, out var stringToSet4))
			{
				MigrationSave.MigratePendingRewards(stringToSet4);
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool MigratePowerups(Dictionary<PowerupTypeLegacy, int> upgradeAmounts)
	{
		try
		{
			MigrationSave.SetPowerups(ConvertLegacyPowerupsToUpgrades(upgradeAmounts));
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool MigrateUpgrades(Dictionary<PowerupTypeLegacy, int> upgradeTiers)
	{
		try
		{
			Dictionary<UpgradeType, int> dictionary = new Dictionary<UpgradeType, int>();
			foreach (KeyValuePair<UpgradeType, int> upgrade in InventorySave.upgrades)
			{
				PowerupTypeLegacy key = PowerupTypeLegacy._notset;
				int key2 = (int)upgrade.Key;
				if (Enum.IsDefined(typeof(UpgradeType), key2))
				{
					key = (PowerupTypeLegacy)key2;
				}
				if (upgradeTiers.ContainsKey(key))
				{
					dictionary[upgrade.Key] = upgradeTiers[key];
				}
			}
			MigrationSave.SetUpgrades(dictionary);
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool MigratePlayerSave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadBool(mainPlayerData, Key.HasSkippedMission1, out var boolToSet))
			{
				PlayerSave.SetSkipMission(0, boolToSet);
			}
			if (TryLoadBool(mainPlayerData, Key.HasSkippedMission2, out var boolToSet2))
			{
				PlayerSave.SetSkipMission(1, boolToSet2);
			}
			if (TryLoadBool(mainPlayerData, Key.HasSkippedMission3, out var boolToSet3))
			{
				PlayerSave.SetSkipMission(2, boolToSet3);
			}
			int[] savedArray = LoadIntArray(mainPlayerData, Key.CurrentMissionSetProgress, new int[3]);
			PlayerSave.LoadCurrentMissionProgress(savedArray);
			if (TryLoadInt(mainPlayerData, Key.RunsCompletedInCurrentMission, out var intToSet))
			{
				PlayerSave.runsCompletedInCurrentMissionsSet = intToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.CurrentMissionSet, out var intToSet2))
			{
				PlayerSave.currentMissionSet = intToSet2;
			}
			if (TryLoadInt(mainPlayerData, Key.MissionCompletedSum, out var intToSet3))
			{
				PlayerSave.missionSetCompletedCount = intToSet3;
			}
			PlayerSave.LoadAchievementProgress(LoadIntArray(mainPlayerData, Key.AchievementProgress, new int[41]));
			if (TryLoadInt(mainPlayerData, Key.MissionsVersionLastUpdate, out var intToSet4))
			{
				PlayerSave.missionsVersion = intToSet4;
			}
			if (TryLoadInt(mainPlayerData, Key.UnreportedGames, out var intToSet5))
			{
				PlayerSave.unreportedGames = intToSet5;
			}
			if (TryLoadLong(mainPlayerData, Key.UnreportedGamesTimestamp, out var longToSet))
			{
				PlayerSave.unreportedGamesTimeStamp = new DateTime(longToSet).ToUniversalTime();
			}
			if (TryLoadInt(mainPlayerData, Key.HighestScore, out var intToSet6))
			{
				PlayerSave.SetHighScore(intToSet6);
			}
			bool[] array = LoadBoolArray(mainPlayerData, Key.UnlockedTrophies, new bool[8]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i])
				{
					PlayerSave.UnlockTrophy((Trophies.Trophy)i);
				}
			}
			if (TryLoadString(mainPlayerData, Key.AwardStatus, out var stringToSet))
			{
				PlayerSave.awardsProgress = MigrationSave.MigrateAwardProgress(stringToSet);
			}
			if (TryLoadDateTime(mainPlayerData, Key.LastTimeAWeeklyFacebookGiftWasCollected, out var valueToSet))
			{
				PlayerSave.weeklyGiftUnlockTime = valueToSet;
			}
			if (TryLoadString(mainPlayerData, Key.Stats, out var stringToSet2))
			{
				MigrateStatistics(stringToSet2);
			}
			if (TryLoadInt(mainPlayerData, Key.AmountOfMysteryBoxesOpened, out var intToSet7))
			{
				PlayerSave.MigrateMysteryBoxOpened(MysteryBox.Type.Normal, intToSet7);
			}
			if (TryLoadInt(mainPlayerData, Key.AmountOfSuperMysteryBoxesOpened, out var intToSet8))
			{
				PlayerSave.MigrateMysteryBoxOpened(MysteryBox.Type.Super, intToSet8);
			}
			if (TryLoadInt(mainPlayerData, Key.AmountOfMiniMysteryBoxesOpened, out var intToSet9))
			{
				PlayerSave.MigrateMysteryBoxOpened(MysteryBox.Type.Mini, intToSet9);
			}
			if (TryLoadInt(mainPlayerData, Key.NumberOfRuns, out var intToSet10))
			{
				PlayerSave.completedRunCount = intToSet10;
			}
			if (TryLoadInt(mainPlayerData, Key.CurrentCharacter, out var intToSet11))
			{
				PlayerSave.currentCharacterId = (Characters.CharacterId)intToSet11;
			}
			if (TryLoadEnum<Boards.BoardId>(mainPlayerData, Key.CurrentHoverboard, out var enumToSet))
			{
				PlayerSave.currentBoardId = enumToSet;
			}
			if (TryLoadEnum<Boards.BoardId>(mainPlayerData, Key.PreviousHoverboard, out var enumToSet2))
			{
				PlayerSave.previousBoardId = enumToSet2;
			}
			if (TryLoadString(mainPlayerData, Key.HoverboardCustomizationData, out var stringToSet3))
			{
				MigrationSave.MigrateBoardThemeData(stringToSet3);
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static void MigrateStatistics(string s)
	{
		if (s.Length <= 0)
		{
			return;
		}
		string[] array = s.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num = text.IndexOf('=');
			string value = text.Substring(0, num);
			try
			{
				PlayerSave.SetStatValue((Stat)Enum.Parse(typeof(Stat), value, ignoreCase: true), int.Parse(text.Substring(num + 1)));
			}
			catch (ArgumentException)
			{
			}
		}
	}

	private static void MigrateTopRunSave(Dictionary<Key, string> mainPlayerData, IDictionary manager, IDictionary playerIds, IDictionary ranks)
	{
		if (TryLoadString(mainPlayerData, Key.LastTopRunResultsAwardedID, out var stringToSet))
		{
			TopRunSave.lastTopRunResultAwardedID = stringToSet;
		}
		if (TryLoadString(mainPlayerData, Key.PendingTopRunResultsID, out var stringToSet2))
		{
			TopRunSave.pendingTopRunResultID = stringToSet2;
		}
		if (TryLoadInt(mainPlayerData, Key.PendingTopRunResultScore, out var intToSet))
		{
			TopRunSave.pendingTopRunResultsScore = intToSet;
		}
		if (TryLoadInt(mainPlayerData, Key.PendingTopRunResultRank, out var intToSet2))
		{
			TopRunSave.pendingTopRunResultsRank = intToSet2;
		}
		if (TryLoadInt(mainPlayerData, Key.PendingTopRunResultWeek, out var intToSet3))
		{
			TopRunSave.pendingTopRunResultsWeek = intToSet3;
		}
		if (TryLoadInt(mainPlayerData, Key.LastWeekMedalWasShown, out var intToSet4))
		{
			TopRunSave.lastWeekMedalWasShown = intToSet4;
		}
		try
		{
			if (manager != null)
			{
				JsonObjectWrapper jsonObjectWrapper = new JsonObjectWrapper(new JsonObject(manager));
				ManagerData managerData = new ManagerData();
				managerData.country = jsonObjectWrapper.Get<string>("country", null);
				managerData.localPlayerFacebookId = jsonObjectWrapper.Get<string>("localPlayerFacebookId", null);
				managerData.localPlayerPrivateKey = jsonObjectWrapper.Get<string>("localPlayerPrivateKey", null);
				managerData.localPlayerPublicKey = jsonObjectWrapper.Get<string>("localPlayerPublicKey", null);
				managerData.lastSubmittedScoreCountry = jsonObjectWrapper.Get<string>("lastSubmittedScoreCountry", null);
				managerData.unsubmittedScoreNoTournament = jsonObjectWrapper.Get("unsubmittedScoreNoTournament", 0);
				managerData.nextTournamentTime = TimeUtility.FromUnixTime(jsonObjectWrapper.Get("nextTournamentTime", 0L));
				managerData.lastSubmittedScoreStartTime = TimeUtility.FromUnixTime(jsonObjectWrapper.Get("lastSubmittedScoreStartTime", 0L));
				managerData.lastSubmittedScoreEndTime = TimeUtility.FromUnixTime(jsonObjectWrapper.Get("lastSubmittedScoreEndTime", 0L));
				managerData.unsubmittedScoreNoTournamentTime = TimeUtility.FromUnixTime(jsonObjectWrapper.Get("unsubmittedScoreNoTournamentTime", 0L));
				ManagerData managerData2 = managerData;
				JsonObjectWrapper.lastLoadedKey = "currentTournament";
				if (manager["currentTournament"] is IDictionary json)
				{
					JsonObjectWrapper jsonObjectWrapper2 = new JsonObjectWrapper(new JsonObject(json));
					TournamentData tournamentData = new TournamentData();
					tournamentData.id = jsonObjectWrapper2.Get<string>("id", null);
					tournamentData.sequenceId = jsonObjectWrapper2.Get("sequenceId", -1);
					tournamentData.country = jsonObjectWrapper2.Get<string>("country", null);
					tournamentData.submittedScore = jsonObjectWrapper2.Get("submittedScore", 0);
					tournamentData.unsubmittedScore = jsonObjectWrapper2.Get("unsubmittedScore", 0);
					tournamentData.rank = jsonObjectWrapper2.Get("rank", -1);
					tournamentData.closed = jsonObjectWrapper2.Get("closed", defaultValue: false);
					tournamentData.participants = jsonObjectWrapper2.Get<IList>("participants", null);
					tournamentData.scoreMatchedParticipants = jsonObjectWrapper2.Get<IList>("scoreMatchedParticipants", null);
					tournamentData.participantsTimeStamp = TimeUtility.FromUnixTime(jsonObjectWrapper2.Get("participantsTimeStamp", 0L));
					tournamentData.scoreMatchedParticipantsTimeStamp = TimeUtility.FromUnixTime(jsonObjectWrapper2.Get("scoreMatchedParticipantsTimeStamp", 0L));
					tournamentData.startTime = TimeUtility.FromUnixTime(jsonObjectWrapper2.Get("startTime", 0L));
					tournamentData.endTime = TimeUtility.FromUnixTime(jsonObjectWrapper2.Get("endTime", 0L));
					tournamentData.nextStartTime = TimeUtility.FromUnixTime(jsonObjectWrapper2.Get("nextStartTime", 0L));
					TournamentData currentTournament = tournamentData;
					managerData2.currentTournament = currentTournament;
				}
				JsonObjectWrapper.lastLoadedKey = "previousTournament";
				if (manager["previousTournament"] is IDictionary json2)
				{
					JsonObjectWrapper jsonObjectWrapper3 = new JsonObjectWrapper(new JsonObject(json2));
					TournamentData tournamentData = new TournamentData();
					tournamentData.id = jsonObjectWrapper3.Get<string>("id", null);
					tournamentData.sequenceId = jsonObjectWrapper3.Get("sequenceId", -1);
					tournamentData.country = jsonObjectWrapper3.Get<string>("country", null);
					tournamentData.submittedScore = jsonObjectWrapper3.Get("submittedScore", 0);
					tournamentData.rank = jsonObjectWrapper3.Get("rank", -1);
					TournamentData previousTournament = tournamentData;
					managerData2.previousTournament = previousTournament;
				}
				TopRunSave.managerData = managerData2;
			}
			if (playerIds != null)
			{
				PlayerIdsData playerIdsData = new PlayerIdsData();
				foreach (DictionaryEntry playerId in playerIds)
				{
					if (playerId.Value is IDictionary json3)
					{
						JsonObjectWrapper jsonObjectWrapper4 = new JsonObjectWrapper(new JsonObject(json3));
						PlayerIdsData.PlayerIdData playerIdData = new PlayerIdsData.PlayerIdData();
						playerIdData.name = jsonObjectWrapper4.Get<string>("name", null);
						playerIdData.publicKey = jsonObjectWrapper4.Get<string>("publicKey", null);
						PlayerIdsData.PlayerIdData playerIdData2 = playerIdData;
						long num = jsonObjectWrapper4.Get("expiry", 0L);
						if (num > 0)
						{
							playerIdData2.expiry = new DateTime(num);
						}
						if (playerId.Key is string key)
						{
							playerIdsData.players[key] = playerIdData2;
						}
					}
				}
				TopRunSave.playerIdsData = playerIdsData;
			}
			if (ranks == null)
			{
				return;
			}
			JsonObjectWrapper jsonObjectWrapper5 = new JsonObjectWrapper(new JsonObject(ranks));
			RanksData ranksData = new RanksData();
			ranksData.tournamentId = jsonObjectWrapper5.Get<string>("tournamentId", null);
			RanksData ranksData2 = ranksData;
			JsonObjectWrapper.lastLoadedKey = "ranks";
			if (ranks["ranks"] is IDictionary dictionary)
			{
				foreach (DictionaryEntry item in dictionary)
				{
					Rank value = null;
					if (item.Value is IDictionary json4)
					{
						JsonObjectWrapper jsonObjectWrapper6 = new JsonObjectWrapper(new JsonObject(json4));
						string country = jsonObjectWrapper6.Get<string>("country", null);
						int rank = jsonObjectWrapper6.Get("rank", -1);
						value = new Rank(country, rank);
					}
					if (item.Key is string key2)
					{
						ranksData2.ranks[key2] = value;
					}
				}
			}
			TopRunSave.ranksData = ranksData2;
		}
		catch (Exception exception)
		{
			LogMigrationFail(JsonObjectWrapper.lastLoadedKey, exception);
		}
	}

	private static object LoadTopRunJsonFile(string file)
	{
		string text = null;
		try
		{
			text = Path.Combine(DeviceUtility.GetUserDataPath(), file);
			byte[] array = FileUtil.Load(text, "GhgtrRFRfPLJhFDsWe#dEdrt5rfg56");
			string json = Encoding.UTF8.GetString(array, 0, array.Length);
			return KilooJson.Deserialize(json);
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception ex2)
		{
			SDebug.LogError("LoadJsonFile failed: " + ex2);
		}
		return null;
	}

	private static void SaveTopRunJsonFile(string file, object json)
	{
		try
		{
			string path = Path.Combine(DeviceUtility.GetUserDataPath(), file);
			if (json != null)
			{
				string s = KilooJson.Serialize(json, null);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				FileUtil.Save(path, bytes, "GhgtrRFRfPLJhFDsWe#dEdrt5rfg56");
			}
			else
			{
				File.Delete(path);
			}
		}
		catch (Exception ex)
		{
			SDebug.LogError("SaveJsonFile failed: " + ex);
		}
	}

	private static void MigrateSocialSave(Dictionary<string, Friend.Status> socialData)
	{
		if (socialData != null)
		{
			try
			{
				SocialSave.friendStatus = new Dictionary<string, Friend.Status>(socialData);
			}
			catch (Exception exception)
			{
				LogMigrationFail("friendStatus", exception);
			}
		}
	}

	private static string GetSocialDataSavePath()
	{
		return DeviceUtility.GetUserDataPath() + "/socialdata";
	}

	private static Dictionary<string, Friend.Status> LoadSocialDataFile()
	{
		try
		{
			string socialDataSavePath = GetSocialDataSavePath();
			byte[] buffer = FileUtil.Load(socialDataSavePath, "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg");
			MemoryStream memoryStream = new MemoryStream(buffer);
			Dictionary<string, Friend.Status> result = ReadSocialData(memoryStream);
			memoryStream.Close();
			return result;
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception ex2)
		{
			SDebug.LogError("Could not social load data: " + ex2.Message);
		}
		return null;
	}

	private static Dictionary<string, Friend.Status> ReadSocialData(Stream stream)
	{
		BinaryReader binaryReader = new BinaryReader(stream);
		Dictionary<string, Friend.Status> dictionary;
		switch (binaryReader.ReadByte())
		{
		case 2:
		{
			int num2 = binaryReader.ReadInt32();
			dictionary = new Dictionary<string, Friend.Status>(num2);
			for (int j = 0; j < num2; j++)
			{
				string text2 = binaryReader.ReadString();
				if (!string.IsNullOrEmpty(text2))
				{
					Friend.Status status = new Friend.Status();
					status.gamesCashedIn = binaryReader.ReadInt32();
					status.lastPokeTime = DateTime.FromBinary(binaryReader.ReadInt64());
					Friend.Status value2 = status;
					binaryReader.ReadInt64();
					dictionary[text2] = value2;
				}
			}
			break;
		}
		case 1:
		{
			int num = binaryReader.ReadInt32();
			dictionary = new Dictionary<string, Friend.Status>(num);
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				if (!string.IsNullOrEmpty(text))
				{
					Friend.Status status = new Friend.Status();
					status.gamesCashedIn = binaryReader.ReadInt32();
					status.lastPokeTime = DateTime.FromBinary(binaryReader.ReadInt64());
					Friend.Status value = status;
					dictionary[text] = value;
				}
			}
			break;
		}
		default:
			throw new IOException("Unsupported playerdata file version");
		}
		return dictionary;
	}

	private static void SaveSocialDataFile(Dictionary<string, Friend.Status> friendStatus)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			WriteSocialData(memoryStream, friendStatus);
			byte[] buffer = memoryStream.GetBuffer();
			FileUtil.Save(GetSocialDataSavePath(), buffer, "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg", 0, (int)memoryStream.Length);
			memoryStream.Close();
		}
		catch (Exception ex)
		{
			SDebug.LogError("Error saving social data: " + ex.GetType().Name + ": " + ex.Message + "\n" + ex.StackTrace);
		}
	}

	private static void WriteSocialData(Stream stream, Dictionary<string, Friend.Status> friendStatus)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write((byte)2);
		if (friendStatus != null)
		{
			binaryWriter.Write(friendStatus.Count);
			{
				foreach (KeyValuePair<string, Friend.Status> item in friendStatus)
				{
					binaryWriter.Write(item.Key);
					binaryWriter.Write(item.Value.gamesCashedIn);
					binaryWriter.Write(item.Value.lastPokeTime.ToBinary());
					binaryWriter.Write(item.Value.lastPokeTime.ToBinary());
				}
				return;
			}
		}
		binaryWriter.Write(0);
	}

	private static string GetAdSystemFolderPath()
	{
		string result = string.Empty;
		string userDataPath = DeviceUtility.GetUserDataPath();
		if (!string.IsNullOrEmpty(userDataPath))
		{
			result = userDataPath + "/AdSystem";
		}
		return result;
	}

	private static string GetAdSystemFilePath(string folderPath)
	{
		return folderPath + "/showlimits";
	}

	private static void SaveAdSystemDataFile(IDictionary<string, object> adSystemData)
	{
		string adSystemFolderPath = GetAdSystemFolderPath();
		if (adSystemData == null)
		{
			return;
		}
		if (!Directory.Exists(adSystemFolderPath))
		{
			Directory.CreateDirectory(adSystemFolderPath);
		}
		string adSystemFilePath = GetAdSystemFilePath(adSystemFolderPath);
		StringBuilder stringBuilder = new StringBuilder();
		IDictionary<string, string> dictionary = adSystemData["listVersionForID"] as IDictionary<string, string>;
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			stringBuilder.Append(item.Key);
			stringBuilder.Append(':');
			stringBuilder.Append(item.Value);
			stringBuilder.Append(',');
		}
		adSystemData["listVersionForID"] = stringBuilder.ToString();
		string s = KilooJson.Serialize(adSystemData, null);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		try
		{
			FileUtil.Save(adSystemFilePath, bytes, "KilooAdSystemInterstitialStatsSecret");
		}
		catch (IOException ex)
		{
			AdSystemLog.LogError("Could not save interstitial stats data: " + ex.Message);
		}
	}

	private static IDictionary<string, object> LoadAdSystemDataFile()
	{
		string adSystemFilePath = GetAdSystemFilePath(GetAdSystemFolderPath());
		byte[] bytes;
		try
		{
			bytes = FileUtil.Load(adSystemFilePath, "KilooAdSystemInterstitialStatsSecret");
		}
		catch (FileNotFoundException)
		{
			return null;
		}
		catch (Exception ex2)
		{
			SDebug.LogError("Could not load interstitial stats " + ex2.Message);
			return null;
		}
		string json = Encoding.UTF8.GetString(bytes);
		return KilooJson.Deserialize(json) as IDictionary<string, object>;
	}

	private static bool MigrateSyboAnalyticsSave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadBool(mainPlayerData, Key.HasLoggedSyboUserId, out var boolToSet))
			{
				SyboAnalyticsSave.kilooAnalyticsHasLoggedSyboUserId = boolToSet;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsUserId, out var stringToSet))
			{
				SyboAnalyticsSave.syboAnalyticsUserId = stringToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.SyboAnalyticsSessionCounter, out var intToSet))
			{
				SyboAnalyticsSave.syboAnalyticsCurrentSession = intToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.SyboAnalyticsGamesSinceFlush, out var intToSet2))
			{
				SyboAnalyticsSave.syboAnalyticsGamesSinceFlush = intToSet2;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsPreviousClientVersion, out var stringToSet2))
			{
				SyboAnalyticsSave.syboAnalyticsPreviousClientVersion = stringToSet2;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsOperatingSystem, out var stringToSet3))
			{
				SyboAnalyticsSave.syboAnalyticsOperatingSystem = stringToSet3;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsDeviceID, out var stringToSet4))
			{
				SyboAnalyticsSave.syboAnalyticsDeviceID = stringToSet4;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsOSLanguage, out var stringToSet5))
			{
				SyboAnalyticsSave.syboAnalyticsOSLanguage = stringToSet5;
			}
			if (TryLoadEnum<DeviceInfo.PerformanceLevel>(mainPlayerData, Key.SyboAnalyticsPerformanceLevel, out var enumToSet))
			{
				SyboAnalyticsSave.syboAnalyticsPerformanceLevel = enumToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.SyboAnalyticsGpuScore, out var intToSet3))
			{
				SyboAnalyticsSave.syboAnalyticsGpuScore = intToSet3;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsOnlineSettingsLocale, out var stringToSet6))
			{
				SyboAnalyticsSave.syboAnalyticsOnlineSettingsLocale = stringToSet6;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsPendingOfferRewardToOfferIdMap, out var stringToSet7))
			{
				Dictionary<string, string> syboAnalyticsPendingOfferRewardToOfferIdMap = Globals.convertStringToStringStringDictionary(stringToSet7);
				SyboAnalyticsSave.syboAnalyticsPendingOfferRewardToOfferIdMap = syboAnalyticsPendingOfferRewardToOfferIdMap;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsUnhandledAbTestData, out var stringToSet8))
			{
				Dictionary<string, string> syboAnalyticsUnhandledAbTestData = Globals.convertStringToStringStringDictionary(stringToSet8);
				SyboAnalyticsSave.syboAnalyticsUnhandledAbTestData = syboAnalyticsUnhandledAbTestData;
			}
			if (TryLoadEnum<SampleState>(mainPlayerData, Key.SyboAnalyticsSampleState, out var enumToSet2))
			{
				SyboAnalyticsSave.syboAnalyticsSampleState = enumToSet2;
			}
			if (TryLoadString(mainPlayerData, Key.SyboAnalyticsAggregatedData, out var stringToSet9))
			{
				SyboAnalyticsSave.syboAggregatedDataString = stringToSet9;
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool MigrateEventsSave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadBool(mainPlayerData, Key.HasLoggedWeeklyHuntPeriod1, out var boolToSet))
			{
				EventsSave.hasLoggedWeeklyHuntPeriod[0] = boolToSet;
			}
			if (TryLoadBool(mainPlayerData, Key.HasLoggedWeeklyHuntPeriod2, out var boolToSet2))
			{
				EventsSave.hasLoggedWeeklyHuntPeriod[1] = boolToSet2;
			}
			if (TryLoadBool(mainPlayerData, Key.HasLoggedWeeklyHuntPeriod3, out var boolToSet3))
			{
				EventsSave.hasLoggedWeeklyHuntPeriod[2] = boolToSet3;
			}
			if (TryLoadBool(mainPlayerData, Key.HasLoggedWeeklyHuntPeriod4, out var boolToSet4))
			{
				EventsSave.hasLoggedWeeklyHuntPeriod[3] = boolToSet4;
			}
			if (TryLoadString(mainPlayerData, Key.WeeklyHuntProgressVersion, out var stringToSet))
			{
				EventsSave.weeklyHuntProgressVersion = stringToSet;
			}
			if (TryLoadString(mainPlayerData, Key.SpecialChallengeData, out var stringToSet2))
			{
				string[] array = stringToSet2.Split(',');
				if (array.ValidIndex(0))
				{
					EventsSave.weeklyHuntProgressData.huntStartVersion = array[0];
				}
				for (int i = 1; i <= 4; i++)
				{
					if (array.ValidIndex(i) && int.TryParse(array[i], out var result))
					{
						EventsSave.weeklyHuntProgressData.tokenProgress[i - 1] = result;
					}
				}
			}
			if (TryLoadString(mainPlayerData, Key.DailyWord, out var stringToSet3))
			{
				EventsSave.wordHuntDailyWord = stringToSet3;
			}
			if (TryLoadBool(mainPlayerData, Key.DailyWordPayedOut, out var boolToSet5))
			{
				EventsSave.wordHuntPayedOut = boolToSet5;
			}
			if (TryLoadInt(mainPlayerData, Key.DailyWordUnlockMask, out var intToSet))
			{
				EventsSave.wordHuntUnlockedMask = intToSet;
			}
			if (TryLoadDateTime(mainPlayerData, Key.DailyWordExpireTime, out var valueToSet))
			{
				EventsSave.wordHuntExpireTime = valueToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.DailyWordInRow, out var intToSet2))
			{
				EventsSave.wordHuntWordsInRow = intToSet2;
			}
			if (TryLoadInt(mainPlayerData, Key.DailyWordLastPayoutDayOfYear, out var intToSet3))
			{
				EventsSave.wordHuntLastPayoutDayOfYear = intToSet3;
			}
			if (TryLoadString(mainPlayerData, Key.CharacterNameEventData, out var stringToSet4))
			{
				EventsSave.characterNameEventData = DeserializeCharacterEventDataFromJson(stringToSet4);
			}
			if (TryLoadLong(mainPlayerData, Key.LastEventID, out var longToSet))
			{
				EventsSave.lastEventID = longToSet;
			}
			if (TryLoadLong(mainPlayerData, Key.LastTimeAnEventHasBeenTried, out var longToSet2))
			{
				EventsSave.lastTimeAnEventHasBeenTried = longToSet2;
			}
			if (TryLoadLong(mainPlayerData, Key.LastTimeAnEventWasStarted, out var longToSet3))
			{
				EventsSave.lastTimeAnEventWasStarted = longToSet3;
			}
			if (TryLoadLong(mainPlayerData, Key.LastTimeEventPopupHasBeenForcedShown, out var longToSet4))
			{
				EventsSave.lastTimeEventPopupWasForceShown = longToSet4;
			}
			if (TryLoadString(mainPlayerData, Key.LastEventItemID, out var stringToSet5))
			{
				EventsSave.lastEventItemID = stringToSet5;
			}
			if (TryLoadBool(mainPlayerData, Key.ShouldRestoreAfterEvent, out var boolToSet6))
			{
				EventsSave.shouldRestoreItemAfterEvent = boolToSet6;
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool InterstitialStatsTryGetValueNotNull(IDictionary<string, object> dict, string key, out object obj)
	{
		_lastInterstitialStatsKey = key;
		return dict.TryGetValueNotNull(key, out obj);
	}

	private static bool MigrateAdsSave(Dictionary<Key, string> mainPlayerData, IDictionary<string, object> adSystemData)
	{
		try
		{
			if (TryLoadBool(mainPlayerData, Key.BehavioralAdsAllowed, out var boolToSet))
			{
				AdsSave.behavioralAdsAllowed = boolToSet;
			}
			if (TryLoadString(mainPlayerData, Key.LastShowedProviders, out var stringToSet) && KilooJson.Deserialize(stringToSet) is Dictionary<string, object> dictionary)
			{
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					AdLocationInfo adLocationInfoFromID = AdSystemManager.GetAdLocationInfoFromID(item.Key);
					AdsSave.lastShowedProviders[adLocationInfoFromID.id] = item.Value.ToString();
				}
			}
			if (TryLoadDateTime(mainPlayerData, Key.ScheduledFirstInterstitialShowTime, out var valueToSet))
			{
				AdsSave.scheduledFirstInterstitialShowTime = valueToSet;
			}
			if (adSystemData != null)
			{
				InterstitialStats interstitialStats = new InterstitialStats();
				if (InterstitialStatsTryGetValueNotNull(adSystemData, "seenThisHour", out var obj))
				{
					interstitialStats.seenThisHour = Convert.ToInt32(obj);
				}
				if (InterstitialStatsTryGetValueNotNull(adSystemData, "seenThisDay", out obj))
				{
					interstitialStats.seenThisDay = Convert.ToInt32(obj);
				}
				if (InterstitialStatsTryGetValueNotNull(adSystemData, "hasSeenFirstInterstitial", out obj))
				{
					interstitialStats.hasSeenFirstInterstitial = Convert.ToBoolean(obj);
				}
				if (InterstitialStatsTryGetValueNotNull(adSystemData, "currentDay", out obj))
				{
					interstitialStats.currentDay = Convert.ToInt32(obj);
				}
				if (InterstitialStatsTryGetValueNotNull(adSystemData, "currentHour", out obj))
				{
					interstitialStats.currentHour = Convert.ToInt32(obj);
				}
				if (InterstitialStatsTryGetValueNotNull(adSystemData, "listVersionForID", out obj))
				{
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
					string text = obj.ToString();
					string[] array = text.Split(',');
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							string[] array3 = text2.Split(':');
							if (array3.Length == 2)
							{
								dictionary2.Add(array3[0], array3[1]);
							}
						}
					}
					interstitialStats.listVersionForID = dictionary2;
				}
				AdsSave.interstitialStats = interstitialStats;
			}
			return true;
		}
		catch (Exception exception)
		{
			string lastKey = _lastInterstitialStatsKey ?? _lastKeyRead.ToString();
			LogMigrationFail(lastKey, exception);
			return false;
		}
	}

	private static bool MigratePreferencesSave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadBool(mainPlayerData, Key.NeverAskForRating, out var boolToSet))
			{
				PreferencesSave.neverAskForRating = boolToSet;
			}
			if (TryLoadString(mainPlayerData, Key.Language, out var stringToSet))
			{
				PreferencesSave.language = stringToSet;
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool MigrateAnalyticsSave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadString(mainPlayerData, Key.FlurryUserId, out var stringToSet))
			{
				Flurry.flurryUserId = stringToSet;
				Flurry.SetUserId();
			}
			if (TryLoadBool(mainPlayerData, Key.HasLoggedStaticData, out var boolToSet))
			{
				AnalyticsSave.hasLoggedStaticData = boolToSet;
			}
			if (TryLoadString(mainPlayerData, Key.LastLoggedDailyData, out var stringToSet2))
			{
				DateTime lastLoggedDailyData = Utils.StringToDateTime(stringToSet2, DateTime.MinValue).ToUniversalTime();
				AnalyticsSave.lastLoggedDailyData = lastLoggedDailyData;
			}
			if (TryLoadFloat(mainPlayerData, Key.AnalyticsSamplingKey, out var floatToSet))
			{
				AnalyticsSave.analyticsSamplingKey = floatToSet;
			}
			if (TryLoadString(mainPlayerData, Key.ABTestLastDailyEventsDate, out var stringToSet3))
			{
				AnalyticsSave.abTestLastDailyEventsReportDate = Utils.StringToDateTime(stringToSet3, DateTime.MinValue).ToUniversalTime();
			}
			if (TryLoadString(mainPlayerData, Key.ABTestPlayerSeed, out var stringToSet4))
			{
				AnalyticsSave.abTestPlayerSeed = stringToSet4;
			}
			if (TryLoadString(mainPlayerData, Key.ABTestTagData, out var stringToSet5))
			{
				AnalyticsSave.abTestTagData = stringToSet5;
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool MigrateOnboardingSave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadInt(mainPlayerData, Key.LastShownTopRunIntroPopupVersionNumber, out var intToSet))
			{
				OnboardingSave.lastShownTopRunIntroPopupVersionNumber = intToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.NumberOfRunsSinceLastGuideline, out var intToSet2))
			{
				OnboardingSave.numberOfRunsSinceLastGuideline = intToSet2;
			}
			if (TryLoadBool(mainPlayerData, Key.AllowSellHeadstartDuringRun, out var boolToSet))
			{
				OnboardingSave.allowSellHeadstartDuringRun = boolToSet;
			}
			if (TryLoadBool(mainPlayerData, Key.AllowSellScoreboosterDuringRun, out var boolToSet2))
			{
				OnboardingSave.allowSellScoreboosterDuringRun = boolToSet2;
			}
			if (TryLoadBool(mainPlayerData, Key.HasCollectedFromFriends, out var boolToSet3))
			{
				OnboardingSave.hasCollectedFromFriends = boolToSet3;
			}
			if (TryLoadBool(mainPlayerData, Key.HasShownCollectPopup, out var boolToSet4))
			{
				OnboardingSave.hasShownCollectPopup = boolToSet4;
			}
			if (TryLoadBool(mainPlayerData, Key.HasShownFacebookPopup, out var boolToSet5))
			{
				OnboardingSave.hasShownFacebookPopup = boolToSet5;
			}
			if (TryLoadBool(mainPlayerData, Key.HasShownHoverboardPopup, out var boolToSet6))
			{
				OnboardingSave.hasShownHoverboardPopup = boolToSet6;
			}
			if (TryLoadBool(mainPlayerData, Key.HasShownMission1Popup, out var boolToSet7))
			{
				OnboardingSave.hasShownMissionIntroPopup = boolToSet7;
			}
			if (TryLoadBool(mainPlayerData, Key.HasShownMission2Popup, out var boolToSet8))
			{
				OnboardingSave.hasShownEndGameMissionPopup = boolToSet8;
			}
			if (TryLoadBool(mainPlayerData, Key.TutorialCompleted, out var boolToSet9))
			{
				OnboardingSave.isTutorialCompleted = boolToSet9;
			}
			if (TryLoadBool(mainPlayerData, Key.ShouldShowCollectPopup, out var boolToSet10))
			{
				OnboardingSave.shouldShowCollectPopup = boolToSet10;
			}
			if (TryLoadBool(mainPlayerData, Key.ShouldShowFacebookPopup, out var boolToSet11))
			{
				OnboardingSave.shouldShowFacebookPopup = boolToSet11;
			}
			if (TryLoadBool(mainPlayerData, Key.ShouldShowHoverboardPopup, out var boolToSet12))
			{
				OnboardingSave.shouldShowHoverboardPopup = boolToSet12;
			}
			if (TryLoadBool(mainPlayerData, Key.ShouldShowMission1Popup, out var boolToSet13))
			{
				OnboardingSave.shouldShowMissionIntroPopup = boolToSet13;
			}
			if (TryLoadBool(mainPlayerData, Key.ShouldShowMission2Popup, out var boolToSet14))
			{
				OnboardingSave.shouldShowEndGameMissionPopup = boolToSet14;
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	public static bool CanMigrate()
	{
		if (UserSave.hasMigratedPlayerInfo)
		{
			return false;
		}
		if (!TryGetLoadPaths(out var path))
		{
			return false;
		}
		FileInfo[] filesForPath = FileUtil.GetFilesForPath(path);
		if (filesForPath == null || filesForPath.Length == 0)
		{
			return false;
		}
		return true;
	}

	private static bool TryGetLoadPaths(out string path)
	{
		string userDataPath = DeviceUtility.GetUserDataPath();
		if (string.IsNullOrEmpty(userDataPath))
		{
			path = null;
			return false;
		}
		path = userDataPath + "/playerdata";
		return true;
	}

	private static string GetSavePath()
	{
		return DeviceUtility.GetUserDataPath() + "/playerdata";
	}

	private static bool MigrateUserSave(Dictionary<Key, string> mainPlayerData)
	{
		try
		{
			if (TryLoadInt(mainPlayerData, Key.InAppNonconsumablePurchaseCount, out var intToSet))
			{
				UserSave.inAppNonConsumablePurchaseCount = intToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.InAppConsumablePurchaseCount, out var intToSet2))
			{
				UserSave.inAppConsumablePurchaseCount = intToSet2;
			}
			if (TryLoadInt(mainPlayerData, Key.InAppRestoredPurchaseCount, out var intToSet3))
			{
				UserSave.inAppRestoredPurchaseCount = intToSet3;
			}
			if (TryLoadInt(mainPlayerData, Key.InAppPurchaseCount, out var intToSet4))
			{
				UserSave.inAppLegacyPurchaseCount = intToSet4;
			}
			if (TryLoadString(mainPlayerData, Key.HistoryOfInapp, out var stringToSet))
			{
				UserSave.inAppPurchaseHistory = Globals.convertStringToStringStringDictionary(stringToSet);
			}
			if (TryLoadBool(mainPlayerData, Key.IsFreshInstall, out var boolToSet))
			{
				UserSave.isFreshInstall = boolToSet;
			}
			if (TryLoadString(mainPlayerData, Key.LastPlayDate, out var stringToSet2) && DateTime.TryParse(stringToSet2, out var result))
			{
				UserSave.lastPlayDate = result;
			}
			if (TryLoadString(mainPlayerData, Key.LastQuitDate, out var stringToSet3) && DateTime.TryParse(stringToSet3, out var result2))
			{
				UserSave.lastQuitDate = result2;
			}
			if (TryLoadString(mainPlayerData, Key.LastDailyOnlineLog, out var stringToSet4) && DateTime.TryParse(stringToSet4, out var result3))
			{
				UserSave.lastDailyOnlineLog = result3;
			}
			if (TryLoadEnum<InAppData.BundleTier>(mainPlayerData, Key.LastPurchasedBundle, out var enumToSet))
			{
				UserSave.lastPurchasedBundle = enumToSet;
			}
			if (TryLoadInt(mainPlayerData, Key.LatestAvailableBundlesVersion, out var intToSet5))
			{
				UserSave.latestAvailableAssetBundlesVersion = intToSet5;
			}
			if (TryLoadBool(mainPlayerData, Key.AllBundlesDownloaded, out var boolToSet2))
			{
				UserSave.allAssetBundlesDownloaded = boolToSet2;
			}
			if (TryLoadBool(mainPlayerData, Key.PayBonusFacebook, out var boolToSet3) && boolToSet3)
			{
				TryLoadBool(mainPlayerData, Key.PayBonusFacebookPack, out var _);
				TryLoadBool(mainPlayerData, Key.PayBonusFacebookCharacter, out var boolToSet5);
				if (!boolToSet5)
				{
					Characters.CharacterId characterId = Characters.CharacterId.facebookRewardCharacter01;
					int num = 0;
					Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(characterId, num);
					bool flag = outfitForCharacter != null;
					CelebrationReward celebrationReward = new CelebrationReward();
					celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
					celebrationReward.rewardType = CelebrationRewardType.character;
					celebrationReward.characterId = characterId;
					celebrationReward.characterOutfitIndex = num;
					CelebrationReward reward = celebrationReward;
					RewardManager.AddRewardToUnlock(reward);
					if (flag)
					{
						CharacterManager.UnlockOutfit(characterId, num);
					}
					else
					{
						CharacterManager.UnlockCharacter(characterId);
					}
				}
				UserSave.hasPaidOutFacebookReward = true;
			}
			if (TryLoadString(mainPlayerData, Key.CharacterThemesSeen, out var stringToSet5))
			{
				Dictionary<Characters.CharacterId, int[]> dictionary = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterId>(stringToSet5);
				Dictionary<Characters.CharacterId, HashSet<int>> dictionary2 = new Dictionary<Characters.CharacterId, HashSet<int>>(dictionary.Count);
				foreach (KeyValuePair<Characters.CharacterId, int[]> item in dictionary)
				{
					dictionary2[item.Key] = new HashSet<int>(item.Value);
				}
				UserSave.characterOutfitsSeen = dictionary2;
			}
			if (TryLoadString(mainPlayerData, Key.CollectCoinsDummyData, out var stringToSet6))
			{
				CollectCoinsDummy[] array = MigrationSave.ParseDummyData(stringToSet6);
				if (array != null)
				{
					UserSave.collectCoinsDummyData = array;
				}
			}
			if (TryLoadBool(mainPlayerData, Key.HasSeenFrontScreenFirstTime, out var boolToSet6))
			{
				UserSave.hasSeenFrontScreenFirstTime = boolToSet6;
			}
			if (TryLoadBool(mainPlayerData, Key.HasMadeOneValidPurchase, out var boolToSet7))
			{
				UserSave.hasMadeOneValidPurchase = boolToSet7;
			}
			if (TryLoadString(mainPlayerData, Key.FirstInstalledVersion, out var stringToSet7))
			{
				UserSave.firstInstalledVersion = stringToSet7;
			}
			if (TryLoadDateTime(mainPlayerData, Key.FirstInstallDate, out var valueToSet))
			{
				UserSave.firstInstallDate = valueToSet.ToUniversalTime();
			}
			if (TryLoadBool(mainPlayerData, Key.DoubleCoins, out var boolToSet8))
			{
				UserSave.hasDoubleCoinsUpgrade = boolToSet8;
			}
			if (TryLoadBool(mainPlayerData, Key.RemoveAds, out var boolToSet9))
			{
				UserSave.hasAdRemovalUpgrade = boolToSet9;
			}
			if (TryLoadString(mainPlayerData, Key.HasCharacterBeenSeen, out var stringToSet8))
			{
				UserSave.hasCharacterBeenSeen = Globals.convertStringToEnumBoolDictionary<Characters.CharacterId>(stringToSet8);
			}
			if (TryLoadString(mainPlayerData, Key.HasHoverboardsBeenSeen, out var stringToSet9))
			{
				UserSave.hasBoardBeenSeen = Globals.convertStringToEnumBoolDictionary<Boards.BoardId>(stringToSet9);
			}
			if (TryLoadString(mainPlayerData, Key.EarnCurrencyData, out var stringToSet10))
			{
				UserSave.earnCurrencyData = MigrationSave.MigrateEarnCurrencyData(stringToSet10);
			}
			if (TryLoadString(mainPlayerData, Key.LastVersionRated, out var stringToSet11))
			{
				UserSave.lastVersionRated = stringToSet11;
			}
			if (TryLoadDateTime(mainPlayerData, Key.LastTimeAVideoForKeysWasSeen, out var valueToSet2))
			{
				UserSave.lastTimeAVideoForKeysWasSeen = DateTime.SpecifyKind(valueToSet2, DateTimeKind.Utc);
			}
			if (TryLoadDateTime(mainPlayerData, Key.LastKnownServerTime, out var valueToSet3))
			{
				UserSave.lastKnownServerTime = DateTime.SpecifyKind(valueToSet3, DateTimeKind.Utc);
			}
			if (TryLoadDateTime(mainPlayerData, Key.LastTimeAppWasUpdatedOrInstalled, out var valueToSet4))
			{
				UserSave.lastTimeAppWasUpdatedOrInstalled = valueToSet4.ToUniversalTime();
			}
			if (TryLoadInt(mainPlayerData, Key.BreadCrumbsShownOnFrontPage, out var intToSet6))
			{
				UserSave.numberOfBreadCrumbsShownOnFrontPage = intToSet6;
			}
			if (TryLoadInt(mainPlayerData, Key.AgeRestrictionInputVersion, out var intToSet7))
			{
				UserSave.ageRestrictionInputVersion = intToSet7;
			}
			if (TryLoadInt(mainPlayerData, Key.AgeRestrictionInputMonth, out var intToSet8))
			{
				UserSave.ageRestrictionInputMonth = intToSet8;
			}
			if (TryLoadInt(mainPlayerData, Key.AgeRestrictionInputYear, out var intToSet9))
			{
				UserSave.ageRestrictionInputYear = intToSet9;
			}
			if (TryLoadDateTime(mainPlayerData, Key.WelcomePackStartTime, out var valueToSet5))
			{
				UserSave.welcomePackStartTime = valueToSet5;
			}
			return true;
		}
		catch (Exception exception)
		{
			LogMigrationFail(_lastKeyRead.ToString(), exception);
			return false;
		}
	}

	private static bool TryLoadString(Dictionary<Key, string> dict, Key key, out string stringToSet)
	{
		_lastKeyRead = key;
		if (dict.TryGetValue(key, out var value))
		{
			stringToSet = value;
			return true;
		}
		stringToSet = null;
		return false;
	}

	private static bool TryLoadInt(Dictionary<Key, string> dict, Key key, out int intToSet)
	{
		_lastKeyRead = key;
		if (dict.TryGetValue(key, out var value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			intToSet = result;
			return true;
		}
		intToSet = 0;
		return false;
	}

	private static bool TryLoadFloat(Dictionary<Key, string> dict, Key key, out float floatToSet)
	{
		_lastKeyRead = key;
		if (dict.TryGetValue(key, out var value) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			floatToSet = result;
			return true;
		}
		floatToSet = 0f;
		return false;
	}

	private static bool TryLoadLong(Dictionary<Key, string> dict, Key key, out long longToSet)
	{
		_lastKeyRead = key;
		if (dict.TryGetValue(key, out var value) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			longToSet = result;
			return true;
		}
		longToSet = 0L;
		return false;
	}

	private static bool TryLoadBool(Dictionary<Key, string> dict, Key key, out bool boolToSet)
	{
		_lastKeyRead = key;
		if (dict.TryGetValue(key, out var value) && bool.TryParse(value, out var result))
		{
			boolToSet = result;
			return true;
		}
		boolToSet = false;
		return false;
	}

	private static bool TryLoadDateTime(Dictionary<Key, string> dict, Key key, out DateTime valueToSet)
	{
		_lastKeyRead = key;
		bool result = false;
		valueToSet = default;
		if (dict.TryGetValue(key, out var value))
		{
			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result2))
			{
				valueToSet = result2;
				result = true;
			}
			if (DateTime.TryParse(value, out result2))
			{
				valueToSet = result2;
				result = true;
			}
		}
		return result;
	}

	private static bool TryLoadEnum<T>(Dictionary<Key, string> dict, Key key, out T enumToSet)
	{
		_lastKeyRead = key;
		enumToSet = default;
		if (dict.TryGetValue(key, out var value))
		{
			try
			{
				enumToSet = (T)Enum.Parse(typeof(T), value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		return false;
	}

	private static int[] LoadIntArray(Dictionary<Key, string> dict, Key key, int[] defaultValue)
	{
		_lastKeyRead = key;
		if (dict.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
		{
			return Globals.convertAllStringToInt(value.Split(','));
		}
		return defaultValue;
	}

	private static bool[] LoadBoolArray(Dictionary<Key, string> dict, Key key, bool[] defaultValue)
	{
		_lastKeyRead = key;
		if (dict.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
		{
			return Globals.convertAllStringToBool(value.Split(','));
		}
		return defaultValue;
	}

	private static CharacterNameEventData DeserializeCharacterEventDataFromJson(string characterNameData)
	{
		CharacterNameEventData characterNameEventData = new CharacterNameEventData();
		KilooSerializer serializer = new KilooSerializer();
		IDictionary<string, object> dictionary = serializer.FromJson<IDictionary<string, object>>(characterNameData);
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("charactersList") && dictionary["charactersList"] != null && dictionary["charactersList"] is List<object> { Count: >0 } list)
			{
				List<Characters.CharacterId> list2 = new List<Characters.CharacterId>();
				for (int i = 0; i < list.Count; i++)
				{
					int num = Convert.ToInt32(list[i]);
					Characters.CharacterId item = (Characters.CharacterId)num;
					list2.Add(item);
				}
				if (list2.Count > 0)
				{
					characterNameEventData.charactersList = list2;
				}
			}
			if (dictionary.ContainsKey("currentCharacter") && dictionary["currentCharacter"] != null)
			{
				int currentCharacter = Convert.ToInt32(dictionary["currentCharacter"]);
				characterNameEventData.currentCharacter = (Characters.CharacterId)currentCharacter;
			}
			bool flag = false;
			List<string> list3 = new List<string>();
			if (dictionary.ContainsKey("categoryWordsList") && dictionary["categoryWordsList"] != null && dictionary["categoryWordsList"] is List<object> { Count: >0 } list4)
			{
				for (int j = 0; j < list4.Count; j++)
				{
					string text = list4[j].ToString();
					if (Strings.IsMapped(text))
					{
						list3.Add(text);
						continue;
					}
					SDebug.LogError("Invalid CharacterNameEvent progress StringID, using character name instead");
					list3.Add(string.Empty);
					int num2 = characterNameEventData.charactersList.IndexOf(characterNameEventData.currentCharacter);
					if (num2 == list3.Count - 1)
					{
						flag = true;
					}
				}
			}
			characterNameEventData.categoryWordsList = ((list3.Count <= 0) ? null : list3);
			SSEventCategory eventCategory = SSEventCategory.None;
			if (dictionary.ContainsKey("eventCategory") && dictionary["eventCategory"] != null)
			{
				int num3 = Convert.ToInt32(dictionary["eventCategory"]);
				if (Enum.IsDefined(typeof(SSEventCategory), num3))
				{
					eventCategory = (SSEventCategory)num3;
				}
			}
			characterNameEventData.eventCategory = eventCategory;
			if (flag)
			{
				characterNameEventData.collectedLetterIndex = -1;
			}
			else if (dictionary.ContainsKey("collectedLetterIndex") && dictionary["collectedLetterIndex"] != null)
			{
				characterNameEventData.collectedLetterIndex = Convert.ToInt32(dictionary["collectedLetterIndex"]);
			}
			if (dictionary.ContainsKey("completedCharacters") && dictionary["completedCharacters"] != null)
			{
				characterNameEventData.completedCharacters = Convert.ToInt32(dictionary["completedCharacters"]);
			}
			if (dictionary.ContainsKey("skippedCharacters") && dictionary["skippedCharacters"] != null)
			{
				characterNameEventData.skippedCharacters = Convert.ToInt32(dictionary["skippedCharacters"]);
			}
			if (dictionary.ContainsKey("wasLastCharacterCompleted") && dictionary["wasLastCharacterCompleted"] != null)
			{
				characterNameEventData.wasLastCharacterCompleted = Convert.ToBoolean(dictionary["wasLastCharacterCompleted"]);
			}
			if (dictionary.ContainsKey("eventID") && dictionary["eventID"] != null)
			{
				characterNameEventData.eventID = Convert.ToInt64(dictionary["eventID"]);
			}
		}
		return characterNameEventData;
	}

	private static Dictionary<UpgradeType, int> ConvertLegacyPowerupsToUpgrades(Dictionary<PowerupTypeLegacy, int> legacyPowerups)
	{
		Dictionary<UpgradeType, int> dictionary = new Dictionary<UpgradeType, int>();
		foreach (KeyValuePair<PowerupTypeLegacy, int> legacyPowerup in legacyPowerups)
		{
			PowerupTypeLegacy key = legacyPowerup.Key;
			int value = legacyPowerup.Value;
			UpgradeType upgradeType = UpgradeType._notset;
			if (key == PowerupTypeLegacy.headstart500)
			{
				upgradeType = UpgradeType.headstart2000;
			}
			else if (Enum.IsDefined(typeof(UpgradeType), key.ToString()))
			{
				upgradeType = (UpgradeType)Enum.Parse(typeof(UpgradeType), key.ToString());
			}
			else
			{
				Flurry.LogEventWithAParameter("Migration unhandled upgrades", "Unhandled powerup", key.ToString());
			}
			if (upgradeType != UpgradeType._notset)
			{
				if (dictionary.ContainsKey(upgradeType))
				{
					dictionary[upgradeType] += value;
				}
				else
				{
					dictionary[upgradeType] = value;
				}
			}
		}
		return dictionary;
	}
}
