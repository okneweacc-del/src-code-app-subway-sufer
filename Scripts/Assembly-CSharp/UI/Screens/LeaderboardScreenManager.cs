using System;
using Info;

namespace UI.Screens;

public class LeaderboardScreenManager
{
	public enum LeaderboardScreenState
	{
		Offline = 0,
		Facebook = 1,
		GameCenter = 2,
		FacebookAndGamecenter = 3,
		AgeRestricted = 4
	}

	private static LeaderboardScreenManager _instance;

	public Action<LeaderboardScreenState> OnLeaderboardScreenStateChangedEvent;

	private LeaderboardScreenState _screenState;

	public LeaderboardScreenState screenState
	{
		get
		{
			UpdateScreenState();
			return _screenState;
		}
		private set
		{
			if (_screenState != value)
			{
				_screenState = value;
				ScreenStateChanged(_screenState);
			}
		}
	}

	public static LeaderboardScreenManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LeaderboardScreenManager();
			}
			return _instance;
		}
	}

	public static bool isInitialized => _instance != null;

	private void ScreenStateChanged(LeaderboardScreenState newState)
	{
		OnLeaderboardScreenStateChangedEvent?.Invoke(newState);
	}

	private void UpdateScreenState()
	{
		Globals.LogInState loginState = Globals.GetLoginState();
		int consolidationState = SocialManager.instance.consolidationState;
		bool flag = loginState == Globals.LogInState.Facebook || loginState == Globals.LogInState.Both;
		bool flag2 = (consolidationState & 1) == 1;
		bool flag3 = (flag && flag2) || Globals.DEBUG_USE_DEBUG_LOGIN_STATE;
		bool flag4 = loginState == Globals.LogInState.Both || loginState == Globals.LogInState.GameCenter;
		bool flag5 = (consolidationState & 2) == 2;
		bool flag6 = flag4 && flag5;
		if (AgeRestriction.isRestricted)
		{
			screenState = LeaderboardScreenState.AgeRestricted;
		}
		else if (flag3)
		{
			if (flag6)
			{
				screenState = LeaderboardScreenState.FacebookAndGamecenter;
			}
			else
			{
				screenState = LeaderboardScreenState.Facebook;
			}
		}
		else if (flag6)
		{
			screenState = LeaderboardScreenState.GameCenter;
		}
		else
		{
			screenState = LeaderboardScreenState.Offline;
		}
	}
}
