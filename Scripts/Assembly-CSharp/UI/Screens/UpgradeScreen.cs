using System.Collections.Generic;
using Data.Save;
using InApp;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Timer;
using UI.Screens.UpgradeScreenInternal;
using UnityEngine;
using Utilities;

namespace UI.Screens;

public class UpgradeScreen : UIScreen
{
	[SerializeField]
	private GameObject _consumablePrefab;

	[SerializeField]
	private GameObject _permanentPrefab;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private UIScrollView _scrollView;

	[SerializeField]
	private GameObject _listTitleComponent;

	[SerializeField]
	private bool _isPopup;

	public List<UpgradeHelper> cachedUpgradeHelpers = new List<UpgradeHelper>(Upgrades.upgrades.Count);

	private GameObject _bundlesButton;

	private bool _facebookLoginRunning;

	public static bool forceScrollRearrange;

	private const char SPLIT_CHARACTER = ';';

	private IEventManager _activeEventManager;

	private float _scrollPanelStartY;

	private const int TITLE_POPUP_FONT_SIZE = 23;

	private int _numberOfObjects;

	private int _indexExpanded = -1;

	public int indexExpanded
	{
		get
		{
			return _indexExpanded;
		}
		set
		{
			_indexExpanded = value;
		}
	}

	protected override void Init()
	{
		FillTable();
		HandleSafeArea();
		FrameTimer.ExecuteWithDelay(2, () =>
		{
			_scrollPanelStartY = _scrollView.transform.localPosition.y;
			SetScrollPanelYForTableElements();
		});
	}

	protected virtual void HandleSafeArea()
	{
		if (SafeArea.IsRestricted)
		{
			_scrollPanel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
			_scrollPanel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
			_scrollPanel.ResetAndUpdateAnchors();
		}
	}

	protected override void Show()
	{
		InAppManager.instance.onProductRequestDone += OnProductRequestDone;
		RefreshContent();
	}

	private void RefreshContent()
	{
		InAppData.BundleTier lastPurchasedBundle = UserSave.lastPurchasedBundle;
		if ((lastPurchasedBundle == InAppData.BundleTier.BundleTier5 || !BundlesActiveInThisLocale()) && _bundlesButton != null)
		{
			Object.Destroy(_bundlesButton);
			_bundlesButton = null;
		}
		if (_footerHandler != null)
		{
			_footerHandler.HandleStickers();
		}
		UIScreenController instance = UIScreenController.Instance;
		GameOverScreen gameOverScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as GameOverScreen;
		if (gameOverScreen != null)
		{
			gameOverScreen.StopCoinsCountUp();
		}
		_activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (IsJackpotEventActive())
		{
			forceScrollRearrange = true;
		}
		if (forceScrollRearrange)
		{
			forceScrollRearrange = false;
			if (_scrollView != null)
			{
				_scrollView.ResetPosition();
			}
		}
		FrameTimer.ExecuteWithDelay(2, _table.Reposition);
	}

	protected override void Hide()
	{
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
		InAppManager.instance.onProductRequestDone -= OnProductRequestDone;
	}

	public override void GainFocus()
	{
		if (!MysteryBox.AreTokensAvailable())
		{
			FillTable();
			_table.repositionNow = true;
		}
	}

	public override void LoseFocus()
	{
	}

	public override string GetFlurryName()
	{
		return "UpgradesUI_shop";
	}

