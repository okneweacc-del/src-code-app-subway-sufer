using UI.Generic.Layout;
using UnityEngine;

namespace UI.Helpers;

public class MostPopularStickerHelper : MonoBehaviour
{
	private const int EXTRA_DEPTH_ON_TOP_OF_PANEL_PLACED_ON = 50;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UIWidget _widget;

	[SerializeField]
	private HorizontalLayout _layout;

	[SerializeField]
	private UIPanel _contentPanel;

	[SerializeField]
	private UIPanel _maskPanel;

	public int height => _widget.height;

	private void OnEnable()
	{
		_title.text = Strings.Get("MOST_POPULAR_STICKER_TITLE");
		_layout.Align();
	}

	public void SetDepthFrom(int depth)
	{
		_contentPanel.depth = depth + 50;
		_maskPanel.depth = depth + 50;
	}
}
