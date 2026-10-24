using System;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using Info;
using Kiloo.Common.UI;
using UI.Leaderboard;
using UI.Screens;
using UI.SlideIns;
using UnityEngine;

namespace UI;

public class GameOverLeaderboardHandler : MonoBehaviour
{
	private class LeaderboardListAdapter : ListAdapter<LeaderboardGameoverCell>
	{
		private readonly GameOverLeaderboardHandler _leaderboardScreen;

		private readonly GameObject _friendPrefab;

		private readonly GameObject _facebookPrefab;

		private readonly GameObject _playerPrefab;

		private readonly GameObject _spacePrefab;

		private const int FRIEND_PREFAB = 0;

		private const int FACEBOOK_PREFAB = 1;

		private const int PLAYER_PREFAB = 2;

		private const int SPACE_PREFAB = 3;

		public LeaderboardListAdapter(List<LeaderboardGameoverCell> contentList, GameObject newFriendPrefab, GameObject newFacebookPrefab, GameObject newPlayerPrefab, GameObject newSpacePrefab, GameOverLeaderboardHandler screenReference)
			: base(contentList)
		{
			_friendPrefab = newFriendPrefab;
			_facebookPrefab = newFacebookPrefab;
			_playerPrefab = newPlayerPrefab;
			_spacePrefab = newSpacePrefab;
			_leaderboardScreen = screenReference;
		}

		public void SetNewDataList(List<LeaderboardGameoverCell> leaderboardListT)
		{
			base.dataList = leaderboardListT;
			NotifyDataSetChanged();
		}

		public override int GetModelType(int index)
		{
			return GetItem(index).cellType switch
			{
				LeaderboardGameoverCellType.Friend => 0, 
				LeaderboardGameoverCellType.FacebookButton => 1, 
				LeaderboardGameoverCellType.Player => 2, 
				LeaderboardGameoverCellType.Space => 3, 
				_ => 0, 
			};
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			LeaderboardGameoverCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				LeaderboardGameoverCellReferences leaderboardGameoverCellReferences = new LeaderboardGameoverCellReferences();
				switch (item.cellType)
				{
				case LeaderboardGameoverCellType.Friend:
				{
					container2 = new Container(_friendPrefab);
					FriendHelperHighScore component4 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardGameoverCellReferences.friendHelper = component4;
					container2.tag = leaderboardGameoverCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardGameoverCellType.FacebookButton:
				{
					container2 = new Container(_facebookPrefab);
					FriendsLoginFBButton component3 = container2.model.GetComponent<FriendsLoginFBButton>();
					leaderboardGameoverCellReferences.facebookScript = component3;
					container2.tag = leaderboardGameoverCellReferences;
					break;
				}
				case LeaderboardGameoverCellType.Player:
				{
					container2 = new Container(_playerPrefab);
					FriendHelperHighScore component2 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardGameoverCellReferences.friendHelper = component2;
					container2.tag = leaderboardGameoverCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardGameoverCellType.Space:
					container2 = new Container(_spacePrefab);
					break;
				default:
				{
					SDebug.LogError("No cell type match for: " + item.cellType);
					container2 = new Container(_friendPrefab);
					FriendHelperHighScore component = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardGameoverCellReferences.friendHelper = component;
					container2.tag = leaderboardGameoverCellReferences;
					break;
				}
				}
			}
			LeaderboardGameoverCellReferences leaderboardGameoverCellReferences2 = (LeaderboardGameoverCellReferences)container2.tag;
			switch (item.cellType)
			{
			case LeaderboardGameoverCellType.Friend:
				leaderboardGameoverCellReferences2.friendHelper.InitFriend(item.friend, item.rank, item.useDarkColour);
				item.OnUpdateCell = leaderboardGameoverCellReferences2.friendHelper.Refresh;
				break;
			case LeaderboardGameoverCellType.FacebookButton:
				leaderboardGameoverCellReferences2.facebookScript.Init(_leaderboardScreen.FacebookClicked, leaderboardScreen: true, item.noFriends, isGameOverScreen: true);
				break;
			case LeaderboardGameoverCellType.Player:
				leaderboardGameoverCellReferences2.friendHelper.InitLocalUser(item.rank, item.useDarkColour, item.isPlayerRanked);
				item.OnUpdateCell = leaderboardGameoverCellReferences2.friendHelper.Refresh;
				break;
			default:
				SDebug.LogError("No cell type match for: " + item.cellType);
				break;
			case LeaderboardGameoverCellType.Space:
				break;
			}
			return container2;
		}
	}

	public GameObject FriendPrefab;

	public GameObject FacebookLoginPrefab;

	public GameObject SpacePrefab;

	[SerializeField]
	private ScrollView _scrollView;

	[SerializeField]
	private UIPanel _scrollPanel;

	private readonly List<LeaderboardGameoverCell> _contentList = new List<LeaderboardGameoverCell>();

	private LeaderboardListAdapter _adapter;

	private const int GC_FRIENDS_THRESHOLD = 4;

	private const int POSITION_OFFSET_IN_LIST = 2;

	private bool _facebookLoginRunning;

	private void Start()
	{
		if (_contentList != null)
		{
			_adapter = new LeaderboardListAdapter(_contentList, FriendPrefab, FacebookLoginPrefab, FriendPrefab, SpacePrefab, this);
		}
		SocialManager.instance.UpdateFriendScores((bool didLoadFriends) =>
		{
			ReloadFriends();
		});
	}

