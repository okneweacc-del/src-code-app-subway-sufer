namespace System.Linq.Expressions;

public sealed class TypeBinaryExpression : Expression
{
	private Expression expression;

	private Type type_operand;

	public Expression Expression => expression;

	public Type TypeOperand => type_operand;
}
