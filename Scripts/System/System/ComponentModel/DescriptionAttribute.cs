namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public class DescriptionAttribute : Attribute
{
	private string desc;

	public static readonly DescriptionAttribute Default = new DescriptionAttribute();

	public virtual string Description => DescriptionValue;

	protected string DescriptionValue => desc;

	public DescriptionAttribute()
	{
		desc = string.Empty;
	}

	public DescriptionAttribute(string name)
	{
		desc = name;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DescriptionAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((DescriptionAttribute)obj).Description == desc;
	}

	public override int GetHashCode()
	{
		return desc.GetHashCode();
	}
}
