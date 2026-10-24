using UnityEngine.Scripting;

namespace UnityEngine;

internal class ScrollViewState
{
	public Rect position;

	public Rect visibleRect;

	public Rect viewRect;

	public Vector2 scrollPosition;

	public bool apply;

	[RequiredByNativeCode]
	public ScrollViewState()
	{
	}
}
