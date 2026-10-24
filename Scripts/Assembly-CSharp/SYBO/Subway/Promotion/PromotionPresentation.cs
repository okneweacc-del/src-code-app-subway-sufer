using System;
using UnityEngine;

namespace SYBO.Subway.Promotion;

[Serializable]
public class PromotionPresentation
{
	[Header("Background")]
	[SerializeField]
	private Color _backgroundColor;

	[SerializeField]
	private Color _flashColor;

	[SerializeField]
	private Color _beamColor;

	public Color BackgroundColor => _backgroundColor;

	public Color BeamColor => _beamColor;

	public Color FlashColor => _flashColor;
}
