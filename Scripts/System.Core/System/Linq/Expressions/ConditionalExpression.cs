namespace System.Linq.Expressions;

public sealed class ConditionalExpression : Expression
{
	private Expression test;

	private Expression if_true;

	private Expression if_false;

	public Expression Test => test;

	public Expression IfTrue => if_true;

	public Expression IfFalse => if_false;
}
