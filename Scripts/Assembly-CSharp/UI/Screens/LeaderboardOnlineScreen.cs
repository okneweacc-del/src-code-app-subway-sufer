using System;
using System.Collections.Generic;
using Data.Save;
using Info;
using Kiloo.Common.UI;
using UI.Leaderboard;
using UI.SlideIns;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace UI.Screens;

public class LeaderboardOnlineScreen : UIScreen
{
	private class LeaderboardListAdapter : ListAdapter<LeaderboardCell>
	{
		private GameObject _friendPrefab;

		private GameObject _playerPrefab;

		private GameObject _spacePrefab;

		private GameObject _headerPrefab;

		private Action<Transform> _adjustOverlay;

		private const int FRIEND_PREFAB = 0;

		private const int PLAYER_PREFAB = 1;

		private const int SPACE_PREFAB = 2;

		private const int HEADER_PREFAB = 3;

		public LeaderboardListAdapter(List<LeaderboardCell> contentList, GameObject newFriendPrefab, GameObject newPlayerPrefab, GameObject newSpacePrefab, GameObject headerPrefab, Action<Transform> adjustOverlay)
			: base(contentList)
		{
			_friendPrefab = newFriendPrefab;
			_playerPrefab = newPlayerPrefab;
			_spacePrefab = newSpacePrefab;
			_headerPrefab = headerPrefab;
			_adjustOverlay = adjustOverlay;
		}

