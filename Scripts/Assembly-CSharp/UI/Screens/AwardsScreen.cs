using System;
using System.Collections.Generic;
using Info;
using Kiloo.Common.UI;
using Missions;
using UI.SlideIns;
using UnityEngine;
using Utilities;

namespace UI.Screens;

public class AwardsScreen : UIScreen
{
	private class AwardListAdapter : ListAdapter<AwardCell>
	{
		public class AwardListAdapterPayload
		{
			public List<AwardCell> awardList;

			public GameObject awardPrefab;

			public GameObject finishedAwardPrefab;

			public GameObject titlePrefab;

			public AwardsScreen screenReference;

			public GameObject spacerPrefab;
		}

		public GameObject finishedAwardPrefab;

		public GameObject awardPrefab;

		public GameObject titlePrefab;

		public AwardsScreen screenReference;

		public GameObject spacerPrefab;

		private const int AWARD_TITLE_PREFAB = 0;

		private const int AWARD_CELL_PREFAB = 1;

		private const int AWARD_FINISHED_CELL_PREFAB = 2;

		private const int AWARD_SPACER_PREFAB = 3;

		public AwardListAdapter(AwardListAdapterPayload payload)
			: base(payload.awardList)
		{
			finishedAwardPrefab = payload.finishedAwardPrefab;
			awardPrefab = payload.awardPrefab;
			titlePrefab = payload.titlePrefab;
			screenReference = payload.screenReference;
			spacerPrefab = payload.spacerPrefab;
		}

		public override int GetModelType(int index)
		{
			return GetItem(index).cellType switch
			{
				AwardCellType.Award => 1, 
				AwardCellType.FinishedAward => 2, 
				AwardCellType.Title => 0, 
				AwardCellType.Spacer => 3, 
				_ => 1, 
			};
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			AwardCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				AwardCellReferences awardCellReferences = new AwardCellReferences();
				switch (item.cellType)
				{
				case AwardCellType.Award:
				{
					container2 = new Container(awardPrefab);
					AwardCellHelper component3 = container2.model.GetComponent<AwardCellHelper>();
					awardCellReferences.awardCellHelper = component3;
					container2.tag = awardCellReferences;
					break;
				}
				case AwardCellType.FinishedAward:
				{
					container2 = new Container(finishedAwardPrefab);
					AwardFinishedCellHelper component2 = container2.model.GetComponent<AwardFinishedCellHelper>();
					awardCellReferences.awardFinishedCellHelper = component2;
					container2.tag = awardCellReferences;
					break;
				}
				case AwardCellType.Title:
				{
					container2 = new Container(titlePrefab);
					ListTitleComponentHelper component = container2.model.GetComponent<ListTitleComponentHelper>();
					awardCellReferences.awardTitleHelper = component;
					container2.tag = awardCellReferences;
					break;
				}
				case AwardCellType.Spacer:
					container2 = new Container(spacerPrefab);
					break;
				}
			}
			if (!isMeasurePass)
			{
				AwardCellReferences awardCellReferences2 = (AwardCellReferences)container2.tag;
				switch (item.cellType)
				{
				case AwardCellType.Award:
					awardCellReferences2.awardCellHelper.InitCell(item.award, screenReference);
					break;
				case AwardCellType.FinishedAward:
					awardCellReferences2.awardFinishedCellHelper.InitCell(item.award);
					break;
				case AwardCellType.Title:
					awardCellReferences2.awardTitleHelper.Setup(Strings.Get("AWARDS_TITLE"), ColorData.instance.midnight);
					break;
				}
			}
			return container2;
		}
	}

	[SerializeField]
	private GameObject _awardPrefab;

	[SerializeField]
	private GameObject _finishedAwardPrefab;

	[SerializeField]
	private GameObject _titlePrefab;

	[SerializeField]
	private GameObject _spacerPrefab;

	[SerializeField]
	private ScrollView _awardListView;

	[SerializeField]
	private UIPanel _scrollPanel;

	private int _indexOnHide;

	private AwardListAdapter _adapter;

	private List<AwardCell> _contentList = new List<AwardCell>();

	private bool _containsCollectableAward;

	private bool _facebookLoginRunning;

	private bool _hasSetSafeAreaOffsetsIfNecessary;

	protected override void Init()
	{
		if (_contentList != null)
		{
			_adapter = new AwardListAdapter(new AwardListAdapter.AwardListAdapterPayload
			{
				awardList = _contentList,
				awardPrefab = _awardPrefab,
				finishedAwardPrefab = _finishedAwardPrefab,
				titlePrefab = _titlePrefab,
				screenReference = this,
				spacerPrefab = _spacerPrefab
			});
		}
		if (SafeArea.IsRestricted && !_hasSetSafeAreaOffsetsIfNecessary)
		{
			_scrollPanel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
			_scrollPanel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
			_hasSetSafeAreaOffsetsIfNecessary = true;
		}
	}

	private void RefreshAwards()
	{
		_containsCollectableAward = SetListContent();
		if (_awardListView != null && _adapter != null)
		{
			_awardListView.SetAdapter(_adapter);
			if (!_containsCollectableAward)
			{
				_awardListView.SetSelection(_indexOnHide);
			}
		}
	}

	protected override void Show()
	{
		UpdateAnchors();
		RefreshAwards();
	}

	public override void GainFocus()
	{
		UpdateAnchors();
		RefreshAwards();
	}

	protected override void Hide()
	{
		Container firstVisibleContainerOnScreen = _awardListView.GetFirstVisibleContainerOnScreen();
		if (firstVisibleContainerOnScreen != null)
		{
			_indexOnHide = firstVisibleContainerOnScreen.dataIndex;
		}
		if (UIScreenController.Instance.isShowingPopup)
		{
			return;
		}
		bool containsCollectibleAwards = false;
		foreach (Award sortedListAward in AwardsManager.instance.GetSortedListAwards(out containsCollectibleAwards))
		{
			if (sortedListAward.isInPreview)
			{
				sortedListAward.isInPreview = false;
			}
		}
	}

	private void UpdateAnchors()
	{
		_scrollPanel.ResetAndUpdateAnchors();
	}

	public void FacebookLoginClicked()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(loggedIn: true);
		}
		Container firstVisibleContainerOnScreen = _awardListView.GetFirstVisibleContainerOnScreen();
		if (firstVisibleContainerOnScreen != null)
		{
			_indexOnHide = firstVisibleContainerOnScreen.dataIndex;
		}
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private void OnFailSlideInFinish()
	{
		FacebookStatusChanged(success: false);
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
		RefreshAwards();
	}

	private bool SetListContent()
	{
		_contentList.Clear();
		List<Award> sortedListAwards = AwardsManager.instance.GetSortedListAwards(out var containsCollectibleAwards);
		_contentList.Add(new AwardCell
		{
			cellType = AwardCellType.Spacer
		});
		_contentList.Add(new AwardCell
		{
			cellType = AwardCellType.Title
		});
		for (int i = 0; i < sortedListAwards.Count; i++)
		{
			_contentList.Add(new AwardCell
			{
				cellType = ((sortedListAwards[i].awardState != AwardState.Finished || sortedListAwards[i].isInPreview) ? AwardCellType.Award : AwardCellType.FinishedAward),
				award = sortedListAwards[i]
			});
		}
		return containsCollectibleAwards;
	}
}
