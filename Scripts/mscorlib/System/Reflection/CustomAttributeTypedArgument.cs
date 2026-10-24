using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
public struct CustomAttributeTypedArgument
{
	private Type argumentType;

	private object value;

	public override string ToString()
	{
		string text = ((value == null) ? string.Empty : value.ToString());
		if (argumentType == typeof(string))
		{
			return "\"" + text + "\"";
		}
		if (argumentType == typeof(Type))
		{
			return "typeof (" + text + ")";
		}
		if (argumentType.IsEnum)
		{
			return "(" + argumentType.Name + ")" + text;
		}
		return text;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CustomAttributeTypedArgument customAttributeTypedArgument))
		{
			return false;
		}
		return (customAttributeTypedArgument.argumentType != argumentType || value == null) ? (customAttributeTypedArgument.value == null) : value.Equals(customAttributeTypedArgument.value);
	}

	public override int GetHashCode()
	{
		return (argumentType.GetHashCode() << 16) + ((value != null) ? value.GetHashCode() : 0);
	}
}
