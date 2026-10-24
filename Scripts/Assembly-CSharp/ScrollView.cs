using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Kiloo.Common.UI;
using UnityEngine;

[RequireComponent(typeof(UIPanel))]
public class ScrollView : MonoBehaviour
{
	public enum ScrollDirection
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		Stationary = 4
	}

	private enum Infinite
	{
		None = 0,
		BothWays = 1
	}

	private enum BoundsCalculationMode
	{
		FullSize = 0,
		Incremental = 1
	}

	private enum Arrangement
	{
		Vertical = 0,
		Horizontal = 1
	}

	private enum PullDownToRefreshState
	{
		None = 0,
		PullDown = 1,
		Refreshing = 2
	}

	private class RecycleBin
	{
		public readonly List<Container> activeContainers = new List<Container>();

		private readonly Dictionary<int, List<Container>> _scrapViews = new Dictionary<int, List<Container>>();

		public Container GetActiveContainer(int position)
		{
			if (position < 0 || position >= activeContainers.Count)
			{
				return null;
			}
			Container container = activeContainers[position];
			if (container == null)
			{
				return null;
			}
			activeContainers.RemoveAt(position);
			return container;
		}

		public void AddToActiveContainers(Container container, bool endOfList)
		{
			if (endOfList)
			{
				activeContainers.Add(container);
			}
			else
			{
				activeContainers.Insert(0, container);
			}
		}

		public void AddScrapContainer(Container container)
		{
			if (!_scrapViews.ContainsKey(container.modelType))
			{
				_scrapViews.Add(container.modelType, new List<Container>());
			}
			_scrapViews[container.modelType].Add(container);
			container.modelTransform.localPosition = new Vector3(Screen.width * 2, Screen.height * 2, 0f);
		}

		public void Clear()
		{
			foreach (Container activeContainer in activeContainers)
			{
				AddScrapContainer(activeContainer);
			}
			activeContainers.Clear();
			foreach (List<Container> value in _scrapViews.Values)
			{
				foreach (Container item in value)
				{
					if (item.onRecycleContainer != null)
					{
						item.onRecycleContainer();
					}
					UnityEngine.Object.Destroy(item.model);
				}
				value.Clear();
			}
			_scrapViews.Clear();
		}

		public Container GetScrapContainer(int modelType)
		{
			Container result = null;
			if (_scrapViews.Count > 0)
			{
				if (!_scrapViews.ContainsKey(modelType))
				{
					_scrapViews.Add(modelType, new List<Container>());
				}
				else
				{
					List<Container> list = _scrapViews[modelType];
					if (list.Count > 0)
					{
						result = list[0];
						list.RemoveAt(0);
					}
				}
			}
			return result;
		}

		public void ScrapActiveContainers()
		{
			List<Container> list = new List<Container>(activeContainers);
			activeContainers.Clear();
			foreach (Container item in list)
			{
				AddScrapContainer(item);
			}
		}

		[Conditional("SCROLL_VIEW_DEBUG_LOGS")]
		public static void Log(string msg)
		{
		}
	}

	[SerializeField]
	private Arrangement _arrangement = Arrangement.Horizontal;

	[SerializeField]
	public int cellSpaceY;

	[SerializeField]
	public int cellSpaceX;

	[SerializeField]
	public GameObject pullDownToRefreshPrefab;

	[SerializeField]
	public bool pullDownToRefreshList;

	[SerializeField]
	private Infinite _infinite;

	private UICenterOnChild _centerOnChild;

	private Transform _rootTransform;

	[NonSerialized]
	private UIPanel _rootPanel;

	[NonSerialized]
	private UIScrollView _uiScrollView;

	private Vector2 _nextAppendPosition = Vector2.zero;

	private Vector2 _nextPrependPosition = Vector2.zero;

	private bool _dataChangedDelegatesAdded;

	private AbstractListAdapter _adapter;

	private Bounds _mListViewBounds;

	private RecycleBin _recycleBin;

	private Container _bottomActiveContainer;

	private Container _topActiveContainer;

	private int _endDataIndexCursor;

	private int _topDataIndexCursor;

	public Container pullDownToRefreshContainer;

	private bool _refreshingContainers;

	public Action onTopReached;

	public Action onDraggingStoppped;

	private SpringPanel _springPanel;

	private Container _firstVisibleContainer;

	private Container _lastVisibleContainer;

	public Action onPreLayout;

	public Action onPullDownRelease;

	public Action onPullDownToTriggerRefresh;

	public Action<int> onPullDown;

	private bool _refreshingContainerAdded;

	private bool _refreshContainerEncapsulated;

	[NonSerialized]
	public float scrollViewHeight;

	[NonSerialized]
	public float scrollViewWidth;

	private const int PULL_TO_REFRESH_CONTAINER = -100;

	private int _currentMaxListCount;

	private int _currentMinListCount;

	private PullDownToRefreshState _pullDownToRefreshState;

	[SerializeField]
	private BoundsCalculationMode boundsCalculationMode;

	private float _currentPos;

	private float _previousPos;

	private ScrollDirection _scrollDirection;

	private bool press;

	private float _movementDelta;

	private float _cumulativeDelta;

	public Transform rootPanelTransform => (!_rootTransform) ? rootPanel.transform : _rootTransform;

	public UIPanel rootPanel => _rootPanel ?? (_rootPanel = base.gameObject.GetComponent<UIPanel>());

	public UIScrollView uiScrollView => _uiScrollView ?? (_uiScrollView = base.gameObject.GetComponent<UIScrollView>());

	public ScrollDirection scrollDirection => _scrollDirection;

	public bool animating { get; private set; }

	public Bounds listViewBounds => _mListViewBounds;

	public bool infinite => _infinite == Infinite.BothWays;

	private void InitScrollView()
	{
		if (_adapter != null)
		{
			if (!_dataChangedDelegatesAdded)
			{
				_adapter.onNotifyDataSetChanged = OnDataSetChanged;
				rootPanel.onClipMove = OnClipMove;
				_dataChangedDelegatesAdded = true;
			}
			_recycleBin = new RecycleBin();
		}
		uiScrollView.movement = ((_arrangement != Arrangement.Horizontal) ? UIScrollView.Movement.Vertical : UIScrollView.Movement.Horizontal);
	}

	[UsedImplicitly]
	private void Start()
	{
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(OnPress));
		_centerOnChild = GetComponent<UICenterOnChild>();
	}

	private void OnPress(GameObject go, bool state)
	{
		press = state;
		if (!state)
		{
			_cumulativeDelta = 0f;
		}
	}

	private void CreatePullDownToRefreshContainer()
	{
		if (pullDownToRefreshContainer == null)
		{
			pullDownToRefreshContainer = new Container(pullDownToRefreshPrefab);
			pullDownToRefreshContainer.Measure();
			pullDownToRefreshContainer.top = 0f - pullDownToRefreshContainer.height;
			pullDownToRefreshContainer.bottom = 0f;
			pullDownToRefreshContainer.SetParent(rootPanelTransform);
			Vector2 zero = Vector2.zero;
			zero.y = rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y + pullDownToRefreshContainer.height * 0.5f - pullDownToRefreshContainer.bounds.center.y - rootPanel.clipSoftness.y;
			pullDownToRefreshContainer.coordinates = zero;
			pullDownToRefreshContainer.SetPosition();
			pullDownToRefreshContainer.SetScale(Vector3.one);
			pullDownToRefreshContainer.modelType = -100;
		}
	}

	private void UpdatePullDownToRefreshContainer()
	{
		if (!pullDownToRefreshList || _refreshingContainers)
		{
			return;
		}
		if (_topDataIndexCursor == -1 && !_refreshContainerEncapsulated && _scrollDirection == ScrollDirection.Down && press)
		{
			_pullDownToRefreshState = PullDownToRefreshState.PullDown;
		}
		if (_pullDownToRefreshState == PullDownToRefreshState.PullDown)
		{
			uiScrollView.dragEffect = UIScrollView.DragEffect.MomentumAndSpring;
			if (onPullDown != null && pullDownToRefreshContainer != null)
			{
				int obj = Mathf.RoundToInt(rootPanelTransform.localPosition.y - pullDownToRefreshContainer.top);
				onPullDown(obj);
			}
		}
		if (pullDownToRefreshList && _topDataIndexCursor == -1 && _pullDownToRefreshState == PullDownToRefreshState.PullDown && pullDownToRefreshContainer != null && Mathf.RoundToInt(rootPanelTransform.localPosition.y - pullDownToRefreshContainer.top) < 0 && !_refreshContainerEncapsulated && !press)
		{
			Vector3 size = _mListViewBounds.size;
			Vector3 center = _mListViewBounds.center;
			size.x = rootPanel.baseClipRegion.z;
			size.y += pullDownToRefreshContainer.height;
			center.y += pullDownToRefreshContainer.height * 0.5f;
			center.x = 0f;
			_mListViewBounds.size = size;
			_mListViewBounds.center = center;
			_refreshContainerEncapsulated = true;
			uiScrollView.RestrictWithinBounds(instant: false, horizontal: false, vertical: true);
			uiScrollView.movement = UIScrollView.Movement.Vertical;
			_pullDownToRefreshState = PullDownToRefreshState.Refreshing;
			if (onPullDownRelease != null)
			{
				onPullDownRelease();
			}
		}
		if (pullDownToRefreshContainer != null && _pullDownToRefreshState == PullDownToRefreshState.Refreshing && Mathf.RoundToInt(rootPanelTransform.localPosition.y - pullDownToRefreshContainer.top) == 0 && onPullDownToTriggerRefresh != null)
		{
			onPullDownToTriggerRefresh();
			_pullDownToRefreshState = PullDownToRefreshState.None;
		}
	}

	private void Update()
	{
		CheckScrollState();
	}

	private void OnClipMove(UIPanel panel)
	{
		if (_refreshingContainers || _adapter == null)
		{
			return;
		}
		HandleScrollUp();
		HandleScrollDown();
		UpdatePullDownToRefreshContainer();
		if (_infinite == Infinite.BothWays)
		{
			if (_arrangement == Arrangement.Horizontal)
			{
				Vector3 center = _mListViewBounds.center;
				center.x = rootPanel.clipOffset.x;
				_mListViewBounds.center = center;
			}
			else
			{
				Vector3 center2 = _mListViewBounds.center;
				center2.y = rootPanel.clipOffset.y;
				_mListViewBounds.center = center2;
			}
		}
	}

	private void SetContainerPositions()
	{
		foreach (Container activeContainer in _recycleBin.activeContainers)
		{
			activeContainer.SetPosition();
		}
	}

	private void ClearList()
	{
		uiScrollView.DisableSpring();
		_endDataIndexCursor = 0;
		_topDataIndexCursor = 0;
		_nextAppendPosition = Vector2.zero;
		_nextPrependPosition = Vector2.zero;
		rootPanel.transform.localPosition = Vector3.zero;
		rootPanel.clipOffset = Vector2.zero;
		_recycleBin.Clear();
	}

	private void OnDataSetChanged(int selection)
	{
		_refreshingContainers = true;
		_refreshingContainerAdded = false;
		if (_adapter.GetCount() == 0)
		{
			ClearList();
		}
		else
		{
			SetSelection(selection);
		}
		uiScrollView.UpdateScrollbars(recalculateBounds: false);
		rootPanel.transform.localPosition = Vector3.zero;
		rootPanel.clipOffset = Vector2.zero;
		_refreshingContainers = false;
	}

	public void CancelLoading()
	{
		_springPanel = GetComponent<SpringPanel>();
		_springPanel.target = Vector3.zero;
		_springPanel.enabled = true;
	}

	public void SetAdapter(AbstractListAdapter adapter)
	{
		if (_recycleBin != null)
		{
			_recycleBin.Clear();
		}
		_adapter = adapter;
		_dataChangedDelegatesAdded = false;
		InitScrollView();
		SetSelection(0);
		uiScrollView.UpdateScrollbars(recalculateBounds: false);
	}

	public void PrintActiveContainers()
	{
	}

	private void PrependContainer(int position, Container container, bool setPosition)
	{
		container.dataIndex = position;
		container.rowId = _adapter.GetItemId(position);
		container.SetModelType(_adapter.GetModelType(position));
		container.Measure();
		container.SetParent(rootPanel.transform);
		container.SetScale(Vector3.one);
		if (_recycleBin.activeContainers.Count > 0)
		{
			_topActiveContainer = _recycleBin.activeContainers[0];
			_nextPrependPosition = _topActiveContainer.coordinates;
		}
		if (_topActiveContainer != null)
		{
			_nextPrependPosition.x -= container.width * 0.5f + _topActiveContainer.width * 0.5f + (float)cellSpaceX + container.bounds.center.x - _topActiveContainer.bounds.center.x;
			_nextPrependPosition.y += container.height * 0.5f + _topActiveContainer.height * 0.5f + (float)cellSpaceY - container.bounds.center.y + _topActiveContainer.bounds.center.y;
		}
		if (_topActiveContainer != null)
		{
			container.bottom = _topActiveContainer.top;
			container.top = container.bottom - container.height - (float)cellSpaceY;
			container.right = _topActiveContainer.left;
			container.left = container.right - container.width - (float)cellSpaceX;
		}
		Vector2 nextPrependPosition = _nextPrependPosition;
		if (_arrangement == Arrangement.Horizontal)
		{
			nextPrependPosition.y = rootPanel.baseClipRegion.y;
		}
		else
		{
			nextPrependPosition.x = rootPanel.baseClipRegion.x;
		}
		container.coordinates = nextPrependPosition;
		_recycleBin.AddToActiveContainers(container, endOfList: false);
		_topDataIndexCursor--;
		if (setPosition)
		{
			container.SetPosition();
		}
	}

	private void AppendContainer(int position, Container container, bool setPosition)
	{
		container.dataIndex = position;
		container.rowId = _adapter.GetItemId(position);
		container.SetModelType(_adapter.GetModelType(position));
		container.SetParent(rootPanel.transform);
		container.SetScale(Vector3.one);
		container.Measure();
		if (_recycleBin.activeContainers.Count > 0)
		{
			_bottomActiveContainer = _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1];
			_nextAppendPosition = _bottomActiveContainer.coordinates;
		}
		else
		{
			_bottomActiveContainer = null;
		}
		if (_bottomActiveContainer != null)
		{
			_nextAppendPosition.x += container.width * 0.5f + _bottomActiveContainer.width * 0.5f + (float)cellSpaceX + _bottomActiveContainer.bounds.center.x - container.bounds.center.x;
			_nextAppendPosition.y -= container.height * 0.5f + _bottomActiveContainer.height * 0.5f + (float)cellSpaceY - _bottomActiveContainer.bounds.center.y + container.bounds.center.y;
			container.top = _bottomActiveContainer.bottom;
			container.bottom = container.top + container.height + (float)cellSpaceY;
			container.left = _bottomActiveContainer.right;
			container.right = container.left + container.width + (float)cellSpaceX;
		}
		Vector2 coordinates = (container.coordinates = _nextAppendPosition);
		if (_arrangement == Arrangement.Horizontal)
		{
			coordinates.y = rootPanel.baseClipRegion.y;
		}
		else
		{
			coordinates.x = rootPanel.baseClipRegion.x;
		}
		container.coordinates = coordinates;
		_recycleBin.AddToActiveContainers(container, endOfList: true);
		_endDataIndexCursor++;
		if (setPosition)
		{
			container.SetPosition();
		}
	}

	public void SetSelection(int position, ContainerPivot containerPivot = ContainerPivot.Top, float offset = 0f)
	{
		if (_adapter == null)
		{
			SDebug.LogError(base.name + " : Set the adapter first.", base.gameObject);
			return;
		}
		if (position < 0)
		{
			position = 0;
		}
		else if (position > _adapter.GetCount() - 1)
		{
			position = _adapter.GetCount() - 1;
		}
		FillFromIndex(position, offset, containerPivot);
		uiScrollView.currentMomentum = Vector3.zero;
	}

	public void SmoothScrollToIndex(int dataIndex, ContainerPivot containerPivot = ContainerPivot.Center, Action onFinished = null)
	{
		Container container = _recycleBin.activeContainers.Find((Container item) => item.dataIndex == dataIndex);
		if (container != null)
		{
			SmoothScrollToItem(container.modelTransform, containerPivot, onFinished);
			return;
		}
		if (dataIndex <= _topDataIndexCursor)
		{
			Vector3 localOffset = default;
			for (int num = _topDataIndexCursor; num >= dataIndex; num--)
			{
				Container container2 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(num)), num);
				container2.SetModelType(_adapter.GetModelType(num));
				container2.Measure();
				container2.SetParent(rootPanel.transform);
				container2.SetScale(Vector3.one);
				if (_arrangement == Arrangement.Vertical)
				{
					localOffset += new Vector3(0f, container2.height + (float)cellSpaceY, 0f);
				}
				else
				{
					localOffset += new Vector3(container2.width + (float)cellSpaceX, 0f, 0f);
				}
				_recycleBin.AddScrapContainer(container2);
			}
			SmoothScrollToOffset(localOffset, onFinished);
			return;
		}
		Vector3 localOffset2 = default;
		for (int num2 = _endDataIndexCursor; num2 <= dataIndex; num2++)
		{
			Container container3 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(num2)), num2);
			container3.SetModelType(_adapter.GetModelType(num2));
			container3.Measure();
			container3.SetParent(rootPanel.transform);
			container3.SetScale(Vector3.one);
			if (_arrangement == Arrangement.Vertical)
			{
				localOffset2 -= new Vector3(0f, container3.height + (float)cellSpaceY, 0f);
			}
			else
			{
				localOffset2 -= new Vector3(container3.width + (float)cellSpaceX, 0f, 0f);
			}
			_recycleBin.AddScrapContainer(container3);
		}
		SmoothScrollToOffset(localOffset2, onFinished);
	}

	public void SmoothScrollToItem(Transform target, ContainerPivot containerPivot = ContainerPivot.Center, Action onFinished = null)
	{
		if (_centerOnChild != null)
		{
			_centerOnChild.CenterOn(target);
			return;
		}
		Vector3[] worldCorners = _uiScrollView.panel.worldCorners;
		Vector3 position;
		switch (containerPivot)
		{
		case ContainerPivot.Center:
		case ContainerPivot.None:
			position = (worldCorners[2] + worldCorners[0]) * 0.5f;
			break;
		case ContainerPivot.Top:
			position = ((_arrangement != Arrangement.Vertical) ? ((worldCorners[0] + worldCorners[1]) * 0.5f) : ((worldCorners[1] + worldCorners[2]) * 0.5f));
			break;
		case ContainerPivot.Bottom:
			position = ((_arrangement != Arrangement.Vertical) ? ((worldCorners[2] + worldCorners[3]) * 0.5f) : ((worldCorners[0] + worldCorners[3]) * 0.5f));
			break;
		default:
			position = (worldCorners[2] + worldCorners[0]) * 0.5f;
			break;
		}
		Transform cachedTransform = _uiScrollView.panel.cachedTransform;
		Vector3 vector = cachedTransform.InverseTransformPoint(target.position);
		Vector3 vector2 = cachedTransform.InverseTransformPoint(position);
		Vector3 localOffset = vector - vector2;
		UIWidget component = target.GetComponent<UIWidget>();
		if (containerPivot != ContainerPivot.Center && containerPivot != ContainerPivot.None && component != null)
		{
			switch (_arrangement)
			{
			case Arrangement.Horizontal:
				localOffset.x += ((containerPivot != ContainerPivot.Bottom) ? 0.5f : (-0.5f)) * (float)component.width;
				break;
			case Arrangement.Vertical:
				localOffset.y += ((containerPivot != ContainerPivot.Bottom) ? 0.5f : (-0.5f)) * (float)component.height;
				break;
			}
		}
		SmoothScrollToOffset(localOffset, onFinished);
	}

	private void SmoothScrollToOffset(Vector3 localOffset, Action onFinished, float strength = 8f)
	{
		animating = true;
		if (!_uiScrollView.canMoveHorizontally)
		{
			localOffset.x = 0f;
		}
		if (!_uiScrollView.canMoveVertically)
		{
			localOffset.y = 0f;
		}
		localOffset.z = 0f;
		Transform cachedTransform = _uiScrollView.panel.cachedTransform;
		SpringPanel.Begin(_uiScrollView.panel.cachedGameObject, cachedTransform.localPosition - localOffset, strength).onFinished = () =>
		{
			animating = false;
			if (onFinished != null)
			{
				onFinished();
			}
		};
	}

	public IEnumerator AnimateSwapPositions(int dataPosition1, int dataPosition2, float time, Action onComplete = null)
	{
		animating = true;
		Container item1 = _recycleBin.activeContainers.Find((Container T) => T.dataIndex == dataPosition1);
		Container item2 = _recycleBin.activeContainers.Find((Container T) => T.dataIndex == dataPosition2);
		if (item1 != null && item2 != null)
		{
			Vector3 origin1 = item1.modelTransform.localPosition;
			Vector3 origin2 = item2.modelTransform.localPosition;
			float timePassed = 0f;
			while (timePassed < time)
			{
				timePassed += Time.deltaTime;
				float ratio = (float)Math.Sin(Math.PI / 2.0 * (double)timePassed / (double)time);
				item1.modelTransform.localPosition = Vector3.Lerp(origin1, origin2, ratio);
				item2.modelTransform.localPosition = Vector3.Lerp(origin2, origin1, ratio);
				yield return new WaitForEndOfFrame();
			}
			item1.modelTransform.localPosition = origin2;
			item2.modelTransform.localPosition = origin1;
			item1.dataIndex = dataPosition2;
			item2.dataIndex = dataPosition1;
		}
		animating = false;
		onComplete?.Invoke();
	}

	public void Recenter()
	{
		if (_centerOnChild != null)
		{
			_centerOnChild.Recenter();
		}
	}

	private void FillFromIndex(int i, float topOffset, ContainerPivot containerPivot = ContainerPivot.None)
	{
		if (pullDownToRefreshContainer != null)
		{
			NGUITools.Destroy(pullDownToRefreshContainer.model);
			pullDownToRefreshContainer = null;
		}
		_refreshContainerEncapsulated = false;
		_refreshingContainerAdded = false;
		_pullDownToRefreshState = PullDownToRefreshState.None;
		_scrollDirection = ScrollDirection.Stationary;
		_previousPos = 0f;
		_currentPos = 0f;
		if (i >= _adapter.GetCount() || i < 0 || _adapter.GetCount() == 0)
		{
			return;
		}
		_refreshingContainers = true;
		uiScrollView.DisableSpring();
		_recycleBin.Clear();
		rootPanelTransform.localPosition = Vector3.zero;
		rootPanel.clipOffset = Vector2.zero;
		_topDataIndexCursor = i;
		_endDataIndexCursor = i;
		CalculateVirtualRelativeWidgetBounds(boundsCalculationMode);
		Container container = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(i)), i);
		if (container != null)
		{
			float num = 0f;
			float num2 = 0f;
			container.isMeasured = false;
			container.Measure();
			num = rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y - container.height * 0.5f - container.bounds.center.y - topOffset;
			num2 = container.width * 0.5f - rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x + container.bounds.center.x + topOffset;
			switch (containerPivot)
			{
			case ContainerPivot.Top:
				num = rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y - container.height * 0.5f - container.bounds.center.y;
				num2 = container.width * 0.5f - rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x + container.bounds.center.x;
				topOffset = 0f;
				break;
			case ContainerPivot.Center:
				num = 0f;
				num2 = 0f;
				topOffset = ((_arrangement != Arrangement.Horizontal) ? (rootPanel.baseClipRegion.w / 2f - container.height / 2f) : (rootPanel.baseClipRegion.z / 2f - container.width / 2f));
				break;
			case ContainerPivot.Bottom:
				num = (0f - rootPanel.baseClipRegion.w) * 0.5f + rootPanel.baseClipRegion.y + container.height * 0.5f - container.bounds.center.y;
				num2 = (0f - container.width) * 0.5f + rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x + container.bounds.center.x;
				topOffset = ((_arrangement != Arrangement.Horizontal) ? (rootPanel.baseClipRegion.w - container.height) : (rootPanel.baseClipRegion.z - container.width));
				break;
			}
			container.coordinates = ((_arrangement != Arrangement.Vertical) ? new Vector2(num2, rootPanel.baseClipRegion.y) : new Vector2(rootPanel.baseClipRegion.x, num));
			if (_arrangement == Arrangement.Vertical)
			{
				container.top = topOffset;
				container.bottom = container.top + container.height + (float)cellSpaceY;
				container.left = 0f;
				container.right = container.left + container.width;
			}
			else
			{
				container.top = 0f;
				container.bottom = container.top + container.height;
				container.left = topOffset;
				container.right = topOffset + container.width + (float)cellSpaceX;
			}
			container.SetModelType(_adapter.GetModelType(i));
			container.SetParent(rootPanel.transform);
			container.SetScale(Vector3.one);
			Vector3 center = _mListViewBounds.center;
			if (_arrangement == Arrangement.Vertical)
			{
				center.y -= topOffset;
				_mListViewBounds.center = center;
			}
			else
			{
				center.x += topOffset + container.bounds.center.x;
				_mListViewBounds.center = center;
			}
			_topDataIndexCursor--;
			_endDataIndexCursor++;
			container.dataIndex = i;
			if (_infinite == Infinite.BothWays)
			{
				if (_topDataIndexCursor < 0)
				{
					_topDataIndexCursor = _adapter.GetCount() - 1;
				}
				if (_endDataIndexCursor == _adapter.GetCount())
				{
					_endDataIndexCursor = 0;
				}
			}
			container.rowId = _adapter.GetItemId(container.dataIndex);
			_recycleBin.AddToActiveContainers(container, endOfList: true);
			if (_arrangement == Arrangement.Vertical)
			{
				while (_endDataIndexCursor < _adapter.GetCount() && _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1].bottom < rootPanel.baseClipRegion.w)
				{
					Container container2 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_endDataIndexCursor)), _endDataIndexCursor);
					if (container2 != null)
					{
						AppendContainer(_endDataIndexCursor, container2, setPosition: false);
						if (_endDataIndexCursor == _adapter.GetCount() - 1 && _infinite == Infinite.BothWays)
						{
							_endDataIndexCursor = 0;
						}
					}
				}
				while (_topDataIndexCursor >= 0 && _recycleBin.activeContainers[0].top > 0f)
				{
					Container container3 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_topDataIndexCursor)), _topDataIndexCursor);
					if (container3 != null)
					{
						PrependContainer(_topDataIndexCursor, container3, setPosition: false);
						if (_infinite == Infinite.BothWays && _topDataIndexCursor < 0)
						{
							_topDataIndexCursor = _adapter.GetCount() - 1;
						}
					}
				}
			}
			else
			{
				while (_endDataIndexCursor < _adapter.GetCount() && _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1].right < rootPanel.baseClipRegion.z)
				{
					Container container4 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_endDataIndexCursor)), _endDataIndexCursor);
					if (container4 != null)
					{
						AppendContainer(_endDataIndexCursor, container4, setPosition: false);
						if (_endDataIndexCursor == _adapter.GetCount() && _infinite == Infinite.BothWays)
						{
							_endDataIndexCursor = 0;
						}
					}
				}
				while (_topDataIndexCursor >= 0 && _recycleBin.activeContainers[0].left > 0f)
				{
					Container container5 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_topDataIndexCursor)), _topDataIndexCursor);
					if (container5 != null)
					{
						PrependContainer(_topDataIndexCursor, container5, setPosition: false);
						if (_infinite == Infinite.BothWays && _topDataIndexCursor < 0)
						{
							_topDataIndexCursor = _adapter.GetCount() - 1;
						}
					}
				}
			}
			if (_infinite != Infinite.BothWays && _arrangement == Arrangement.Vertical)
			{
				if (_endDataIndexCursor == _adapter.GetCount() && _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1].bottom < rootPanel.baseClipRegion.w)
				{
					container = _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1];
					float num3 = (0f - rootPanel.baseClipRegion.w) * 0.5f + container.height * 0.5f - container.bounds.center.y + rootPanel.baseClipRegion.y;
					float num4 = container.coordinates.y - num3;
					_recycleBin.ScrapActiveContainers();
					_topDataIndexCursor = _adapter.GetCount() - 1;
					_endDataIndexCursor = _adapter.GetCount() - 1;
					container = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_adapter.GetCount() - 1)), _adapter.GetCount() - 1);
					container.isMeasured = false;
					container.Measure();
					container.dataIndex = _adapter.GetCount() - 1;
					container.coordinates.y = num3;
					container.coordinates.x = rootPanel.baseClipRegion.x;
					container.bottom = rootPanel.baseClipRegion.w;
					container.top = container.bottom - container.height;
					container.SetModelType(_adapter.GetModelType(_adapter.GetCount() - 1));
					container.SetParent(rootPanel.transform);
					container.SetScale(Vector3.one);
					container.rowId = _adapter.GetItemId(container.dataIndex);
					_recycleBin.AddToActiveContainers(container, endOfList: true);
					_topDataIndexCursor--;
					_endDataIndexCursor++;
					while (_topDataIndexCursor >= 0 && _recycleBin.activeContainers[0].top > 0f)
					{
						Container container6 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_topDataIndexCursor)), _topDataIndexCursor);
						if (container6 != null)
						{
							PrependContainer(_topDataIndexCursor, container6, setPosition: false);
						}
						if (_infinite == Infinite.BothWays && _topDataIndexCursor < 0)
						{
							_topDataIndexCursor = _adapter.GetCount() - 1;
						}
					}
					center = _mListViewBounds.center;
					center.y -= num4;
					_mListViewBounds.center = center;
				}
				if (_topDataIndexCursor < 0 && _recycleBin.activeContainers[0].top > 0f)
				{
					container = _recycleBin.activeContainers[0];
					float num5 = rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y - container.height * 0.5f - container.bounds.center.y;
					float num6 = container.coordinates.y - num5;
					_recycleBin.ScrapActiveContainers();
					_topDataIndexCursor = 0;
					_endDataIndexCursor = 0;
					container = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(0)), 0);
					container.isMeasured = false;
					container.Measure();
					container.dataIndex = _topDataIndexCursor;
					container.coordinates.y = num5;
					container.coordinates.x = rootPanel.baseClipRegion.x;
					container.bottom = container.height;
					container.top = 0f;
					container.SetModelType(_adapter.GetModelType(0));
					container.SetParent(rootPanel.transform);
					container.SetScale(Vector3.one);
					_recycleBin.AddToActiveContainers(container, endOfList: true);
					container.rowId = _adapter.GetItemId(container.dataIndex);
					_topDataIndexCursor--;
					_endDataIndexCursor++;
					while (_endDataIndexCursor < _adapter.GetCount() && _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1].bottom < rootPanel.baseClipRegion.w)
					{
						Container container7 = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_endDataIndexCursor)), _endDataIndexCursor);
						if (container7 != null)
						{
							AppendContainer(_endDataIndexCursor, container7, setPosition: false);
						}
						if (_endDataIndexCursor == _adapter.GetCount() && _infinite == Infinite.BothWays)
						{
							_endDataIndexCursor = 0;
						}
					}
					center = _mListViewBounds.center;
					center.y -= num6;
					_mListViewBounds.center = center;
				}
			}
			_firstVisibleContainer = _recycleBin.activeContainers[0];
			_lastVisibleContainer = _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1];
			_currentMaxListCount = _endDataIndexCursor;
			_currentMinListCount = _topDataIndexCursor;
			_refreshingContainers = false;
			if (boundsCalculationMode == BoundsCalculationMode.Incremental)
			{
				CalculateVirtualRelativeWidgetBounds(BoundsCalculationMode.Incremental);
				if (_arrangement == Arrangement.Horizontal)
				{
					center = _mListViewBounds.center;
					center.x += GetFirstVisibleContainerOnScreen().left;
					_mListViewBounds.center = center;
				}
				else
				{
					center = _mListViewBounds.center;
					center.y -= GetFirstVisibleContainerOnScreen().top;
					_mListViewBounds.center = center;
				}
			}
		}
		if (onPreLayout != null)
		{
			onPreLayout();
		}
		SetContainerPositions();
		PrintActiveContainers();
	}

	private void CheckScrollState()
	{
		if (!(rootPanel == null) && !(rootPanelTransform == null))
		{
			_previousPos = _currentPos;
			if (_arrangement == Arrangement.Vertical)
			{
				_currentPos = rootPanelTransform.localPosition.y;
			}
			else
			{
				_currentPos = rootPanel.clipOffset.x;
			}
			_movementDelta = _currentPos - _previousPos;
			if (press)
			{
				_cumulativeDelta += _movementDelta;
			}
			if (_movementDelta < 0f)
			{
				_scrollDirection = ((_arrangement == Arrangement.Vertical) ? ScrollDirection.Down : ScrollDirection.Right);
			}
			else if (_movementDelta > 0f)
			{
				_scrollDirection = ((_arrangement != Arrangement.Vertical) ? ScrollDirection.Left : ScrollDirection.Up);
			}
			else if (Math.Abs(_movementDelta) == 0f)
			{
				_scrollDirection = ScrollDirection.Stationary;
			}
		}
	}

	private bool ShouldRecycleContainer(Container container)
	{
		if (_arrangement == Arrangement.Horizontal)
		{
			if (rootPanel.clipOffset.x > container.right)
			{
				return true;
			}
		}
		else if (rootPanelTransform.localPosition.y + (float)cellSpaceY > container.bottom)
		{
			return true;
		}
		if (_arrangement == Arrangement.Horizontal)
		{
			if (rootPanel.clipOffset.x + rootPanel.baseClipRegion.z - container.bounds.center.x < container.left)
			{
				return true;
			}
		}
		else if (rootPanelTransform.localPosition.y + rootPanel.baseClipRegion.w - container.bounds.center.y + (float)cellSpaceX < container.top)
		{
			return true;
		}
		return false;
	}

	private bool ShouldAppendContainer(Container container)
	{
		if (_arrangement == Arrangement.Vertical)
		{
			if (rootPanelTransform.localPosition.y + rootPanel.baseClipRegion.w - (float)cellSpaceY > container.bottom)
			{
				return true;
			}
		}
		else if (rootPanel.clipOffset.x + rootPanel.baseClipRegion.z > container.right)
		{
			return true;
		}
		return false;
	}

	private bool ShouldPrependContainer(Container container)
	{
		if (_arrangement == Arrangement.Vertical)
		{
			if (rootPanelTransform.localPosition.y + (float)cellSpaceY < container.top)
			{
				return true;
			}
		}
		else if (rootPanel.clipOffset.x < container.left)
		{
			return true;
		}
		return false;
	}

	private void HandleScrollDown()
	{
		if (_recycleBin == null || _recycleBin.activeContainers.Count <= 0)
		{
			return;
		}
		while (ShouldRecycleContainer(_recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1]) && _recycleBin.activeContainers.Count - 1 > 0)
		{
			_endDataIndexCursor = _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1].dataIndex;
			Container activeContainer = _recycleBin.GetActiveContainer(_recycleBin.activeContainers.Count - 1);
			if (activeContainer.onRecycleContainer != null)
			{
				activeContainer.onRecycleContainer();
			}
			_recycleBin.AddScrapContainer(activeContainer);
			if (_recycleBin.activeContainers.Count > 0)
			{
				_bottomActiveContainer = _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1];
				_nextAppendPosition = _bottomActiveContainer.coordinates;
			}
			else
			{
				SDebug.LogError("Recycle bin has no active containers# Handle scroll up  #");
			}
		}
		while (ShouldPrependContainer(_recycleBin.activeContainers[0]) && _topDataIndexCursor >= 0 && _recycleBin.activeContainers[0].modelType != -100)
		{
			Container container = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_topDataIndexCursor)), _topDataIndexCursor);
			if (container == null)
			{
				continue;
			}
			PrependContainer(_topDataIndexCursor, container, setPosition: true);
			if (_currentMinListCount <= _topDataIndexCursor)
			{
				continue;
			}
			if (boundsCalculationMode == BoundsCalculationMode.Incremental)
			{
				Vector3 size = _mListViewBounds.size;
				Vector3 center = _mListViewBounds.center;
				if (_arrangement == Arrangement.Vertical)
				{
					size.y += container.height + (float)cellSpaceY;
					center.y += container.height / 2f + (float)cellSpaceY / 2f;
					_mListViewBounds.size = size;
					_mListViewBounds.center = center;
				}
				else
				{
					size.x += container.width + (float)cellSpaceX;
					center.x -= container.width / 2f + (float)cellSpaceX / 2f;
					_mListViewBounds.size = size;
					_mListViewBounds.center = center;
				}
			}
			_currentMinListCount = _topDataIndexCursor;
		}
		if (_infinite == Infinite.BothWays && _topDataIndexCursor < 0)
		{
			_topDataIndexCursor = _adapter.GetCount() - 1;
		}
		if (pullDownToRefreshList && _pullDownToRefreshState == PullDownToRefreshState.PullDown && _topDataIndexCursor == -1 && !_refreshingContainerAdded)
		{
			CreatePullDownToRefreshContainer();
			Vector2 coordinates = GetFirstVisibleContainerOnScreen().coordinates;
			coordinates.y += pullDownToRefreshContainer.height * 0.5f + GetFirstVisibleContainerOnScreen().height * 0.5f + (0f - pullDownToRefreshContainer.bounds.center.y) + GetFirstVisibleContainerOnScreen().bounds.center.y;
			pullDownToRefreshContainer.coordinates = coordinates;
			pullDownToRefreshContainer.bottom = GetFirstVisibleContainerOnScreen().top;
			pullDownToRefreshContainer.top = pullDownToRefreshContainer.bottom - pullDownToRefreshContainer.height;
			pullDownToRefreshContainer.SetPosition();
			_refreshingContainerAdded = true;
		}
	}

	private void HandleScrollUp()
	{
		if (_recycleBin == null || _recycleBin.activeContainers.Count <= 0)
		{
			return;
		}
		while (ShouldRecycleContainer(_recycleBin.activeContainers[0]) && _recycleBin.activeContainers.Count - 1 > 0)
		{
			_topDataIndexCursor = _recycleBin.activeContainers[0].dataIndex;
			Container activeContainer = _recycleBin.GetActiveContainer(0);
			if (activeContainer.onRecycleContainer != null)
			{
				activeContainer.onRecycleContainer();
			}
			_recycleBin.AddScrapContainer(activeContainer);
			if (_recycleBin.activeContainers.Count > 0)
			{
				_topActiveContainer = _recycleBin.activeContainers[0];
				_nextPrependPosition = _topActiveContainer.coordinates;
				continue;
			}
			SDebug.LogError("Recycle bin has no active containers# Handle scroll up  #");
			break;
		}
		while (ShouldAppendContainer(_recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1]) && _endDataIndexCursor < _adapter.GetCount())
		{
			Container container = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(_endDataIndexCursor)), _endDataIndexCursor);
			if (container == null)
			{
				continue;
			}
			AppendContainer(_endDataIndexCursor, container, setPosition: true);
			if (_currentMaxListCount >= _endDataIndexCursor)
			{
				continue;
			}
			if (boundsCalculationMode == BoundsCalculationMode.Incremental)
			{
				Vector3 size = _mListViewBounds.size;
				Vector3 center = _mListViewBounds.center;
				if (_arrangement == Arrangement.Vertical)
				{
					if (_endDataIndexCursor <= _adapter.GetCount() - 1)
					{
						size.y += container.height + (float)cellSpaceY;
						center.y -= container.height / 2f + (float)cellSpaceY / 2f;
						_mListViewBounds.size = size;
						_mListViewBounds.center = center;
					}
					else
					{
						size.y += container.height;
						center.y -= container.height / 2f;
						_mListViewBounds.size = size;
						_mListViewBounds.center = center;
					}
				}
				else if (_endDataIndexCursor <= _adapter.GetCount() - 1)
				{
					size.x += container.width + (float)cellSpaceX;
					center.x += container.width / 2f + (float)cellSpaceX / 2f;
					_mListViewBounds.size = size;
					_mListViewBounds.center = center;
				}
				else
				{
					size.x += container.width;
					center.x += container.width / 2f;
					_mListViewBounds.size = size;
					_mListViewBounds.center = center;
				}
			}
			_currentMaxListCount = _endDataIndexCursor;
		}
		if (_endDataIndexCursor == _adapter.GetCount() && _infinite == Infinite.BothWays)
		{
			_endDataIndexCursor = 0;
		}
	}

	private void CalculateVirtualRelativeWidgetBounds(BoundsCalculationMode mode)
	{
		Vector3 size;
		Vector3 center;
		switch (mode)
		{
		case BoundsCalculationMode.FullSize:
			if (_recycleBin != null)
			{
				if (_infinite == Infinite.None)
				{
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					float num6 = 0f;
					for (int i = 0; i < _adapter.GetCount(); i++)
					{
						Container container = _adapter.GetContainer(_recycleBin.GetScrapContainer(_adapter.GetModelType(i)), i, isMeasurePass: true);
						container.SetModelType(_adapter.GetModelType(i));
						container.Measure();
						container.SetParent(rootPanel.transform);
						container.SetScale(Vector3.one);
						if (_arrangement == Arrangement.Horizontal)
						{
							num3 += container.width + (float)cellSpaceX;
							if (_topDataIndexCursor > 0 && _topDataIndexCursor < _adapter.GetCount() && _topDataIndexCursor > i)
							{
								num6 += container.width + (float)cellSpaceX;
							}
						}
						else
						{
							num4 += container.height + (float)cellSpaceY;
							if (_topDataIndexCursor >= 0 && _topDataIndexCursor < _adapter.GetCount() && _topDataIndexCursor > i)
							{
								num6 += container.height + (float)cellSpaceY;
							}
						}
						num5 += container.bounds.size.z;
						_recycleBin.AddScrapContainer(container);
					}
					if (_arrangement == Arrangement.Horizontal)
					{
						size = new Vector3(num3 - (float)cellSpaceX, rootPanel.baseClipRegion.w, num5);
						center = new Vector3(num3 * 0.5f - rootPanel.baseClipRegion.z * 0.5f - num6 + rootPanel.baseClipRegion.x - (float)cellSpaceX / 2f, rootPanel.baseClipRegion.y, 0f);
					}
					else
					{
						size = new Vector3(rootPanel.baseClipRegion.z, num4 - (float)cellSpaceY, num5);
						center = new Vector3(rootPanel.baseClipRegion.x, (0f - num4) * 0.5f + rootPanel.baseClipRegion.w * 0.5f + num6 + (float)cellSpaceY / 2f + rootPanel.baseClipRegion.y, 0f);
					}
					scrollViewHeight = size.y;
					scrollViewWidth = size.x;
					if (_arrangement == Arrangement.Vertical && pullDownToRefreshList)
					{
						if (size.y < rootPanel.baseClipRegion.w)
						{
							size = new Vector3(rootPanel.baseClipRegion.z, rootPanel.baseClipRegion.w * 2f, num5);
							center = new Vector3(rootPanel.baseClipRegion.x, (0f - rootPanel.baseClipRegion.w) / 2f + num6, 0f);
						}
					}
					else if (_arrangement == Arrangement.Horizontal && !pullDownToRefreshList)
					{
					}
					Bounds mListViewBounds = new Bounds(center, size);
					_mListViewBounds = mListViewBounds;
					break;
				}
				if (_infinite == Infinite.BothWays)
				{
					if (_arrangement == Arrangement.Vertical)
					{
						size = new Vector3(rootPanel.baseClipRegion.z, rootPanel.baseClipRegion.w * 3f, 0f);
						center = new Vector3(0f, 0f, 0f);
					}
					else
					{
						size = new Vector3(rootPanel.baseClipRegion.z * 3f, rootPanel.baseClipRegion.w, 0f);
						center = new Vector3(0f, 0f, 0f);
					}
					Bounds mListViewBounds2 = new Bounds(center, size);
					_mListViewBounds = mListViewBounds2;
					break;
				}
			}
			_mListViewBounds = new Bounds(Vector3.zero, Vector3.zero);
			break;
		case BoundsCalculationMode.Incremental:
			if (_arrangement == Arrangement.Vertical)
			{
				float num = 0f;
				foreach (Container activeContainer in _recycleBin.activeContainers)
				{
					num += activeContainer.height + (float)cellSpaceY;
				}
				size = new Vector3(rootPanel.baseClipRegion.z, num, 0f);
				center = new Vector3(0f, (0f - num) * 0.5f + rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y, 0f);
			}
			else
			{
				float num2 = 0f;
				foreach (Container activeContainer2 in _recycleBin.activeContainers)
				{
					num2 += activeContainer2.width + (float)cellSpaceX;
				}
				size = new Vector3(num2, rootPanel.baseClipRegion.w, 0f);
				center = new Vector3(num2 * 0.5f - rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x, 0f, 0f);
			}
			_mListViewBounds = new Bounds(center, size);
			break;
		default:
			throw new ArgumentOutOfRangeException("mode");
		}
	}

	public Container GetFirstVisibleContainerOnScreen()
	{
		if (_recycleBin != null)
		{
			if (_recycleBin.activeContainers != null)
			{
				if (_recycleBin.activeContainers.Count > 0)
				{
					if (_recycleBin.activeContainers[0] != null)
					{
						_firstVisibleContainer = _recycleBin.activeContainers[0];
					}
					else
					{
						SDebug.LogError("GetFirstVisibleContainerOnScreen: _recycleBin.activeContainers[0] was null!");
					}
				}
			}
			else
			{
				SDebug.LogError("GetFirstVisibleContainerOnScreen: activeContainers was null!");
			}
		}
		else
		{
			SDebug.LogError("GetFirstVisibleContainerOnScreen: _recycleBin was null!");
		}
		return _firstVisibleContainer;
	}

	public Container GetLastVisibleContainerOnScreen()
	{
		if (_recycleBin.activeContainers.Count > 0)
		{
			_lastVisibleContainer = _recycleBin.activeContainers[_recycleBin.activeContainers.Count - 1];
		}
		return _lastVisibleContainer;
	}

	public List<Container> GetActiveContainers()
	{
		return _recycleBin.activeContainers;
	}

	[Conditional("SCROLL_VIEW_DEBUG_LOGS")]
	public static void Log(string msg)
	{
	}
}
