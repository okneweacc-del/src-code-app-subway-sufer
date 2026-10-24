using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

public class TextEditor
{
	public enum DblClickSnapping : byte
	{
		WORDS = 0,
		PARAGRAPHS = 1
	}

	public TouchScreenKeyboard keyboardOnScreen = null;

	public int controlID = 0;

	public GUIStyle style = GUIStyle.none;

	public bool multiline = false;

	public bool hasHorizontalCursorPos = false;

	public bool isPasswordField = false;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal bool m_HasFocus;

	public Vector2 scrollOffset = Vector2.zero;

	private GUIContent m_Content = new GUIContent();

	private int m_CursorIndex = 0;

	private int m_SelectIndex = 0;

	private bool m_RevealCursor = false;

	private bool m_MouseDragSelectsWholeWords = false;

	private int m_DblClickInitPos = 0;

	private DblClickSnapping m_DblClickSnap = DblClickSnapping.WORDS;

	private bool m_bJustSelected = false;

	private int m_iAltCursorPos = -1;

	public string text
	{
		get
		{
			return m_Content.text;
		}
		set
		{
			m_Content.text = value ?? string.Empty;
			EnsureValidCodePointIndex(ref m_CursorIndex);
			EnsureValidCodePointIndex(ref m_SelectIndex);
		}
	}

	public int cursorIndex
	{
		get
		{
			return m_CursorIndex;
		}
		set
		{
			int num = m_CursorIndex;
			m_CursorIndex = value;
			EnsureValidCodePointIndex(ref m_CursorIndex);
			if (m_CursorIndex != num)
			{
				m_RevealCursor = true;
				OnCursorIndexChange();
			}
		}
	}

	public int selectIndex
	{
		get
		{
			return m_SelectIndex;
		}
		set
		{
			int num = m_SelectIndex;
			m_SelectIndex = value;
			EnsureValidCodePointIndex(ref m_SelectIndex);
			if (m_SelectIndex != num)
			{
				OnSelectIndexChange();
			}
		}
	}

	[RequiredByNativeCode]
	public TextEditor()
	{
	}

	private void ClearCursorPos()
	{
		hasHorizontalCursorPos = false;
		m_iAltCursorPos = -1;
	}

	public void OnFocus()
	{
		if (multiline)
		{
			int num = (selectIndex = 0);
			cursorIndex = num;
		}
		else
		{
			SelectAll();
		}
		m_HasFocus = true;
	}

	public void SelectAll()
	{
		cursorIndex = 0;
		selectIndex = text.Length;
		ClearCursorPos();
	}

	public bool DeleteSelection()
	{
		if (cursorIndex == selectIndex)
		{
			return false;
		}
		if (cursorIndex < selectIndex)
		{
			m_Content.text = text.Substring(0, cursorIndex) + text.Substring(selectIndex, text.Length - selectIndex);
			selectIndex = cursorIndex;
		}
		else
		{
			m_Content.text = text.Substring(0, selectIndex) + text.Substring(cursorIndex, text.Length - cursorIndex);
			cursorIndex = selectIndex;
		}
		ClearCursorPos();
		return true;
	}

	public void ReplaceSelection(string replace)
	{
		DeleteSelection();
		m_Content.text = text.Insert(cursorIndex, replace);
		selectIndex = (cursorIndex += replace.Length);
		ClearCursorPos();
	}

	public void Copy()
	{
		if (selectIndex != cursorIndex && !isPasswordField)
		{
			string systemCopyBuffer = ((cursorIndex >= selectIndex) ? text.Substring(selectIndex, cursorIndex - selectIndex) : text.Substring(cursorIndex, selectIndex - cursorIndex));
			GUIUtility.systemCopyBuffer = systemCopyBuffer;
		}
	}

	private static string ReplaceNewlinesWithSpaces(string value)
	{
		value = value.Replace("\r\n", " ");
		value = value.Replace('\n', ' ');
		value = value.Replace('\r', ' ');
		return value;
	}

	public bool Paste()
	{
		string text = GUIUtility.systemCopyBuffer;
		if (text != "")
		{
			if (!multiline)
			{
				text = ReplaceNewlinesWithSpaces(text);
			}
			ReplaceSelection(text);
			return true;
		}
		return false;
	}

	internal virtual void OnCursorIndexChange()
	{
	}

	internal virtual void OnSelectIndexChange()
	{
	}

	private void ClampTextIndex(ref int index)
	{
		index = Mathf.Clamp(index, 0, text.Length);
	}

	private void EnsureValidCodePointIndex(ref int index)
	{
		ClampTextIndex(ref index);
		if (!IsValidCodePointIndex(index))
		{
			index = NextCodePointIndex(index);
		}
	}

	private bool IsValidCodePointIndex(int index)
	{
		if (index < 0 || index > text.Length)
		{
			return false;
		}
		if (index == 0 || index == text.Length)
		{
			return true;
		}
		return !char.IsLowSurrogate(text[index]);
	}

	private int NextCodePointIndex(int index)
	{
		if (index < text.Length)
		{
			index++;
		}
		while (index < text.Length && char.IsLowSurrogate(text[index]))
		{
			index++;
		}
		return index;
	}
}
