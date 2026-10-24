namespace System.Linq.Expressions;

public sealed class ConstantExpression : Expression
{
	private object value;

	public object Value => value;
}
