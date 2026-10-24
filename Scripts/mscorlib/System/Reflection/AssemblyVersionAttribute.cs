namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class AssemblyVersionAttribute : Attribute
{
	private readonly string Version__BackingField;

	public string Version => Version__BackingField;

	public AssemblyVersionAttribute(string version)
	{
		Version__BackingField = version;
	}
}
