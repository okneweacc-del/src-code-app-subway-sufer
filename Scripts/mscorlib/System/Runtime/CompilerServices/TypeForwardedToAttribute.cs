namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class TypeForwardedToAttribute : Attribute
{
	private readonly Type Destination__BackingField;

	public Type Destination => Destination__BackingField;

	public TypeForwardedToAttribute(Type destination)
	{
		Destination__BackingField = destination;
	}
}