		public override int GetModelType(int index)
		{
			return GetItem(index).cellType switch
			{
				LeaderboardCellType.Friend => 0, 
				LeaderboardCellType.Player => 1, 
				LeaderboardCellType.Space => 2, 
				LeaderboardCellType.DummyFriend => 0, 
				LeaderboardCellType.Header => 3, 
				_ => 0, 
			};
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
		{
			Container container2 = container;
			LeaderboardCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				LeaderboardCellReferences leaderboardCellReferences = new LeaderboardCellReferences();
				switch (item.cellType)
				{
				case LeaderboardCellType.Friend:
				{
					container2 = new Container(_friendPrefab);
					FriendHelperHighScore component5 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component5;
					container2.tag = leaderboardCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardCellType.Player:
				{
					container2 = new Container(_playerPrefab);
					FriendHelperHighScore component4 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component4;
					container2.tag = leaderboardCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardCellType.DummyFriend:
				{
					container2 = new Container(_friendPrefab);
					FriendHelperHighScore component3 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component3;
					container2.tag = leaderboardCellReferences;
					break;
				}
				case LeaderboardCellType.Space:
					container2 = new Container(_spacePrefab);
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				case LeaderboardCellType.Header:
				{
					container2 = new Container(_headerPrefab);
					LeaderboardHeaderHelper component2 = container2.model.GetComponent<LeaderboardHeaderHelper>();
					leaderboardCellReferences.headerHelper = component2;
					container2.tag = leaderboardCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				default:
				{
					SDebug.LogError("No cell type match for: " + item.cellType);
					container2 = new Container(_friendPrefab);
					FriendHelperHighScore component = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component;
					container2.tag = leaderboardCellReferences;
					break;
				}
				}
			}
			if (item.adjustOverlay)
			{
				_adjustOverlay(container2.modelTransform);
			}
			LeaderboardCellReferences leaderboardCellReferences2 = (LeaderboardCellReferences)container2.tag;
			switch (item.cellType)
			{
			case LeaderboardCellType.Friend:
				leaderboardCellReferences2.friendHelper.InitFriend(item.friend, item.rank, item.useDarkColor);
				item.OnUpdateCell = leaderboardCellReferences2.friendHelper.Refresh;
				break;
			case LeaderboardCellType.Player:
				leaderboardCellReferences2.friendHelper.InitLocalUser(item.rank, item.useDarkColor, item.isPlayerRanked);
				item.OnUpdateCell = leaderboardCellReferences2.friendHelper.Refresh;
				break;
			case LeaderboardCellType.DummyFriend:
				leaderboardCellReferences2.friendHelper.InitDummy(item.dummyFriend, item.rank, item.useDarkColor);
				break;
			case LeaderboardCellType.Header:
				leaderboardCellReferences2.headerHelper.Setup(item.headerText, item.headerColor);
				break;
			default:
				SDebug.LogError("No cell type match for: " + item.cellType);
				break;
			case LeaderboardCellType.Space:
				break;
			}
			return container2;
		}
	}

	[SerializeField]
	private GameObject _friendPrefab;

	[SerializeField]
	private FacebookLoginPromotion _facebookContent;

	[SerializeField]
	private GameObject _spacePrefab;

	[SerializeField]
	private GameObject _headerPrefab;

	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private ScrollView _scrollView;

	[SerializeField]
	private UIPanel _scrollPanel;

	[FormerlySerializedAs("_overlaySizer")]
	[SerializeField]
	private UIWidget _lockedOverlay;

	[SerializeField]
	private GameObject _lockContent;

	private const int GC_FRIENDS_THRESHOLD = 4;

	private List<LeaderboardCell> _contentList = new List<LeaderboardCell>();

	private LeaderboardListAdapter _adapter;

	private bool _facebookLoginRunning;

	private const int POSITION_OFFSET_IN_LIST = 2;

	protected override void Init()
	{
		base.Init();
		HandleSafeArea();
		_adapter = new LeaderboardListAdapter(_contentList, _friendPrefab, _friendPrefab, _spacePrefab, _headerPrefab, AdjustLockOverlay);
		_scrollPanel.ResetAndUpdateAnchors();
	}

	private void HandleSafeArea()
	{
		if (SafeArea.IsRestricted)
		{
			_scrollPanel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
			_scrollPanel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
			_scrollPanel.ResetAndUpdateAnchors();
			_titleLabel.ResetAndUpdateAnchors();
		}
	}

	protected override void Show()
	{
		base.Show();
		SocialManager.friendsChangedHandler += ReloadFriends;
		UIScreenController.OnApplicationResumed += ReloadFriends;
		SocialManager.instance.UpdateFriendScores((bool didLoadFriends) =>
		{
			ReloadFriends();
		});
		ReloadFriends();
		_titleLabel.text = Strings.Get("HIGH_SCORES_TITLE");
	}

	private void ScrollToMyPosition()
	{
		int num = GetMyIndexInTheList() - 2;
		if (num > 0)
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
		return _contentList.FindIndex((LeaderboardCell x) => x.cellType == LeaderboardCellType.Player);
	}

	protected override void Hide()
	{
		base.Hide();
		SocialManager.friendsChangedHandler -= ReloadFriends;
		UIScreenController.OnApplicationResumed -= ReloadFriends;
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	private void ReloadFriends()
	{
		SetListContent();
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
		_lockedOverlay.ResetAnchors();
		LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
		Friend[] array = ((!Application.isEditor) ? SocialManager.instance.FriendsSortedByScore() : Globals.GetDebugFriends(1));
		bool flag = screenState == LeaderboardScreenManager.LeaderboardScreenState.Facebook || screenState == LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter;
		if (((screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && array.Length >= 4) || flag) && screenState != LeaderboardScreenManager.LeaderboardScreenState.Offline)
		{
			ScrollToMyPosition();
		}
	}

	private void ResetFriendList()
	{
		_contentList.Clear();
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

	private void SetAgeRestrictedContent()
	{
		int nElements = 0;
		int rankCount = 1;
		InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: false, listLocked: false, adjustOverlay: false);
		SetActiveFacebookLoginButton(active: false);
		AddOverlayAndLockList(enable: false);
	}

	private void SetOfflineContent()
	{
		int nElements = 0;
		int rankCount = 1;
		InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: false, listLocked: true, adjustOverlay: true);
		SetActiveFacebookLoginButton(active: true);
		AddDummiesToList(ref rankCount, ref nElements);
		AddOverlayAndLockList(enable: true);
	}

	private void SetOnlineContent(LeaderboardScreenManager.LeaderboardScreenState screenState)
	{
		Friend[] array = ((!Application.isEditor) ? SocialManager.instance.FriendsSortedByScore() : Globals.GetDebugFriends(30));
		bool lockList = screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && array.Length < 4;
		bool facebookButtonActive = false;
		switch (screenState)
		{
		case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
		case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
			SetOnlineContentListForLeaderboard(fbConnected: true, array, lockList, ref facebookButtonActive);
			break;
		case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
			SetOnlineContentListForLeaderboard(fbConnected: false, array, lockList, ref facebookButtonActive);
			break;
		}
		SetActiveFacebookLoginButton(facebookButtonActive);
	}

	private void SetOnlineContentListForLeaderboard(bool fbConnected, Friend[] friends, bool lockList, ref bool facebookButtonActive)
	{
		bool flag = false;
		int nElements = 0;
		int rankCount = 1;
		int num = friends.Length;
		bool flag2 = !fbConnected && friends.Length >= 4;
		bool flag3 = !fbConnected && friends.Length < 4;
		if (num > 0)
		{
			if (!fbConnected)
			{
				AddGameCenterHeaderToList(lockList);
			}
			foreach (Friend friend in friends)
			{
				if (!flag && PlayerSave.highScore >= friend.score)
				{
					InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: true, lockList, adjustOverlay: false);
					flag = true;
					if (flag2)
					{
						facebookButtonActive = true;
					}
				}
				InsertFriendScoreToLeaderboard(friend, ref rankCount, ref nElements, lockList);
			}
			if (!flag)
			{
				InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: true, lockList, adjustOverlay: false);
				if (flag2)
				{
					facebookButtonActive = true;
				}
			}
			if (flag3)
			{
				facebookButtonActive = true;
				AddSpaceToList(lockList);
				AddSpaceToList(lockList);
				AddFacebookHeaderToList(lockList);
				AddDummiesToList(ref rankCount, ref nElements);
				AddOverlayAndLockList(enable: true);
			}
			else
			{
				AddOverlayAndLockList(enable: false);
			}
			AddSpaceToList(lockList);
		}
		else
		{
			InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, isRanked: true, listLocked: true, adjustOverlay: true);
			if (!fbConnected)
			{
				facebookButtonActive = true;
				AddDummiesToList(ref rankCount, ref nElements);
				AddOverlayAndLockList(enable: true);
			}
			else
			{
				AddOverlayAndLockList(enable: false);
			}
		}
	}

