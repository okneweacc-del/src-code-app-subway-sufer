using System.ComponentModel.Design;

namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
public sealed class DesignerAttribute : Attribute
{
	private string name;

	private string basetypename;

	public string DesignerBaseTypeName => basetypename;

	public string DesignerTypeName => name;

	public DesignerAttribute(string designerTypeName)
	{
		if (designerTypeName == null)
		{
			throw new NullReferenceException();
		}
		name = designerTypeName;
		basetypename = typeof(IDesigner).FullName;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DesignerAttribute))
		{
			return false;
		}
		return ((DesignerAttribute)obj).DesignerBaseTypeName.Equals(basetypename) && ((DesignerAttribute)obj).DesignerTypeName.Equals(name);
	}

	public override int GetHashCode()
	{
		return (name + basetypename).GetHashCode();
	}
}
