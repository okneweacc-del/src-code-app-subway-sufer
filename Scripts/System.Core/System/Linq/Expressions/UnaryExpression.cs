namespace System.Linq.Expressions;

public sealed class UnaryExpression : Expression
{
	private Expression operand;

	public Expression Operand => operand;
}
