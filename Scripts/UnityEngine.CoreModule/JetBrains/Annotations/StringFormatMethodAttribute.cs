using System;

namespace JetBrains.Annotations;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class StringFormatMethodAttribute : Attribute
{
	private string FormatParameterName { set; }

	public StringFormatMethodAttribute(string formatParameterName)
	{
		FormatParameterName = formatParameterName;
	}
}
