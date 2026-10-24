using UnityEngine;

public class KUIScrollView : UIScrollView
{
	public ScrollView scrollView;

	public override Bounds bounds => (!(scrollView == null)) ? scrollView.listViewBounds : base.bounds;

	protected override bool shouldMove
	{
		get
		{
			if (!disableDragIfFits)
			{
				return true;
			}
			if (mPanel == null)
			{
				mPanel = GetComponent<UIPanel>();
			}
			Vector4 baseClipRegion = mPanel.baseClipRegion;
			Bounds bounds = this.bounds;
			if (base.canMoveHorizontally && bounds.size.x > baseClipRegion.z)
			{
				return true;
			}
			if (base.canMoveVertically && bounds.size.y > baseClipRegion.w)
			{
				return true;
			}
			return false;
		}
	}

	public override void MoveRelative(Vector3 relative)
	{
		if (scrollView.pullDownToRefreshList)
		{
			float num = scrollView.rootPanelTransform.localPosition.y - scrollView.GetFirstVisibleContainerOnScreen().top;
			float num2 = scrollView.rootPanelTransform.localPosition.y + relative.y;
			if (num2 > 0f && scrollView.scrollViewHeight < scrollView.rootPanel.baseClipRegion.w)
			{
				float y = relative.y - Mathf.Abs(scrollView.rootPanelTransform.localPosition.y);
				relative.y = y;
			}
			if (scrollView.scrollViewHeight < scrollView.rootPanel.baseClipRegion.w && num >= 0f && relative.y >= 0f)
			{
				relative = Vector3.zero;
			}
		}
		base.MoveRelative(relative);
	}
}
