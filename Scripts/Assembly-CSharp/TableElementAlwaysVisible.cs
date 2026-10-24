using System.Collections;
using System.Diagnostics;
using UI.Screens;
using UnityEngine;

public class TableElementAlwaysVisible : MonoBehaviour
{
	private UIPanel _scrollPanel;

	[SerializeField]
	private TweenHeight _largestTweenHeight;

	private float _scrollPanelStartY;

	private int _index;

	private UpgradeScreen _upgradeScreen;

	private UIPanel scrollPanel
	{
		get
		{
			if (!_scrollPanel)
			{
				_scrollPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
			}
			return _scrollPanel;
		}
	}

	public float ScrollPanelStartY
	{
		set
		{
			_scrollPanelStartY = value;
		}
	}

	public bool IsExpanded => _largestTweenHeight.cachedWidget.height != _largestTweenHeight.from;

	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	private UpgradeScreen upgradeScreen => (!_upgradeScreen) ? (_upgradeScreen = GetComponentInParent<UpgradeScreen>()) : _upgradeScreen;

	private void TheElementIsOnBottom(float bottomPointOnScreen, float bottomPointOfTable)
	{
		if (AreAllClosed())
		{
			if (WillExpand())
			{
				float noOfPixels = bottomPointOfTable - bottomPointOnScreen;
				StartCoroutine(MovePanelAfterFrames(noOfPixels, 3));
			}
		}
		else if (hasAnotherElementOpenOnBottom())
		{
			if (WillExpand())
			{
				float noOfPixels2 = bottomPointOfTable - bottomPointOnScreen;
				MovePanelBy(noOfPixels2);
			}
		}
		else if (hasAnotherElementOpenOnTop() && WillExpand())
		{
			float num = bottomPointOfTable - bottomPointOnScreen;
			MovePanelBy(num - (float)(_largestTweenHeight.to - _largestTweenHeight.from));
		}
	}

	private void TheElementIsOnTop(float topPointOnScreen, float topPointOfTable)
	{
		if (AreAllClosed())
		{
			if (WillExpand())
			{
				float noOfPixels = topPointOfTable - topPointOnScreen;
				MovePanelBy(noOfPixels);
			}
		}
		else if (hasAnotherElementOpenOnTop())
		{
			if (WillExpand())
			{
				float num = 0f - (topPointOnScreen - topPointOfTable);
				MovePanelBy((float)(-(_largestTweenHeight.to - _largestTweenHeight.from)) + num);
			}
		}
		else if (WillExpand())
		{
			float noOfPixels2 = 0f - (topPointOnScreen - topPointOfTable);
			MovePanelBy(noOfPixels2);
		}
	}

	private void OnClick()
	{
		ToggleButtonScale();
	}

	public void ToggleButtonScale()
	{
		float num = 0f;
		float num2 = ((!WillExpand()) ? (base.gameObject.transform.localPosition.y + num) : (base.gameObject.transform.localPosition.y - num));
		float y = scrollPanel.transform.localPosition.y;
		float num3 = ((!WillExpand()) ? _largestTweenHeight.from : _largestTweenHeight.to);
		float num4 = num2 - num3;
		float num5 = num4 + y;
		float num6 = 0f - scrollPanel.baseClipRegion.w + _scrollPanelStartY;
		float num7 = num2;
		float num8 = num7 + y;
		float scrollPanelStartY = _scrollPanelStartY;
		if (num5 < num6)
		{
			TheElementIsOnBottom(num5, num6);
		}
		bool flag = false;
		if (hasAnotherElementOpenOnTop())
		{
			float num9 = num8 + (float)(_largestTweenHeight.to - _largestTweenHeight.from);
			if (num9 > scrollPanelStartY)
			{
				TheElementIsOnTop(num8, scrollPanelStartY);
				flag = true;
			}
		}
		if (num8 > scrollPanelStartY && !flag)
		{
			TheElementIsOnTop(num8, scrollPanelStartY);
		}
		if (!WillExpand())
		{
			upgradeScreen.indexExpanded = _index;
		}
	}

	private IEnumerator MovePanelAfterFrames(float noOfPixels, int frames)
	{
		int numberOfFrames = 0;
		while (numberOfFrames < frames)
		{
			numberOfFrames++;
			yield return null;
		}
		MovePanelBy(noOfPixels);
	}

	private void MovePanelBy(float noOfPixels)
	{
		Vector3 localPosition = scrollPanel.transform.localPosition;
		localPosition.y += noOfPixels;
		scrollPanel.transform.localPosition = localPosition;
		Vector2 zero = Vector2.zero;
		zero.y = noOfPixels;
		scrollPanel.clipOffset -= zero;
	}

	private bool hasAnotherElementOpenOnTop()
	{
		return upgradeScreen.indexExpanded < _index && upgradeScreen.indexExpanded != -1;
	}

	private bool hasAnotherElementOpenOnBottom()
	{
		return upgradeScreen.indexExpanded > _index && upgradeScreen.indexExpanded != -1;
	}

	private bool WillExpand()
	{
		if (_largestTweenHeight.cachedWidget.height == _largestTweenHeight.from)
		{
			return true;
		}
		return false;
	}

	private bool AreAllClosed()
	{
		return !upgradeScreen.IsLastElementOpen();
	}

	private void OnDestroy()
	{
		_upgradeScreen = null;
	}

	[Conditional("ENABLE_TABLE_ELEMENT_ALWAYS_VISIBLE_LOGS")]
	private static void Log(string msg, Object context = null)
	{
	}
}
