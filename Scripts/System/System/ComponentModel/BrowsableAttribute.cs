namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class BrowsableAttribute : Attribute
{
	private bool browsable;

	public static readonly BrowsableAttribute Default = new BrowsableAttribute(browsable: true);

	public static readonly BrowsableAttribute No = new BrowsableAttribute(browsable: false);

	public static readonly BrowsableAttribute Yes = new BrowsableAttribute(browsable: true);

	public bool Browsable => browsable;

	public BrowsableAttribute(bool browsable)
	{
		this.browsable = browsable;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is BrowsableAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((BrowsableAttribute)obj).Browsable == browsable;
	}

	public override int GetHashCode()
	{
		return browsable.GetHashCode();
	}
}