	private void InsertPlayerScoreToLeaderboard(ref int rankCount, ref int nElements, bool isRanked, bool listLocked, bool adjustOverlay)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.Player,
			rank = rankCount,
			useDarkColor = (nElements % 2 == 0),
			isPlayerRanked = isRanked,
			lockList = listLocked,
			adjustOverlay = adjustOverlay
		});
		if (isRanked)
		{
			rankCount++;
		}
		nElements++;
	}

	private void InsertFriendScoreToLeaderboard(Friend newFriend, ref int rankCount, ref int nElements, bool listLocked)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.Friend,
			friend = newFriend,
			useDarkColor = (nElements % 2 == 0),
			rank = rankCount,
			lockList = listLocked
		});
		rankCount++;
		nElements++;
	}

	private void AddSpaceToList(bool listLocked)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.Space,
			lockList = listLocked
		});
	}

	private void AddGameCenterHeaderToList(bool lockList)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.Header,
			headerText = Strings.Get("LEADERBOARD_HEADER_GAMECENTER"),
			headerColor = ColorData.instance.bulbasaur,
			lockList = lockList
		});
	}

	private void AddFacebookHeaderToList(bool lockList)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.Space,
			lockList = lockList,
			adjustOverlay = true
		});
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.Header,
			headerText = Strings.Get("LEADERBOARD_HEADER_FACEBOOK"),
			headerColor = ColorData.instance.ocean,
			lockList = lockList
		});
	}

	private void SetActiveFacebookLoginButton(bool active)
	{
		_facebookContent.gameObject.SetActive(active);
		if (active)
		{
			_facebookContent.Init("LeaderboardUI", FacebookClicked);
		}
	}

	private void AddDummiesToList(ref int rankCount, ref int nElements)
	{
		List<LeaderboardDummyData.LeaderboardDummy> dummyData = LeaderboardDummyData.dummyData;
		if (dummyData == null)
		{
			return;
		}
		foreach (LeaderboardDummyData.LeaderboardDummy item in dummyData)
		{
			_contentList.Add(new LeaderboardCell
			{
				cellType = LeaderboardCellType.DummyFriend,
				dummyFriend = item,
				rank = rankCount,
				useDarkColor = (nElements % 2 == 0)
			});
			rankCount++;
			nElements++;
		}
	}

	private void AddOverlayAndLockList(bool enable)
	{
		_lockContent.SetActive(enable);
	}

	private void AdjustLockOverlay(Transform obj)
	{
		_lockedOverlay.topAnchor.target = obj;
		_lockedOverlay.topAnchor.relative = 0f;
		_lockedOverlay.topAnchor.absolute = 0;
		_lockedOverlay.ResetAndUpdateAnchors();
	}

	private void FacebookClicked()
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
			ReloadFriends();
		}
		else if (_facebookLoginRunning)
		{
			OnFacebookLoginResult(loggedIn: false);
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_facebookLoginRunning = false;
	}

	private void OnFacebookLoginResult(bool loggedIn)
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
