using System.Collections.Generic;
using ABTest;
using Kiloo.Common.Extensions;
using Kiloo.Common.UI;
using UI.Popups.ABTestTesterPopupInternal;
using UnityEngine;

namespace UI.Popups;

public class ABTestTesterPopup : UIPopup
{
	private class ABTestTesterListAdapter : ListAdapter<ABTestData.ABTestDataItem>
	{
		private GameObject _cellPrefab;

		public ABTestTesterListAdapter(List<ABTestData.ABTestDataItem> identifiers, GameObject cell)
			: base(identifiers)
		{
			_cellPrefab = cell;
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
		{
			Container container2 = container;
			ABTestData.ABTestDataItem item = GetItem(dataIndex);
			if (container2 == null)
			{
				container2 = new Container(_cellPrefab);
				ABTestTesterCellHelper component = container2.model.GetComponent<ABTestTesterCellHelper>();
				container2.tag = component;
			}
			if (!isMeasurePass)
			{
				ABTestTesterCellHelper aBTestTesterCellHelper = (ABTestTesterCellHelper)container2.tag;
				aBTestTesterCellHelper.Init(item);
			}
			return container2;
		}
	}

	[SerializeField]
	private GameObject _cellPrefab;

	[SerializeField]
	private ScrollView _scrollView;

	[SerializeField]
	private GameObject _initializationInfo;

	private ABTestTesterListAdapter _adapter;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
		_initializationInfo.SetActive(!ABTestManager.isInitialized);
		List<ABTestData.ABTestDataItem> list = new List<ABTestData.ABTestDataItem>();
		if (ABTestManager.isInitialized)
		{
			foreach (ABTestData.ABTestDataItem testDatum in ABTestData.testData)
			{
				if (!testDatum.forceDisable && !testDatum.debugTitle.IsNullOrEmpty() && testDatum.debugPayload != null)
				{
					list.Add(testDatum);
				}
			}
		}
		_adapter = new ABTestTesterListAdapter(list, _cellPrefab);
	}

	protected override void Show()
	{
		_scrollView.SetAdapter(_adapter);
	}
}
