using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI;

[Serializable]
public struct ColorBlock : IEquatable<ColorBlock>
{
	[FormerlySerializedAs("normalColor")]
	[SerializeField]
	private Color m_NormalColor;

	[SerializeField]
	[FormerlySerializedAs("m_SelectedColor")]
	[FormerlySerializedAs("highlightedColor")]
	private Color m_HighlightedColor;

	[SerializeField]
	[FormerlySerializedAs("pressedColor")]
	private Color m_PressedColor;

	[SerializeField]
	[FormerlySerializedAs("disabledColor")]
	private Color m_DisabledColor;

	[SerializeField]
	[Range(1f, 5f)]
	private float m_ColorMultiplier;

	[SerializeField]
	[FormerlySerializedAs("fadeDuration")]
	private float m_FadeDuration;

	public Color normalColor => m_NormalColor;

	public Color highlightedColor => m_HighlightedColor;

	public Color pressedColor => m_PressedColor;

	public Color disabledColor => m_DisabledColor;

	public float colorMultiplier
	{
		get
		{
			return m_ColorMultiplier;
		}
		set
		{
			m_ColorMultiplier = value;
		}
	}

	public float fadeDuration
	{
		get
		{
			return m_FadeDuration;
		}
		set
		{
			m_FadeDuration = value;
		}
	}

	public static ColorBlock defaultColorBlock => new ColorBlock
	{
		m_NormalColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		m_HighlightedColor = new Color32(245, 245, 245, byte.MaxValue),
		m_PressedColor = new Color32(200, 200, 200, byte.MaxValue),
		m_DisabledColor = new Color32(200, 200, 200, 128),
		colorMultiplier = 1f,
		fadeDuration = 0.1f
	};

	public override bool Equals(object obj)
	{
		if (!(obj is ColorBlock))
		{
			return false;
		}
		return Equals((ColorBlock)obj);
	}

	public bool Equals(ColorBlock other)
	{
		return normalColor == other.normalColor && highlightedColor == other.highlightedColor && pressedColor == other.pressedColor && disabledColor == other.disabledColor && colorMultiplier == other.colorMultiplier && fadeDuration == other.fadeDuration;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
