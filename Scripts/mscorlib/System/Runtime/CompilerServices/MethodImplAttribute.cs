namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
public sealed class MethodImplAttribute : Attribute
{
	public MethodCodeType MethodCodeType;

	private readonly MethodImplOptions Value__BackingField;

	public MethodImplOptions Value => Value__BackingField;

	public MethodImplAttribute(MethodImplOptions methodImplOptions)
	{
		Value__BackingField = methodImplOptions;
	}

	public MethodImplAttribute(short value)
	{
		Value__BackingField = (MethodImplOptions)value;
	}

	public MethodImplAttribute()
	{
	}
}