	private void OnDisable()
	{
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	public void ReloadFriends()
	{
		SetListContent();
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
		if (Application.isEditor)
		{
			ScrollToMyPosition();
			return;
		}
		LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
		if (screenState == LeaderboardScreenManager.LeaderboardScreenState.Facebook || screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter || screenState == LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter)
		{
			ScrollToMyPosition();
		}
	}

	private void ScrollToMyPosition()
	{
		int num = GetMyIndexInTheList() - 2;
		if (num >= 0)
		{
			Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
			if (firstVisibleContainerOnScreen != null)
			{
				float offset = _scrollView.rootPanel.baseClipRegion.w * 0.5f - firstVisibleContainerOnScreen.height * 0.5f;
				_scrollView.SetSelection(num, ContainerPivot.Top, offset);
			}
		}
	}

	private int GetMyIndexInTheList()
	{
		int result = -1;
		if (_contentList != null)
		{
			for (int i = 0; i < _contentList.Count; i++)
			{
				if (_contentList[i].cellType == LeaderboardGameoverCellType.Player)
				{
					result = i;
				}
			}
		}
		return result;
	}

	private void SetListContent()
	{
		ResetFriendList();
		LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
		switch (screenState)
		{
		case LeaderboardScreenManager.LeaderboardScreenState.Offline:
			SetOfflineContent();
			break;
		case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
		case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
		case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
			SetOnlineContent(screenState);
			break;
		case LeaderboardScreenManager.LeaderboardScreenState.AgeRestricted:
			SetAgeRestrictedContent();
			break;
		default:
			SetOfflineContent();
			break;
		}
	}

	private void ResetFriendList()
	{
		_contentList.Clear();
	}

	private void SetAgeRestrictedContent()
	{
		int nElements = 0;
		int rankCount = 1;
		InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: false, listLocked: true);
	}

	private void SetOfflineContent()
	{
		int nElements = 0;
		int rankCount = 1;
		InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: false, listLocked: false);
		AddFacebookButtonToList(ref nElements, noFriends: true);
	}

	private void SetOnlineContent(LeaderboardScreenManager.LeaderboardScreenState screenState)
	{
		if (Application.isEditor)
		{
			Friend[] debugFriends = Globals.GetDebugFriends(30);
			SetOnlineContentListForLeaderboard(fbConnected: true, debugFriends, lockList: false);
			return;
		}
		Friend[] array = SocialManager.instance.FriendsSortedByScore();
		bool lockList = screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && array.Count() < 4;
		switch (screenState)
		{
		case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
		case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
			SetOnlineContentListForLeaderboard(fbConnected: true, array, lockList);
			break;
		case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
			SetOnlineContentListForLeaderboard(fbConnected: false, array, lockList);
			break;
		}
	}

	private void SetOnlineContentListForLeaderboard(bool fbConnected, Friend[] friends, bool lockList)
	{
		bool flag = false;
		int nElements = 0;
		int rankCount = 1;
		int num = friends.Count();
		bool flag2 = !fbConnected && friends.Count() >= 4;
		bool flag3 = !fbConnected && friends.Count() < 4;
		if (num > 0)
		{
			for (int i = 0; i < friends.Length; i++)
			{
				if (!flag && PlayerSave.highScore >= friends[i].score)
				{
					InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: true, lockList);
					flag = true;
					if (flag2)
					{
						InsertFacebookLoginButton(ref nElements);
					}
				}
				InsertFriendScoreToLeaderboard(friends[i], ref rankCount, ref nElements, lockList);
			}
			if (!flag)
			{
				InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: true, lockList);
				if (flag2)
				{
					InsertFacebookLoginButton(ref nElements);
				}
			}
			if (flag3)
			{
				InsertFacebookLoginButton(ref nElements);
			}
			AddSpaceToList();
		}
		else
		{
			InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: true, listLocked: true);
			if (!fbConnected)
			{
				AddFacebookButtonToList(ref nElements, noFriends: true, lockList);
			}
		}
	}

	private void InsertPlayerScoreToLeaderboard(ref int rankCount, ref int nElements, bool isRanked, bool listLocked)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.Player,
			rank = rankCount,
			useDarkColour = (nElements % 2 == 0),
			isPlayerRanked = isRanked,
			lockList = listLocked
		});
		if (isRanked)
		{
			rankCount++;
		}
		nElements++;
	}

	private void InsertFriendScoreToLeaderboard(Friend newFriend, ref int rankCount, ref int nElements, bool listLocked)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.Friend,
			friend = newFriend,
			useDarkColour = (nElements % 2 == 0),
			rank = rankCount,
			lockList = listLocked
		});
		rankCount++;
		nElements++;
	}

	private void AddSpaceToList()
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.Space
		});
	}

	private void InsertFacebookLoginButton(ref int nElements)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.FacebookButton
		});
	}

	private void AddFacebookButtonToList(ref int nElements, bool noFriends = false, bool lockList = false)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.FacebookButton,
			noFriends = noFriends,
			lockList = lockList
		});
		nElements++;
	}

	public void FacebookClicked()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(loggedIn: true);
		}
	}

	private void FacebookStatusChanged(bool success)
	{
		if (success)
		{
			if (!FacebookRewardsInfo.PayoutRewardIfPossible())
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.Facebook,
					facebookState = UISlideInFacebook.FacebookSlideInState.ConnectSucceeded
				});
			}
		}
		else if (_facebookLoginRunning)
		{
			OnFacebookLoginResult(loggedIn: false);
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_facebookLoginRunning = false;
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new SlideIn
		{
			type = SlideInData.SlideInType.Facebook,
			facebookState = UISlideInFacebook.FacebookSlideInState.ConnectFailed,
			onDidShowCallback = onFinishCallback
		});
	}

	private void OnFailSlideInFinish()
	{
		FacebookStatusChanged(success: false);
	}
}
