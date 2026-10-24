namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public sealed class EditorAttribute : Attribute
{
	private string name;

	private string basename;

	public string EditorBaseTypeName => basename;

	public string EditorTypeName => name;

	public EditorAttribute(string typeName, string baseTypeName)
	{
		name = typeName;
		basename = baseTypeName;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is EditorAttribute))
		{
			return false;
		}
		return ((EditorAttribute)obj).EditorBaseTypeName.Equals(basename) && ((EditorAttribute)obj).EditorTypeName.Equals(name);
	}

	public override int GetHashCode()
	{
		return (name + basename).GetHashCode();
	}
}
