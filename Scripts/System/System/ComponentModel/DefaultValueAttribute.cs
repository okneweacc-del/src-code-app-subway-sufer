namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public class DefaultValueAttribute : Attribute
{
	private object DefaultValue;

	public virtual object Value => DefaultValue;

	public DefaultValueAttribute(bool value)
	{
		DefaultValue = value;
	}

	public DefaultValueAttribute(string value)
	{
		DefaultValue = value;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DefaultValueAttribute defaultValueAttribute))
		{
			return false;
		}
		if (DefaultValue == null)
		{
			return defaultValueAttribute.Value == null;
		}
		return DefaultValue.Equals(defaultValueAttribute.Value);
	}

	public override int GetHashCode()
	{
		if (DefaultValue == null)
		{
			return base.GetHashCode();
		}
		return DefaultValue.GetHashCode();
	}
}
