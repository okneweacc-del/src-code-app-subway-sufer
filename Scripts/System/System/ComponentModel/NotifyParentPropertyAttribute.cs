namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NotifyParentPropertyAttribute : Attribute
{
	private bool notifyParent;

	public static readonly NotifyParentPropertyAttribute Default = new NotifyParentPropertyAttribute(notifyParent: false);

	public static readonly NotifyParentPropertyAttribute No = new NotifyParentPropertyAttribute(notifyParent: false);

	public static readonly NotifyParentPropertyAttribute Yes = new NotifyParentPropertyAttribute(notifyParent: true);

	public bool NotifyParent => notifyParent;

	public NotifyParentPropertyAttribute(bool notifyParent)
	{
		this.notifyParent = notifyParent;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is NotifyParentPropertyAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((NotifyParentPropertyAttribute)obj).NotifyParent == notifyParent;
	}

	public override int GetHashCode()
	{
		return notifyParent.GetHashCode();
	}
}
