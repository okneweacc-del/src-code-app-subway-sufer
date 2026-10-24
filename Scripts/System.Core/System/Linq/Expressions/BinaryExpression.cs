namespace System.Linq.Expressions;

public sealed class BinaryExpression : Expression
{
	private Expression left;

	private Expression right;

	private LambdaExpression conversion;

	public Expression Left => left;

	public Expression Right => right;

	public LambdaExpression Conversion => conversion;
}
