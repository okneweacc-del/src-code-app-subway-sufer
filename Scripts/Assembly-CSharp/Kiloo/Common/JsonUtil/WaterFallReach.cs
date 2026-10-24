using System.Linq;
using Kiloo.Common.Ads;
using UI.Screens;

namespace Kiloo.Common.JsonUtil;

public class WaterFallReach
{
	private static readonly string[] SCREENS_THAT_CAN_CACHE_A_VIDEO_AD = new string[10] { "EarnCoinsScreen", "StorePopup", "ConsumablesPopup", "CharacterNameEventPopup", "GameOverScreen", "FrontScreen", "SaveMePopup", "ChallengesScreen", "CelebrationPopup", "PauseUI" };

	private bool _hasCachedSuccesFullyOnCurrentScreen;

	private bool _isFirstScreen;

	private int _currentScreenCacheFailCount;

	private string _previousScreenName;

	public WaterFallReach()
	{
		UIScreenController.OnChangedScreen += OnChangedScreen;
		AdSystemManager.onCachedVideo += OnCachedVideo;
		AdSystemManager.onFailedToCacheVideo += OnFailedToCacheVideo;
		_isFirstScreen = true;
	}

	private void OnFailedToCacheVideo(AdLocationInfo adLocationInfo)
	{
		_currentScreenCacheFailCount++;
	}

	private void OnCachedVideo(AdLocationInfo adLocationInfo, bool wasAlreadyCached)
	{
		_hasCachedSuccesFullyOnCurrentScreen = true;
	}

	private void OnChangedScreen(string screenName)
	{
		LogWaterFallReach();
		Reset();
		_previousScreenName = screenName;
	}

	private void LogWaterFallReach()
	{
		if (!_isFirstScreen && !_hasCachedSuccesFullyOnCurrentScreen && CanCacheAVideoAdOnScreen(_previousScreenName) && _currentScreenCacheFailCount > 0)
		{
			AdTrackingSystemLogger.LogWaterFallReach(_previousScreenName, _currentScreenCacheFailCount);
		}
	}

	private void Reset()
	{
		_hasCachedSuccesFullyOnCurrentScreen = false;
		_isFirstScreen = false;
		_currentScreenCacheFailCount = 0;
	}

	private bool CanCacheAVideoAdOnScreen(string screenName)
	{
		if (SCREENS_THAT_CAN_CACHE_A_VIDEO_AD.Contains(screenName))
		{
			if (screenName == "ChallengesScreen")
			{
				return IsOnMissionTab();
			}
			return true;
		}
		return false;
	}

	private bool IsOnMissionTab()
	{
		if (ChallengesScreen.GetPageIndex() == 1)
		{
			return true;
		}
		return false;
	}
}
