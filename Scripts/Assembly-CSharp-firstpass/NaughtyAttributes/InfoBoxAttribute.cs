using System;

namespace NaughtyAttributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class InfoBoxAttribute : MetaAttribute
{
	private string text;

	private InfoBoxType type;

	private string visibleIf;

	public string Text => text;

	public InfoBoxType Type => type;

	public string VisibleIf => visibleIf;

	public InfoBoxAttribute(string text, InfoBoxType type = InfoBoxType.Normal, string visibleIf = null)
	{
		this.text = text;
		this.type = type;
		this.visibleIf = visibleIf;
	}

	public InfoBoxAttribute(string text, string visibleIf)
		: this(text, InfoBoxType.Normal, visibleIf)
	{
	}
}
