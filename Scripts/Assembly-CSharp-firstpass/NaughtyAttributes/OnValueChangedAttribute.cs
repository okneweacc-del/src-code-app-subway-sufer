using System;

namespace NaughtyAttributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class OnValueChangedAttribute : MetaAttribute
{
	private string callbackName;

	public string CallbackName => callbackName;

	public OnValueChangedAttribute(string callbackName)
	{
		this.callbackName = callbackName;
	}
}
