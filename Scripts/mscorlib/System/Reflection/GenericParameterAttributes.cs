namespace System.Reflection;

[Flags]
public enum GenericParameterAttributes
{
	Covariant = 1,
	Contravariant = 2,
	VarianceMask = Covariant | Contravariant,
	None = 0,
	ReferenceTypeConstraint = 4,
	NotNullableValueTypeConstraint = 8,
	DefaultConstructorConstraint = 0x10,
	SpecialConstraintMask = ReferenceTypeConstraint | NotNullableValueTypeConstraint | DefaultConstructorConstraint
}
