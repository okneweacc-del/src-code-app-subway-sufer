using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[Serializable]
[StructLayout((LayoutKind)0)]
public class GUIContent
{
	[SerializeField]
	private string m_Text = string.Empty;

	[SerializeField]
	private Texture m_Image;

	[SerializeField]
	private string m_Tooltip = string.Empty;

	private static readonly GUIContent s_Text = new GUIContent();

	private static readonly GUIContent s_Image = new GUIContent();

	private static readonly GUIContent s_TextImage = new GUIContent();

	public static GUIContent none = new GUIContent("");

	public string text
	{
		get
		{
			return m_Text;
		}
		set
		{
			m_Text = value;
		}
	}

	public Texture image
	{
		set
		{
			m_Image = value;
		}
	}

	public string tooltip
	{
		set
		{
			m_Tooltip = value;
		}
	}

	public GUIContent()
	{
	}

	public GUIContent(string text)
		: this(text, null, string.Empty)
	{
	}

	public GUIContent(string text, string tooltip)
		: this(text, null, tooltip)
	{
	}

	public GUIContent(string text, Texture image, string tooltip)
	{
		this.text = text;
		this.image = image;
		this.tooltip = tooltip;
	}

	public GUIContent(GUIContent src)
	{
		text = src.m_Text;
		image = src.m_Image;
		tooltip = src.m_Tooltip;
	}

	internal static GUIContent Temp(string t)
	{
		s_Text.m_Text = t;
		s_Text.m_Tooltip = string.Empty;
		return s_Text;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void ClearStaticCache()
	{
		s_Text.m_Text = null;
		s_Text.m_Tooltip = string.Empty;
		s_Image.m_Image = null;
		s_Image.m_Tooltip = string.Empty;
		s_TextImage.m_Text = null;
		s_TextImage.m_Image = null;
	}
}
