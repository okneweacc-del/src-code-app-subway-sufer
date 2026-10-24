using System;

namespace UnityEngine;

[Serializable]
public sealed class GUISettings
{
	[SerializeField]
	private bool m_DoubleClickSelectsWord = true;

	[SerializeField]
	private bool m_TripleClickSelectsLine = true;

	[SerializeField]
	private Color m_CursorColor = Color.white;

	[SerializeField]
	private float m_CursorFlashSpeed = -1f;

	[SerializeField]
	private Color m_SelectionColor = new Color(0.5f, 0.5f, 1f);
}