	private bool IsJackpotEventActive()
	{
		if (_activeEventManager != null && _activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
		{
			return true;
		}
		return false;
	}

	public void SaleEnded()
	{
		_table.repositionNow = true;
	}

	private void FillTable()
	{
		_numberOfObjects = 0;
		cachedUpgradeHelpers.Clear();
		List<Transform> childList = _table.GetChildList();
		childList.ForEach((Transform k) =>
		{
			k.gameObject.Kill();
		});
		UIWidget uIWidget = _table.gameObject.AddWidget<UIWidget>();
		uIWidget.height = ((!_isPopup) ? 10 : 3);
		uIWidget.name = $"{_numberOfObjects:000}";
		_numberOfObjects++;
		GameObject gameObject = _table.gameObject.AddChild(_listTitleComponent);
		ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
		gameObject.name = $"{_numberOfObjects:000}";
		string text = Strings.Get("UPGRADE_SCREEN_SINGLE_USE");
		Color midnight = ColorData.instance.midnight;
		int fontSize = ((!_isPopup) ? (-1) : 23);
		component.Setup(text, midnight, 300, fontSize);
		_numberOfObjects++;
		UpgradeType[] consumableUpgradeTypes = Upgrades.consumableUpgradeTypes;
		foreach (UpgradeType upgradeType in consumableUpgradeTypes)
		{
			if (upgradeType != UpgradeType.tokenmysterybox || MysteryBox.AreTokensAvailable())
			{
				MakeBuyable(upgradeType, permanent: false);
				_numberOfObjects++;
			}
		}
		GameObject gameObject2 = _table.gameObject.AddChild(_listTitleComponent);
		ListTitleComponentHelper component2 = gameObject2.GetComponent<ListTitleComponentHelper>();
		gameObject2.name = $"{_numberOfObjects:000}";
		text = Strings.Get("UPGRADES_TITLE");
		midnight = ColorData.instance.midnight;
		fontSize = ((!_isPopup) ? (-1) : 23);
		component2.Setup(text, midnight, 300, fontSize);
		_numberOfObjects++;
		UpgradeType[] permanentUpgradeTypes = Upgrades.permanentUpgradeTypes;
		foreach (UpgradeType upgradeType2 in permanentUpgradeTypes)
		{
			MakeBuyable(upgradeType2, permanent: true);
			_numberOfObjects++;
		}
	}

	private void MakeBuyable(UpgradeType upgradeType, bool permanent)
	{
		GameObject gameObject;
		if (permanent)
		{
			gameObject = _table.gameObject.AddChild(_permanentPrefab);
			gameObject.GetComponent<UpgradeHelper>().InitPermanent(upgradeType, _scrollPanel);
		}
		else
		{
			gameObject = _table.gameObject.AddChild(_consumablePrefab);
			gameObject.GetComponent<UpgradeHelper>().InitSingle(upgradeType, _scrollPanel);
		}
		gameObject.name = $"{_numberOfObjects:000}";
		NGUITools.AddWidgetCollider(gameObject);
		cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
		MakeTableElementAlwaysVisible(gameObject, _numberOfObjects);
	}

	private void SetScrollPanelYForTableElements()
	{
		TableElementAlwaysVisible[] componentsInChildren = _table.GetComponentsInChildren<TableElementAlwaysVisible>();
		foreach (TableElementAlwaysVisible tableElementAlwaysVisible in componentsInChildren)
		{
			tableElementAlwaysVisible.ScrollPanelStartY = _scrollPanelStartY;
		}
	}

	private void MakeTableElementAlwaysVisible(GameObject go, int index)
	{
		TableElementAlwaysVisible component = go.GetComponent<TableElementAlwaysVisible>();
		if (component != null)
		{
			component.index = index;
		}
	}

	public bool IsLastElementOpen()
	{
		bool result = false;
		TableElementAlwaysVisible[] componentsInChildren = _table.GetComponentsInChildren<TableElementAlwaysVisible>();
		foreach (TableElementAlwaysVisible tableElementAlwaysVisible in componentsInChildren)
		{
			if (tableElementAlwaysVisible.index == _indexExpanded && tableElementAlwaysVisible.IsExpanded)
			{
				result = true;
			}
		}
		return result;
	}

	private bool BundlesActiveInThisLocale()
	{
		string locale = DeviceUtility.GetLocale();
		if (!string.IsNullOrEmpty(locale))
		{
			bool result = false;
			locale = locale.ToLower();
			locale = locale.Replace('-', '_');
			if (OnlineSettings.instance.TryGetValue("bundles_enabled_locales", out var valueString))
			{
				if (string.IsNullOrEmpty(valueString))
				{
					valueString = "all";
				}
			}
			else
			{
				valueString = "all";
			}
			if (OnlineSettings.instance.TryGetValue("bundles_disabled_locales", out var valueString2))
			{
				if (string.IsNullOrEmpty(valueString2))
				{
					valueString2 = "none";
				}
			}
			else
			{
				valueString2 = "none";
			}
			if (!valueString.Contains("all"))
			{
				valueString2 = "all";
			}
			valueString = valueString.ToLower();
			valueString = valueString.Replace('-', '_');
			valueString2 = valueString2.ToLower();
			valueString2 = valueString2.Replace('-', '_');
			string[] array = valueString.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			string[] array2 = valueString2.Split(';');
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = array2[j].Trim();
			}
			if (valueString.Contains("all"))
			{
				for (int k = 0; k < array2.Length; k++)
				{
					if (!locale.Contains(array2[k]))
					{
						result = true;
						break;
					}
				}
			}
			else if (valueString2.Contains("all"))
			{
				for (int l = 0; l < array.Length; l++)
				{
					if (locale.Contains(array[l]))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		return false;
	}

	private void OnProductRequestDone()
	{
		RefreshContent();
	}
}
