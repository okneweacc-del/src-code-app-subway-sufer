using System;
using System.Collections;
using System.Collections.Generic;

namespace SYBO.Components.Utilities;

public class ToStringObject
{
	private readonly string _objectTypeName;

	private Dictionary<string, string> _members;

	private bool _isOneLine = true;

	public ToStringObject(object obj, bool isOneLine = true)
	{
		if (obj is Type)
		{
			_objectTypeName = (obj as Type).Name;
		}
		else
		{
			_objectTypeName = obj.GetType().Name;
		}
		_isOneLine = isOneLine;
		_members = new Dictionary<string, string>();
	}

	public void AddMember(string name, object value)
	{
		_members.Add(name, GetValueAsString(value));
	}

	public string GetValueAsString(object value)
	{
		string empty = string.Empty;
		if (value is IEnumerable enumerable && value.GetType() != typeof(string))
		{
			string text = string.Empty;
			foreach (object item in enumerable)
			{
				text = text + item.ToString() + ",";
			}
			if (text.Length > 0)
			{
				text = text.Substring(0, text.Length - 2);
			}
			return empty + $"{text}";
		}
		return empty + $"{value}";
	}

	public override string ToString()
	{
		string empty = string.Empty;
		empty = ((!_isOneLine) ? (empty + $"[{_objectTypeName}...\n(\n") : (empty + $"[{_objectTypeName} ("));
		int num = 0;
		foreach (KeyValuePair<string, string> member in _members)
		{
			empty = ((member.Value != null && !object.ReferenceEquals(member.Value, null) && member.Value.ToString().Length != 0) ? (empty + $"{member.Key}={member.Value}") : (empty + $"<color=red>{member.Key}={member.Value}</color>"));
			if (num++ < _members.Count - 1)
			{
				empty += ", ";
				if (!_isOneLine)
				{
					empty += Environment.NewLine;
				}
			}
		}
		if (!_isOneLine)
		{
			empty += Environment.NewLine;
		}
		empty += string.Format(")]", GetType().Name);
		if (!_isOneLine)
		{
			empty += Environment.NewLine;
		}
		return empty;
	}
}
