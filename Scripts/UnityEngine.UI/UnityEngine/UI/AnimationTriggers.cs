using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI;

[Serializable]
public class AnimationTriggers
{
	private const string kDefaultNormalAnimName = "Normal";

	private const string kDefaultSelectedAnimName = "Highlighted";

	private const string kDefaultPressedAnimName = "Pressed";

	private const string kDefaultDisabledAnimName = "Disabled";

	[SerializeField]
	[FormerlySerializedAs("normalTrigger")]
	private string m_NormalTrigger = "Normal";

	[SerializeField]
	[FormerlySerializedAs("m_SelectedTrigger")]
	[FormerlySerializedAs("highlightedTrigger")]
	private string m_HighlightedTrigger = "Highlighted";

	[SerializeField]
	[FormerlySerializedAs("pressedTrigger")]
	private string m_PressedTrigger = "Pressed";

	[SerializeField]
	[FormerlySerializedAs("disabledTrigger")]
	private string m_DisabledTrigger = "Disabled";

	public string normalTrigger => m_NormalTrigger;

	public string highlightedTrigger => m_HighlightedTrigger;

	public string pressedTrigger => m_PressedTrigger;

	public string disabledTrigger => m_DisabledTrigger;
}
