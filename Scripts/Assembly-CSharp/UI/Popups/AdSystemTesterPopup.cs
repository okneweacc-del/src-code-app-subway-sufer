using System.Collections.Generic;
using Kiloo.Common.Ads;
using UnityEngine;

namespace UI.Popups;

public class AdSystemTesterPopup : UIPopup
{
	[SerializeField]
	private GameObject _cellPrefab;

	[SerializeField]
	private GameObject _cellParent;

	[SerializeField]
	private UITable _cellParentTable;

	private ICollection<AdLocationInfo> _adLocations;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
		_adLocations = AdLocations.adLocations;
		foreach (AdLocationInfo adLocation in _adLocations)
		{
			GameObject gameObject = _cellParent.AddChild(_cellPrefab);
			gameObject.GetComponent<AdSystemTesterCell>().Init(adLocation);
		}
		_cellParentTable.Reposition();
	}
}
