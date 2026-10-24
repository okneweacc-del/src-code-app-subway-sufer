namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DefaultEventAttribute : Attribute
{
	private string eventName;

	public static readonly DefaultEventAttribute Default = new DefaultEventAttribute(null);

	public DefaultEventAttribute(string name)
	{
		eventName = name;
	}

	public override bool Equals(object o)
	{
		if (!(o is DefaultEventAttribute))
		{
			return false;
		}
		return ((DefaultEventAttribute)o).eventName == eventName;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
