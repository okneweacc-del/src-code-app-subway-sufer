using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AppRefresh;
using Data.Migration;
using Data.Save;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Data.DebugUtil;

public static class SaveGameDebug
{
	private static readonly string[] _saveFiles = new string[14]
	{
		"playerdata", "playerdata.0", "playerdata.1", "playerdata.2", "playerdata.3", "playerdata.4", "toprun_manager", "toprun_playerids", "toprun_ranks", "socialdata",
		"AdSystem\\showlimits", "Save\\local", "Save\\cloud", "Save\\local_debug"
	};

	private static string _lastLoadedSave;

	private static string debugSaveDirectory => "..\\..\\tools\\OldPlayerInfoVersions".RelativeToAbsolutePath();

	private static string gameSaveDirectory => "..\\EditorCache".RelativeToAbsolutePath();

	private static string saveInfoPath => Path.Combine(debugSaveDirectory, "active.txt");

	public static string lastLoadedSave
	{
		get
		{
			if (_lastLoadedSave.IsNullOrEmpty())
			{
				if (File.Exists(saveInfoPath))
				{
					_lastLoadedSave = File.ReadAllText(saveInfoPath);
				}
				else
				{
					_lastLoadedSave = "N/A";
				}
			}
			return _lastLoadedSave;
		}
		private set
		{
			_lastLoadedSave = value;
			File.WriteAllText(saveInfoPath, value);
		}
	}

	private static string tmpPlayerInfoPath => Path.Combine(Path.GetTempPath(), "tmp_playerinfo.json");

	private static string tmpCloudPath => Path.Combine(Path.GetTempPath(), "tmp_cloud.json");

	private static string tmpLocalPath => Path.Combine(Path.GetTempPath(), "tmp_local.json");

	public static IEnumerable<string> GetPossibleSaves()
	{
		string saveDir = debugSaveDirectory;
		string[] directories = Directory.GetDirectories(saveDir);
		return directories.Select((string x) => x.Remove(0, saveDir.Length + 1));
	}

	private static void CopySaveFiles(string from, string to)
	{
		string[] saveFiles = _saveFiles;
		foreach (string path in saveFiles)
		{
			try
			{
				string path2 = path.UnifyDirectorySeparators();
				string sourceFileName = Path.Combine(from, path2);
				string text = Path.Combine(to, path2);
				MakeSureDirectoryExists(to, text);
				File.Copy(sourceFileName, text, overwrite: true);
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
	}

	private static void MakeSureDirectoryExists(string folderPath, string filePath)
	{
		int num = filePath.LastIndexOf(Path.DirectorySeparatorChar);
		if (num > 0)
		{
			string text = filePath.Substring(0, num);
			if (folderPath != text)
			{
				Directory.CreateDirectory(text);
			}
		}
	}

	private static void DeleteExistingSaveFiles(string folder)
	{
		string[] saveFiles = _saveFiles;
		foreach (string path in saveFiles)
		{
			try
			{
				string path2 = path.UnifyDirectorySeparators();
				File.Delete(Path.Combine(folder, path2));
			}
			catch (DirectoryNotFoundException)
			{
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
	}

	public static void SwitchSaveGame(string newSave)
	{
		string text = Path.Combine(debugSaveDirectory, newSave);
		string text2 = gameSaveDirectory;
		DeleteExistingSaveFiles(text2);
		CopySaveFiles(text, text2);
		lastLoadedSave = newSave;
		if (Application.isPlaying)
		{
			AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
		}
	}

	public static void StoreSaveGame(string saveFolder)
	{
		string text = gameSaveDirectory;
		string text2 = Path.Combine(debugSaveDirectory, saveFolder);
		DeleteExistingSaveFiles(text2);
		CopySaveFiles(text, text2);
		Log($"Stored save [{saveFolder}]");
	}

	public static void OpenDebugSaveDirectory()
	{
		PathHandling.OpenPathInExplorer(debugSaveDirectory);
	}

	public static void OpenGameSaveDirectory()
	{
		PathHandling.OpenPathInExplorer(gameSaveDirectory);
	}

	private static void Log(string msg)
	{
	}

	public static void ExportPlayerInfoToExternal()
	{
		string userDataPath = DeviceUtility.GetUserDataPath();
		string androidExternalSavePath = DeviceUtility.GetAndroidExternalSavePath();
		try
		{
			DeleteExistingSaveFiles(androidExternalSavePath);
			CopySaveFiles(userDataPath, androidExternalSavePath);
		}
		catch (Exception ex)
		{
			SDebug.LogError(ex.ToString());
		}
	}

	public static void ImportPlayerInfoFromExternal()
	{
		string userDataPath = DeviceUtility.GetUserDataPath();
		string androidExternalSavePath = DeviceUtility.GetAndroidExternalSavePath();
		try
		{
			DeleteExistingSaveFiles(userDataPath);
			CopySaveFiles(androidExternalSavePath, userDataPath);
		}
		catch (Exception ex)
		{
			SDebug.LogError(ex.ToString());
		}
	}

	public static void ExportToFiles()
	{
		KilooSerializer serializer = new KilooSerializer();
		string contents = string.Empty;
		try
		{
			contents = serializer.ToJson(PlayerInfoMigrator.LoadPlayerInfoData());
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception)
		{
		}
		SaveManager.Load();
		string cloud = DebugSave.debug_saveData.cloud;
		string local = DebugSave.debug_saveData.local;
		File.WriteAllText(tmpPlayerInfoPath, contents);
		File.WriteAllText(tmpCloudPath, cloud);
		File.WriteAllText(tmpLocalPath, local);
	}

	public static void ImportFromFiles()
	{
		KilooSerializer serializer = new KilooSerializer();
		PlayerInfoData playerInfoData = serializer.FromJson<PlayerInfoData>(File.ReadAllText(tmpPlayerInfoPath));
		string cloud = File.ReadAllText(tmpCloudPath);
		string local = File.ReadAllText(tmpLocalPath);
		if (playerInfoData != null)
		{
			PlayerInfoMigrator.SavePlayerInfoData(playerInfoData);
		}
		SaveDataStrings saveDataStrings = new SaveDataStrings();
		saveDataStrings.cloud = cloud;
		saveDataStrings.local = local;
		DebugSave.debug_saveData = saveDataStrings;
		SaveManager.Save();
	}

	public static bool FilesExist()
	{
		return File.Exists(tmpPlayerInfoPath) && File.Exists(tmpCloudPath) && File.Exists(tmpLocalPath);
	}

	public static void OpenFiles()
	{
		using (Process.Start(tmpPlayerInfoPath))
		{
		}
		using (Process.Start(tmpCloudPath))
		{
		}
		using (Process.Start(tmpLocalPath))
		{
		}
	}
}
