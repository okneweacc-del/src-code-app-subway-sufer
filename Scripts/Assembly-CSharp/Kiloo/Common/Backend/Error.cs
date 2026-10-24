using System;
using System.Collections.Generic;

namespace Kiloo.Common.Backend;

[Serializable]
public class Error
{
	public string category;

	public int code;

	public string message;

	public string exception;

	public Dictionary<string, string> info;

	public override string ToString()
	{
		if (exception != null)
		{
			return $"{category} error {code}: {message} ({exception})";
		}
		return $"{category} error {code}: {message}";
	}

	public ErrorCategory GetErrorCategory()
	{
		return GetErrorCategory(category);
	}

	public static ErrorCategory GetErrorCategory(string category)
	{
		try
		{
			return (ErrorCategory)Enum.Parse(typeof(ErrorCategory), category, ignoreCase: true);
		}
		catch (Exception)
		{
			return ErrorCategory.Unknown;
		}
	}
}
