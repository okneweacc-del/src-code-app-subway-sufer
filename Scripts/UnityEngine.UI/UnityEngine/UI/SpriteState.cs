using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI;

[Serializable]
public struct SpriteState : IEquatable<SpriteState>
{
	[FormerlySerializedAs("m_SelectedSprite")]
	[FormerlySerializedAs("highlightedSprite")]
	[SerializeField]
	private Sprite m_HighlightedSprite;

	[FormerlySerializedAs("pressedSprite")]
	[SerializeField]
	private Sprite m_PressedSprite;

	[FormerlySerializedAs("disabledSprite")]
	[SerializeField]
	private Sprite m_DisabledSprite;

	public Sprite highlightedSprite => m_HighlightedSprite;

	public Sprite pressedSprite => m_PressedSprite;

	public Sprite disabledSprite => m_DisabledSprite;

	public bool Equals(SpriteState other)
	{
		return highlightedSprite == other.highlightedSprite && pressedSprite == other.pressedSprite && disabledSprite == other.disabledSprite;
	}
}
