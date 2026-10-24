namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DesignerCategoryAttribute : Attribute
{
	private string category;

	public static readonly DesignerCategoryAttribute Component = new DesignerCategoryAttribute("Component");

	public static readonly DesignerCategoryAttribute Form = new DesignerCategoryAttribute("Form");

	public static readonly DesignerCategoryAttribute Generic = new DesignerCategoryAttribute("Designer");

	public static readonly DesignerCategoryAttribute Default = new DesignerCategoryAttribute(string.Empty);

	public string Category => category;

	public DesignerCategoryAttribute(string category)
	{
		this.category = category;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DesignerCategoryAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((DesignerCategoryAttribute)obj).Category == category;
	}

	public override int GetHashCode()
	{
		return category.GetHashCode();
	}
}
